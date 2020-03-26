using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Main Script for the timeline.
/// </summary>
public class Timeline : TranslatableContent
{
    [System.Serializable] public class mEvent : UnityEvent { }

    [Tooltip("Enable/disable extended debugging output.")]
    public bool verboseDebug = false;
    void mDebug(string txt)
    {
        if (verboseDebug == true)
        {
            Debug.Log(txt);
        }
    }

    public static Timeline instance;          //Hmm.. another singleton. This manager can only be used once per scene.
    string jsonKey = "Timeline";              //Save key for the Playerprefs.
    public System.Action OnTimelineChangeAction;   //Callback possibility for other scripts to get informed if something changes in the Timeline.
    public System.Action OnTimelineCleared;
    bool loaded = false;

    public valueDefinitions.values timeValue;
    public int startYear = 1500;

    private void Awake()
    {
        instance = this;
        loadCatalog();
        load();
        StartCoroutine(delayedActualize());
    }

    private void Start()
    {
        TranslationManager.instance.registerTranslateableContentScript(this);
    }

    [ReadOnlyInspector] public int maxYear = 0;
    ValueScript vs = null;

    IEnumerator delayedActualize()
    {
        yield return null;
        vs = valueManager.instance.getFirstFittingValue(timeValue);
        timelineChanged();
    }

    private void Update()
    {
        if (vs != null)
        {
            if (Mathf.RoundToInt(vs.value) > maxYear)
            {
                maxYear = Mathf.RoundToInt(vs.value);
                timelineChanged();
            }
        }
    }

    public int getMaxYear()
    {
        if (vs != null)
        {
            if (Mathf.RoundToInt(vs.value) > maxYear)
            {
                maxYear = Mathf.RoundToInt(vs.value);
                timelineChanged();
            }
        }
        return maxYear;
    }

    //storage class for timeline data for saving the timeline
    [System.Serializable]
    public class C_TimelineData
    {
        [ReadOnlyInspector] public int year = 0;
        [Tooltip("Translation can't be applied here, because text-replacement is already finished. ")]
        [ReadOnlyInspector] public string addText = "";
        [ReadOnlyInspector] public string timelineKey = "";             //key (name) of the event
        [ReadOnlyInspector] public HistoryEvent historyEvent;              //link to the actual event (saving by serialization of this entry is not usable but it is correctly loaded by the key and the catalog)
        [ReadOnlyInspector] public bool loadedFromCatalog = false;   //Memorize if the event key was found in the catalog. If not: Event was removed from the catalog and shall not be displayed anymore (but is still saved).
    }

    //catalog for all possible events (loaded from resources-folder)
    [ReadOnlyInspector] public List<HistoryEvent> catalog = new List<HistoryEvent>();

    //embed the event entry data to an class to allow correct serialization
    //The history consists of multible events of the past. Sounds good for me.
    [System.Serializable]
    public class C_History
    {
        public List<C_TimelineData> events;
        public int timelineEnd;
        [ReadOnlyInspector] public int startYearOffset = 0;
    }

    //history with the relevant events only
    [ReadOnlyInspector] public C_History history;

    //Processed history for displaying in the UI. Can contain fillers for years without event. The Index refers to the year for faster access.
    [ReadOnlyInspector] public List<C_TimelineData> displayedHistory = new List<C_TimelineData>();

    public HistoryEvent GetEventFromCatalogByKey(string key)
    {
        foreach (HistoryEvent historyEvent in catalog)
        {
            if (historyEvent != null && historyEvent.name == key)
            {
                return historyEvent;
            }
        }
        mDebug("The event with the key '" + key + "' could not be found within the event catalog.");
        return null;
    }

    /// <summary>
    /// Delete all events from the history.
    /// This is absolute, the events can't be restored.
    /// </summary>
    public void ResetHistoryData()
    {
        history = new C_History();
        history.events = new List<C_TimelineData>();
        maxYear = 0;
        history.timelineEnd = 0;
        displayedHistory.Clear();

        timelineChanged();

        if (OnTimelineCleared != null)
        {
            OnTimelineCleared.Invoke();
        }

        save();
    }

    /// <summary>
    /// Create a new history appending to the actual one.
    /// The actual History itself will be deleted, the end year is memorized as an offset to the actual history.
    /// </summary>
    public void CreateNewFollowingHistory()
    {
        int startYearOffset = history.startYearOffset + history.timelineEnd;
        //Debug.Log("Start year offset is " + startYearOffset.ToString());

        ResetHistoryData();

        history.startYearOffset = startYearOffset;

        save();
    }


    /// <summary>
    /// Add a history event.
    /// </summary>
    /// <param name="historyEvent"></param>
    public void AddHistoryEvent(HistoryEvent historyEvent, string additionalText = "")
    {
        if (historyEvent == null)
        {
            Debug.LogWarning("History event could not be added because it is 'null'. Missing a proper configuration at a value script or an result of one of the scriptable objects (item, quest, etc.)?");
            return;
        }


        mDebug("Add " + historyEvent.ToString());

        //create new timeline data from the history event
        C_TimelineData timelineData = new C_TimelineData();

        if (vs != null)
        {
            timelineData.year = Mathf.RoundToInt(vs.value);
            timelineData.historyEvent = historyEvent;
            timelineData.timelineKey = historyEvent.name;
            timelineData.addText = TextReplacement.TranslateAndReplace(additionalText);
            history.events.Add(timelineData);

            expandDisplayedHistory(timelineData);

            timelineChanged();

            save();
        }
        else
        {
            Debug.Log("Could not generate history event because the time value '" + timeValue.ToString() + "' is not available.");
        }
    }
    public void AddHistoryEvent(C_TimelLineEventChange historyEvent)
    {
        AddHistoryEvent(historyEvent.newHistoryEvent, historyEvent.additionalHistoryText);
    }

