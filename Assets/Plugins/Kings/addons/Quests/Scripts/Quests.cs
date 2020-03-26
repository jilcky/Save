using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Main Script for the quests.
/// 
/// Quests is a small quest system.
/// The quests progress is stored in the player prefs and can be defined with the use of scriptable objects
/// </summary>


public class Quests : TranslatableContent
{
    [System.Serializable] public class mEvent : UnityEvent { }

    //verbose debugging if activated in the inspector
    public bool verboseDebug = false;
    void mDebug(string txt)
    {
        if (verboseDebug == true)
        {
            Debug.Log(txt);
        }
    }


    public static Quests instance;                  //Hmm.. another singleton. This manager can only be used once per scene.
    string jsonKey = "Quests";                      //Save key for the Playerprefs.
    public System.Action OnQuestbookChangeAction;   //Callback possibility for other scripts to get informed if something changes.
    bool loaded = false;                            //Internally information, if loading occured.

    [Tooltip("Please specify the maximal number of active quests.")]
    public int maxNrOfActiveQuests = 3;
    [Tooltip("Check if number of active quests shall be autmatically refilled to maximum. The internal check occures if something changes.")]
    public bool autoRefillActiveQuests = false;

    private void Awake()
    {
        instance = this;
        loadCatalog();
        load();
    }

    private void Start()
    {
        TranslationManager.instance.registerTranslateableContentScript(this);
        if (CardStack.instance != null)
        {
            CardStack.instance.OnCardDestroy += OnCardDestroy;
        }
        StartCoroutine(delayedActualize());
    }

    //if an card is destroyed, the questsystem does actions like checking the conditions for active quests
    void OnCardDestroy()
    {
        checkIfActiveQuestIsFullfilledByCondition();
    }

    //Data class for the fullfillment popup.
    [System.Serializable]
    public class C_QuestFullfillPopup
    {
        [ReadOnlyInspector] public C_QuestState selectedElement;
        [HideInInspector] public Quest_UIItem uiItem;
        public GameObject popupPrefab;
        public GameObject popupParent;
        [HideInInspector] public GameObject spawnedQuestPopupPanel;
    }
    public C_QuestFullfillPopup questFullfillmentPopup;

    //Types of actions to change the state of a quest. At the moment only "fullfill_if_active" is available, but:
    //for future expanstion like grinding quests it is also possible to generate new types like 'add' some progress to an quest.
    [System.Serializable]
    public enum E_QuestChangeAction
    {
        fullfill_if_active
    }

    [System.Serializable]
    public class C_QuestChange
    {
        public E_QuestChangeAction questAction;
        public QuestDefinition quest;
    }

    public void computeQuestChange(C_QuestChange questChange)
    {
        switch (questChange.questAction)
        {
            case E_QuestChangeAction.fullfill_if_active:
                fullfill_quest_if_active(questChange.quest);
                break;
            //+some future cases like add 1 to grinding or test condition for reachement...?
            default:
                Debug.LogWarning("The quest change action '" + questChange.questAction.ToString() + "' is not implemented yet.");
                break;
        }
    }

    /// <summary>
    /// Count how many quest are active at the moment.
    /// </summary>
    /// <returns></returns>
    public int GetNrOfActiveQuests()
    {
        return questBook.activeQuests.Count;
    }

    /// <summary>
    /// Count how many quest are available. Without zombies.
    /// </summary>
    /// <returns></returns>
    public int GetNrOfAvailableQuests()
    {
        int cnt = 0;
        foreach (C_QuestState qs in questBook.quests)
        {
            if (qs.loadedFromCatalog == true)
            {
                cnt++;
            }
        }
        return cnt;
    }

    /// <summary>
    /// Count how many quest are finished.
    /// </summary>
    /// <returns></returns>
    public int GetNrOfFinishedQuests()
    {
        int cnt = 0;
        foreach (C_QuestState qs in questBook.quests)
        {
            if (qs.activeState == E_QuestActiveState.finished)
            {
                cnt++;
            }
        }
        return cnt;
    }

    /// <summary>
    /// Reset the quest book. Afterwards all quests are not fullfilled and the active quest list is empty.
    /// </summary>
    public void ResetQuestbook()
    {
        questBook.nrOfActiveQuests = 0;
        questBook.nrOfFinishedQuests = 0;
        questBook.nrOfQuests = 0;
        questBook.activeQuests = new List<C_QuestState>();
        foreach (C_QuestState questState in questBook.quests)
        {
            questState.Reset();
        }
        TryAutomaticRefill();
        questsChanged();
        save();
    }

    public bool IsQuestInQuestbook(QuestDefinition questDefinition)
    {
        foreach (C_QuestState qs in questBook.quests)
        {
            if (qs.questKey == questDefinition.name)
            {
                return true;
            }
        }
        return false;
    }

