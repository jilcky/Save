using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


/// <summary>
/// The quest ui list is responsible for spawning a list of ui elements of quests into the scene.
/// This scipt can also filter which quest states are spawned into the scene.
/// </summary>

public class Quest_UIList : MonoBehaviour
{
    [System.Serializable] public class mEvent : UnityEvent { }

    //Link to the main script.
    private Quests quests;

    [Tooltip("Provide a templates for the display of a quest within the UI.")]
    public GameObject UI_OpenTemplate;
    [Tooltip("Provide a templates for the display of a quest within the UI.")]
    public GameObject UI_ActiveTemplate;
    [Tooltip("Provide a templates for the display of a quest within the UI.")]
    public GameObject UI_CompletedTemplate;

    [System.Serializable]
    public enum E_QuestFilterType
    {
        showAll,
        showOpen,
        showActive,
        showCompleted
    }

    [System.Serializable]
    public class C_QuestNumberDisplay
    {
        public Text completedQuestsText;
        public Text overallQuestText;
        public Text combinedTexts;
    }
    public C_QuestNumberDisplay questNumberDisplay;

    [Tooltip("Add filters for displaying elements. Filters are applied one by one and all results of each filter are added to the list.")]
    public E_QuestFilterType[] questDisplayFilters;

    bool callbackSet = false;
    private void Start()
    {
        //if not configurated how to filter: add a default filter which shows all
        if (questDisplayFilters == null || questDisplayFilters.Length == 0)
        {
            questDisplayFilters = new E_QuestFilterType[1];
            questDisplayFilters[0] = E_QuestFilterType.showAll;
        }

        quests = Quests.instance;
        quests.OnQuestbookChangeAction += OnQuestboockChanged;   //Callback to get informed if something changes.
        callbackSet = true;

        OnQuestboockChanged();
    }

    //List of items to display in this questbook display.
    [HideInInspector] public List<GameObject> listElements;

    void OnQuestboockChanged()
    {
        ActualizeUIList();
    }

    //Destroy the callback for actualization. This is needed for quests shown at a card, the callback would be invalid after destroying the card.
    private void OnDestroy()
    {
        if (callbackSet == true)
        {
            quests.OnQuestbookChangeAction -= OnQuestboockChanged;
        }
    }

    //Actualization of the questbook for the UI
    public void ActualizeUIList()
    {
        //1. clear
        foreach(GameObject go in listElements)
        {
            Destroy(go);
        }

        //2. add
        //2a. foreach filter (to get a defined order)
        //2b. test for each quest State
        //2c. filter and spawn according to preferences
        foreach (E_QuestFilterType filter in questDisplayFilters)
        {
            foreach (Quests.C_QuestState questState in quests.questBook.quests)
            {
                if (questState.loadedFromCatalog == true)
                {

                    switch (filter)
                    {
                        case E_QuestFilterType.showOpen:
                            if (questState.activeState == Quests.E_QuestActiveState.none)
                            {
                                listElements.Add(CreateQuestDisplay(questState));
                            }
                            break;
                        case E_QuestFilterType.showActive:
                            if (questState.activeState == Quests.E_QuestActiveState.active)
                            {
                                listElements.Add(CreateQuestDisplay(questState));
                            }
                            break;
                        case E_QuestFilterType.showCompleted:
                            if (questState.activeState == Quests.E_QuestActiveState.finished)
                            {
                                listElements.Add(CreateQuestDisplay(questState));
                            }
                            break;
                        case E_QuestFilterType.showAll:
                            listElements.Add(CreateQuestDisplay(questState));
                            break;
                    }
                }
            }
        }

        actualizeQuestNumberDisplay();
    }

    void actualizeQuestNumberDisplay()
    {
        if (quests != null) {

            if (questNumberDisplay.completedQuestsText != null)
            {
                questNumberDisplay.completedQuestsText.text = quests.questBook.nrOfFinishedQuests.ToString();
            }

            if (questNumberDisplay.overallQuestText != null)
            {
                questNumberDisplay.overallQuestText.text = quests.questBook.nrOfQuests.ToString();
            }

            if (questNumberDisplay.combinedTexts != null)
            {
                questNumberDisplay.combinedTexts.text = quests.questBook.nrOfFinishedQuests.ToString() + " / " + quests.questBook.nrOfQuests.ToString();
            }
        }
    }

    //create a single ui element for one quest
    private GameObject CreateQuestDisplay(Quests.C_QuestState questState)
    {
        GameObject go = null;
        switch (questState.activeState)
        {
            case Quests.E_QuestActiveState.none:
                if(UI_OpenTemplate == null)
                {
                    Debug.LogError("The parameter 'UI_OpenTemplate' for the script Quest_UIList was not configured at " + gameObject.name + ". Element will not be displayed.");
                    return null;
                }
                go = (GameObject)Instantiate(UI_OpenTemplate);
                break;
            case Quests.E_QuestActiveState.active:
                if (UI_ActiveTemplate == null)
                {
                    Debug.LogError("The parameter 'UI_ActiveTemplate' for the script Quest_UIList was not configured at " + gameObject.name + ". Element will not be displayed.");
                    return null;
                }
                go = (GameObject)Instantiate(UI_ActiveTemplate);
                break;
            case Quests.E_QuestActiveState.finished:
                if (UI_CompletedTemplate == null)
                {
                    Debug.LogError("The parameter 'UI_CompletedTemplate' for the script Quest_UIList was not configured at " + gameObject.name + ". Element will not be displayed.");
                    return null;
                }
                go = (GameObject)Instantiate(UI_CompletedTemplate);
                break;
        }

        if(go == null)
        {
            Debug.LogError("The element for displaying a quest couldn't be created.");
            return null;
        }

        go.transform.SetParent(gameObject.transform,false);
        Quest_UIItem uiItem = go.GetComponent<Quest_UIItem>();
        if (uiItem == null)
        {
            Debug.LogWarning("The Prefab for quest UI-items should have a 'Quest_UIItem' script. This script was not found. Skipping display.");
            return null;
        }
        else
        {
            uiItem.SetQuestState(questState);
            listElements.Add(go);
            return go;
        }
    }
}