    /// <summary>
    /// Return the number of events in the history of an specific type.
    /// </summary>
    /// <param name="historyEvent"></param>
    /// <returns></returns>
    public int GetHistoryEventsCount(HistoryEvent historyEvent)
    {
        int i = 0;
        foreach (C_TimelineData td in history.events)
        {
            if (td.timelineKey == historyEvent.name)
                i++;
        }
        return i;
    }
    /// <summary>
    /// Return the number of all events in the history.
    /// </summary>
    /// <returns></returns>
    public int GetHistoryEventsCount()
    {
        return history.events.Count;
    }

    //Internal method. This is called from various places if something in the timeline changes.
    private void timelineChanged()
    {

        history.timelineEnd = getMaxYear();

        //actualized displayedHistory - List
        expandDisplayedHistory(null);

        //Execute all requested callbacks (e.g. from the UI script to actualize the list), if not null;
        if (OnTimelineChangeAction != null)
        {
            OnTimelineChangeAction.Invoke();
        }

        //Execute User Events
        OnHistoryChanged.Invoke();
    }

    public C_TimelineData GetEventOfYear(int year)
    {
        if (year < displayedHistory.Count)
        {
            return displayedHistory[year];
        }
        //no event for this specific year? return null.
        return null;
    }


    private void load()
    {
        string json = SecurePlayerPrefs.GetString(jsonKey);
        if (!string.IsNullOrEmpty(json))
        {
            JsonUtility.FromJsonOverwrite(json, history);
        }

        //Json loads some IDs to reference to scriptable object. Because of mismatching IDs to the actual IDs this resulted in missing/empty scriptable objects.
        //To bypass this, the name of the scriptableObject is used as a key and the corresponding object is inserted from the catalog.

        foreach (C_TimelineData timelineData in history.events)
        {
            HistoryEvent historyEvent = GetEventFromCatalogByKey(timelineData.timelineKey);
            if (historyEvent != null)
            {
                timelineData.historyEvent = historyEvent;
                timelineData.loadedFromCatalog = true;
            }
            else
            {
                timelineData.loadedFromCatalog = false;
            }
        }

        createDisplayedHistory();

        loaded = true;
    }

    //compute the displayed History from the actual history events, but sorted and with fillers. Without pre and post elements.
    void createDisplayedHistory()
    {
        displayedHistory.Clear();
        displayedHistory = new List<C_TimelineData>();

        //create a list which can hold an entry for each year.
        while (displayedHistory.Count <= history.timelineEnd)
        {
            displayedHistory.Add(new C_TimelineData());
        }

        //sort the actual Events directly into the list
        foreach (C_TimelineData td in history.events)
        {
            if (td.loadedFromCatalog == true && td.historyEvent != null)
            {
                if (td.year < displayedHistory.Count)
                {
                    displayedHistory[td.year] = td;
                }
            }
        }
    }

    void expandDisplayedHistory(C_TimelineData timelineData)
    {

        while (displayedHistory.Count <= (history.timelineEnd))
        {
            displayedHistory.Add(new C_TimelineData());
        }

        if (timelineData != null)
        {

            while (displayedHistory.Count <= (timelineData.year))
            {
                displayedHistory.Add(new C_TimelineData());
            }
            //overwrite with new event (if not null)-> last event counts
            if (timelineData.historyEvent != null)
            {
                displayedHistory[timelineData.year] = timelineData;
            }
        }
    }

    //Load all possible events from the "Resource" folder of the project and add it to a catalog.
    private void loadCatalog()
    {
        catalog.Clear();

        Object[] objects = Resources.LoadAll("", typeof(HistoryEvent));

        if (objects != null)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                catalog.Add((HistoryEvent)objects[i]);
            }
        }
    }


    private void save()
    {
        if (loaded)
        {
            string json = JsonUtility.ToJson(history);
            SecurePlayerPrefs.SetString(jsonKey, json);
        }
        else
        {
            Debug.LogError("Timeline can not be altered bevore it is loaded. Your change is lost.");
        }
    }

    //General event something in the history changes 
    public mEvent OnHistoryChanged;

    //Return all possible translatable terms for the translation managment in Kings.
    public override List<string> getTranslatableTerms()
    {
        List<string> terms = new List<string>();
        terms.Clear();

        foreach (HistoryEvent historyEvent in catalog)
        {
            terms.Add(historyEvent.title);
            terms.Add(historyEvent.description);
        }

        return terms;
    }

    [System.Serializable]
    public class C_TimelLineEventChange
    {
        public HistoryEvent newHistoryEvent;
        public string additionalHistoryText = "";


        public List<string> GetTranslatableContent()
        {
            List<string> tContent = new List<string>();
            tContent.Add(additionalHistoryText);
            return tContent;
        }
    }
}