    public void fullfill_quest_if_active(QuestDefinition questDefinition)
    {
        if (questDefinition == null)
        {
            //Quest definition doesn't exist.
            return;
        }

        // Debug.Log("Try to fullfill " + questDefinition.name);

        //faster version, only accessing active quests from additional List
        foreach (C_QuestState qs in questBook.activeQuests)
        {
            if (qs.quest == questDefinition)
            {
                //Debug.Log("Fullfill " + questDefinition.name);

                qs.activeState = E_QuestActiveState.finished;
                GenerateQuestEvent(qs.quest, E_QuestEventType.finished);
                qs.quest.fullfillmentResult.ExecuteEffect();
                questBook.activeQuests.Remove(qs);                      //remove quest from temporary activeQuest list
                createQuestFinishedPopup(qs);
                questsChanged();
                save();
                break;
            }
        }
        TryAutomaticRefill();
    }

    void createQuestFinishedPopup(C_QuestState questState)
    {
        if (questFullfillmentPopup.popupParent != null && questFullfillmentPopup.popupPrefab != null)
        {
            GameObject popup = (GameObject)Instantiate(questFullfillmentPopup.popupPrefab);
            popup.transform.SetParent(questFullfillmentPopup.popupParent.transform,false);
            Quest_UIItem questUiItem = popup.GetComponent<Quest_UIItem>();
            if (questUiItem != null)
            {
                questUiItem.SetQuestState(questState);
            }
            else
            {
                Debug.LogWarning("The popup for the fullfillment of quest '" + questState.questKey + "' couldn't be displayed because the script 'Quest_UIItem' is not attached in the popup prefab.");
            }
        }
    }

    void checkIfActiveQuestIsFullfilledByCondition()
    {
        for (int i = questBook.activeQuests.Count - 1; i >= 0; i--) //walk backward to avoid itterator problems with element removal
        {
            //... if nothing is null and there are conditions present
            if (questBook.activeQuests[i] != null &&
                questBook.activeQuests[i].quest != null &&
                questBook.activeQuests[i].quest.fullfilmentConditions != null &&
                questBook.activeQuests[i].quest.fullfilmentConditions.conditions != null &&
                questBook.activeQuests[i].quest.fullfilmentConditions.conditions.Length > 0)
            {
                if (questBook.activeQuests[i].quest.fullfilmentConditions.AreConditionsMet())
                {
                    questBook.activeQuests[i].activeState = E_QuestActiveState.finished;
                    GenerateQuestEvent(questBook.activeQuests[i].quest, E_QuestEventType.finished);
                    questBook.activeQuests[i].quest.fullfillmentResult.ExecuteEffect();
                    createQuestFinishedPopup(questBook.activeQuests[i]);
                    questBook.activeQuests.Remove(questBook.activeQuests[i]);
                    questsChanged();
                    save();
                }
            }
        }
        TryAutomaticRefill();
    }

    /// <summary>
    /// Make as many quests active as possible, depending on maxNrOfActiveQuests.
    /// Aborts if no more aquests are possible.
    /// </summary>
    public void FillActiveQuests()
    {
        while (GetNrOfActiveQuests() < maxNrOfActiveQuests)
        {
            if (SetRandomQuestActiveGetCandidateCount() <= 0)
            {
                //no more quests can be added, the pool is empty
                return;
            }
        }
    }

    /// <summary>
    /// Discard actual active quests and refill active quests.
    /// </summary>
    public void ReselectActiveQuests()
    {
        AbortActiveQuests();
        FillActiveQuests();
    }

    /// <summary>
    /// Abort all active quests. 
    /// </summary>
    public void AbortActiveQuests()
    {
        for (int i = questBook.activeQuests.Count - 1; i >= 0; i--) //walk backward to avoid itterator problems with element removal
        {
            questBook.activeQuests[i].activeState = E_QuestActiveState.none;
            if (questBook.activeQuests[i].quest != null)
            {
                GenerateQuestEvent(questBook.activeQuests[i].quest, E_QuestEventType.aborted);
            }
            questBook.activeQuests.Remove(questBook.activeQuests[i]);
        }
        TryAutomaticRefill();
        questsChanged();
        save();
    }

    /// <summary>
    /// Fullfill/Finish all active quests. 
    /// </summary>
    public void FullfillActiveQuests()
    {
        for (int i = questBook.activeQuests.Count - 1; i >= 0; i--) //walk backward to avoid itterator problems with element removal
        {
            questBook.activeQuests[i].activeState = E_QuestActiveState.finished;
            if (questBook.activeQuests[i].quest != null)
            {
                GenerateQuestEvent(questBook.activeQuests[i].quest, E_QuestEventType.finished);
                questBook.activeQuests[i].quest.fullfillmentResult.ExecuteEffect();
                createQuestFinishedPopup(questBook.activeQuests[i]);
            }
            questBook.activeQuests.Remove(questBook.activeQuests[i]);
        }
        TryAutomaticRefill();
        questsChanged();
        save();
    }

    /// <summary>
    /// Set randomly one of the possible quest from the pool active.
    /// </summary>
    public void SetRandomQuestActive()
    {
        SetRandomQuestActiveGetCandidateCount();
    }

    public int SetRandomQuestActiveGetCandidateCount()
    {

        if (GetNrOfActiveQuests() >= maxNrOfActiveQuests)
        {
            //number of active quest is full
            return 0;
        }

        //1. get candidates
        List<C_QuestState> candidates = new List<C_QuestState>();
        foreach (C_QuestState qs in questBook.quests)
        {
            if (qs.loadedFromCatalog == true)
            {

                switch (qs.quest.questRepeatType)
                {
                    case QuestDefinition.E_QuestRepeatType.onlyOnce:
                        if (qs.activeState == E_QuestActiveState.none)
                        {
                            if (qs.quest.activatabilityConditions.AreConditionsMet())
                            {
                                candidates.Add(qs);
                            }
                        }
                        break;
                    case QuestDefinition.E_QuestRepeatType.repeatable:
                        //if a quest is repeadable it can only be selected if it is not active (either state 'none' or 'finished')
                        if (qs.activeState != E_QuestActiveState.active)
                        {
                            if (qs.quest.activatabilityConditions.AreConditionsMet())
                            {
                                candidates.Add(qs);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        if (candidates.Count < 1)
        {
            //adding not possible, no more quest candidates
            return 0;
        }

        //2. set random element from candidates to active
        int rnd = Random.Range(0, candidates.Count);
        candidates[rnd].activeState = E_QuestActiveState.active;

        questBook.activeQuests.Add(candidates[rnd]);

        questsChanged();
        save();

        return (candidates.Count - 1);
    }

    void actualizeQuestBookInfo()
    {
        questBook.nrOfActiveQuests = GetNrOfActiveQuests();
        questBook.nrOfQuests = GetNrOfAvailableQuests();
        questBook.nrOfFinishedQuests = GetNrOfFinishedQuests();
    }

    IEnumerator delayedActualize()
    {
        yield return null;
        TryAutomaticRefill();
        questsChanged(false);
    }

    //definition of event types for specific quests
    [System.Serializable]
    public enum E_QuestEventType
    {
        finished,       /* Event type if the quest is finished. */
        aborted         /* Event type it the is aborted. */
    }

    [System.Serializable]
    public class C_QuestEvent
    {
        public E_QuestEventType eventType = E_QuestEventType.finished;
        public QuestDefinition quest;
        public mEvent OnEvent;
    }
    //user definable list of quest events
    public List<C_QuestEvent> QuestEvents = new List<C_QuestEvent>();

    [System.Serializable]
    public enum E_QuestActiveState
    {
        none,       /*nothing is happening with this quest*/
        active,     /*the quest is currently active*/
        finished    /*the quest was finished*/
    }

    [System.Serializable]
    public class C_QuestState
    {
        [ReadOnlyInspector] public string questKey = "";            //key (name) of the quest
        [ReadOnlyInspector] public QuestDefinition quest;           //link to the actual quest (saving by serialization of this entry is not usable but it is correctly loaded by the key and the catalog)
        [ReadOnlyInspector] public E_QuestActiveState activeState = E_QuestActiveState.none;
        [ReadOnlyInspector] public bool loadedFromCatalog = true;   //Memorize if the quest key was found in the catalog. If not: Quest was removed from the catalog and shall not be displayed anymore (but is still saved).

        public void Reset()
        {
            activeState = E_QuestActiveState.none;
        }
    }

    //catalog for all possible quests (loaded from resources-folder)
    [ReadOnlyInspector] public List<QuestDefinition> catalog = new List<QuestDefinition>();

    //embed the quest data to an class to allow correct serialization
    [System.Serializable]
    public class C_Quests
    {
        [ReadOnlyInspector] public int nrOfQuests = 0;
        [ReadOnlyInspector] public int nrOfActiveQuests = 0;
        [ReadOnlyInspector] public int nrOfFinishedQuests = 0;
        [ReadOnlyInspector] public List<C_QuestState> quests = new List<C_QuestState>();          //states for all quests
        [ReadOnlyInspector] public List<C_QuestState> activeQuests = new List<C_QuestState>();    //Redundant memory for active quests. To avoid checking all quests for updates/changes it is faster to check only the active ones.
    }

    [ReadOnlyInspector] public C_Quests questBook;     //contains data for all quests

    //Return an quest state distinguished by its string key.
    public C_QuestState GetQuestStateByKey(string key)
    {

        foreach (C_QuestState qs in questBook.quests)
        {
            if (qs.questKey == key)
            {
                return qs;
            }
        }

        return null;
    }

    public QuestDefinition GetQuestFromCatalogByKey(string key)
    {
        foreach (QuestDefinition questDefinition in catalog)
        {
            if (questDefinition != null && questDefinition.name == key)
            {
                return questDefinition;
            }
        }
        mDebug("The quest with the key '" + key + "' could not be found within the quest catalog.");
        return null;
    }

    /// <summary>
    /// Delete all quests from the questbook.
    /// This is absolute, the quests can't be restored.
    /// </summary>
    public void DeleteQuestbook()
    {
        questBook = new C_Quests();
        questsChanged();
        save();
    }

    //Internal method to generete quest event-type specific events.
    private void GenerateQuestEvent(QuestDefinition quest, E_QuestEventType requestedEventType)
    {
        int eventsFound = 0;
        foreach (C_QuestEvent qe in QuestEvents)
        {
            if (qe.quest != null)
            {
                if (qe.quest.name == quest.name)
                {
                    if (qe.eventType == requestedEventType)
                    {
                        qe.OnEvent.Invoke();
                        eventsFound++;
                    }
                }
            }
        }
        if (eventsFound != 1)
        {
            mDebug("There is a possibility for missing/duplicate events for the event '" + requestedEventType.ToString() + "' of quest '" + quest.name + "': There were " + eventsFound.ToString() + " events excuted.");
        }
    }

    //Internal method. This is called from various places if something in the quest book changes.
    private void questsChanged(bool fireUserEvents = true)
    {
        //Execute all requested callbacks (e.g. from the UI script to actualize the list), if not null;
        if (OnQuestbookChangeAction != null)
        {
            OnQuestbookChangeAction.Invoke();
        }

        if (fireUserEvents == true)
        {
            //Execute User Events
            OnQuestbookChange.Invoke();
        }

        actualizeQuestBookInfo();
    }

    void TryAutomaticRefill()
    {
        if (autoRefillActiveQuests == true)
        {
            if (GetNrOfActiveQuests() < maxNrOfActiveQuests)
            {
                FillActiveQuests();
            }
        }
    }

    //load the state of all quests from the player prefs
    private void load()
    {
        bool alteredOnLoad = false;

        //load quests states from PlayerPrefs
        string json = SecurePlayerPrefs.GetString(jsonKey);
        if (!string.IsNullOrEmpty(json))
        {
            JsonUtility.FromJsonOverwrite(json, questBook);
        }

        //search through the catalog and add quests if they are new
        foreach (QuestDefinition questDefinition in catalog)
        {
            if (!IsQuestInQuestbook(questDefinition))
            {
                C_QuestState questState = new C_QuestState();
                questState.quest = questDefinition;
                questState.questKey = questDefinition.name;
                questBook.quests.Add(questState);

                alteredOnLoad = true;
            }
        }

        //clear the temporary list for active quests
        questBook.activeQuests.Clear();

        //regenerate link to the scriptable object and search/add active quests to the temporary list
        foreach (C_QuestState qs in questBook.quests)
        {
            QuestDefinition quest = GetQuestFromCatalogByKey(qs.questKey);
            if (quest != null)
            {
                qs.quest = quest;
                qs.loadedFromCatalog = true;

                //ifthe quest is active, also add it to the active list
                if (qs.activeState == E_QuestActiveState.active)
                {
                    questBook.activeQuests.Add(qs);
                }
            }
            else
            {
                qs.loadedFromCatalog = false;
            }
        }

        loaded = true;

        if (alteredOnLoad == true)
        {
            save();
        }
    }

    //Load all possible quests from the "Resource" folder of the project and add it to a catalog.
    private void loadCatalog()
    {
        catalog.Clear();

        Object[] objects = Resources.LoadAll("", typeof(QuestDefinition));

        if (objects != null)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                catalog.Add((QuestDefinition)objects[i]);
            }
        }
    }


    private void save()
    {
        if (loaded)
        {
            string json = JsonUtility.ToJson(questBook);
            SecurePlayerPrefs.SetString(jsonKey, json);
        }
        else
        {
            Debug.LogError("Quest book can not be altered bevore it is loaded. Your change is lost.");
        }
    }

    //General event something in the quest system changes.
    public mEvent OnQuestbookChange;


    /// <summary>
    /// Return all possible translatable terms for the translation managment in Kings.
    /// </summary>
    /// <returns></returns>
    public override List<string> getTranslatableTerms()
    {
        List<string> terms = new List<string>();
        terms.Clear();

        foreach (QuestDefinition quest in catalog)
        {
            terms.Add(quest.questTitle);
            terms.Add(quest.description);
            terms.Add(quest.rewardText);
        }

        return terms;
    }
}

