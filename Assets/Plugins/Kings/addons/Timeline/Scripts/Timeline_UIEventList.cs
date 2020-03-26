using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// The timeline ui (history)event list is responsible for spawning a list of ui elements into the scene.
/// </summary>
public class Timeline_UIEventList : MonoBehaviour
{
    [System.Serializable] public class mEvent : UnityEvent { }

    //Link to the main script of the timeline.
    private Timeline timeline;

    [Tooltip("Provide a template for the display of an history event within the UI.")]
    public GameObject UITemplate;
    [Tooltip("To display long timelines a scrollbar is needed. The value of the bar will be altered by this script to show a movement effekt if something is added or it gets animated.")]
    public Scrollbar historyScrollbar;
    [Tooltip("Please provide a textfield where the acutal year is shown. Only works with filler elements like in the example.")]
    public Text yearsText;
    [Tooltip("Enable/disable the automatic scrollbar movement when new elements are added.")]
    public bool autoScrollbarMove = true;
    [Tooltip("Define the speed of the scrollbar when new elements are added (autoScrollbarMove has to be enabled).")]
    public float scrollbarSpeed = 1f;

    [Tooltip("For the animation effect of the timeline: specify the time it stays on the first element bevore scrolling through the timeline.")]
    public float scrollbarAnimationDelay = 2f;
    [Tooltip("For the animation effect of the timeline: specify the speed for scrolling through the timeline.")]
    public float scrollbarAnimationSpeed = 2f;
    [Tooltip("Define if display fillers for years without a historical event are used.")]
    public bool displayFillerEvents = true;
    [Tooltip("Please define how many post fillers for formatting are in the timeline. The information is needed to correctly spawn new events between filler elements.s")]
    public int nrOfPostFillers = 4;

    public mEvent OnAnimationStart;
    public mEvent OnAnimationEnd;

    private List<GameObject> preEntries = new List<GameObject>();
    private List<GameObject> postEntries = new List<GameObject>();


    private void Start()
    {
        timeline = Timeline.instance;
        timeline.OnTimelineChangeAction += OnTimelinechanged;   //Callback to get informed if something in the timeline changes.
        timeline.OnTimelineCleared += OnTimelineCleared;
        OnTimelinechanged();
        StartCoroutine(freezableUpdate());
    }

    //List of items to display in this timeline.
    [HideInInspector] public List<GameObject> listElements;

    void OnTimelinechanged()
    {
        ActualizeUIList();
        startMoveScrollbar();
    }

    void OnTimelineCleared()
    {
        foreach (GameObject go in listElements)
        {
            Destroy(go);
        }
        timelineDictionary = new Dictionary<int, GameObject>();
        lastActualizedYear = 0;
    }

    Coroutine move = null;
    void startMoveScrollbar()
    {
        if (move != null)
        {
            StopCoroutine(move);
        }
        move = StartCoroutine(moveScrollbar());
    }

    void actualizeYearsTextAndFakeScrollbar()
    {
        if (yearsText != null)
        {
            //years update only works if fillers are used
            if (displayFillerEvents == true)
            {
                //get the element in the middle of the list
                int elementIndex = (int)(historyScrollbar.value * timelineDictionary.Count + 0.5f);
                yearsText.text = (elementIndex + timeline.startYear + timeline.history.startYearOffset - 1).ToString();
            }
        }
    }

    private int lockFrames = 0;
    IEnumerator freezableUpdate()
    {
        while (true)
        {
            if (lockFrames <= 0)
            {
                actualizeYearsTextAndFakeScrollbar();
                yield return null;
            }
            else
            {
                //don't actualize (freeze for this frame)

                //decrement lock frames
                lockFrames--;
                yield return null;
            }
        }
    }

    IEnumerator moveScrollbar()
    {
        if (autoScrollbarMove == true && gameObject.activeInHierarchy == true)
        {
            yield return null;
            yield return null;
            while (historyScrollbar.value < 1f)
            {
                if (historyScrollbar.value < 0.9f)
                {
                    historyScrollbar.value = 0.9f;
                }

                if (timelineDictionary.Count > 0)
                {
                    historyScrollbar.value += scrollbarSpeed * Time.deltaTime / timelineDictionary.Count;
                }
                yield return null;
            }
        }
        move = null;
    }

    public void AnimateScrollbar()
    {
        if (move != null)
        {
            StopCoroutine(move);
        }
        StartCoroutine(animateScrollbar());
    }

    IEnumerator animateScrollbar()
    {
        if (gameObject.activeInHierarchy == true)
        {
            historyScrollbar.value = 0f;
            yield return null;
            historyScrollbar.value = 0f;
            yield return null;
            historyScrollbar.value = 0f;
            yield return null;

            yield return new WaitForSeconds(scrollbarAnimationDelay);

            OnAnimationStart.Invoke();

            while (historyScrollbar.value < 1f)
            {
                if (timelineDictionary.Count > 0)
                {
                    historyScrollbar.value += scrollbarAnimationSpeed * Time.deltaTime / timelineDictionary.Count;
                }
                yield return null;
            }
        }

        OnAnimationEnd.Invoke();
    }

    private int lastActualizedYear = 0;
    private Dictionary<int, GameObject> timelineDictionary = new Dictionary<int, GameObject>();

    //Actualization of the timeline for the UI
    public void ActualizeUIList()
    {
        while (timeline.maxYear >= lastActualizedYear)
        {
            if (!timelineDictionary.ContainsKey(lastActualizedYear))
            {
                GameObject go = CreateEvent(timeline.GetEventOfYear(lastActualizedYear));

                if (go != null)
                {
                    //correct the position of the element within the siblings (order of the the game objects in the displayed list)
                    int siblingIndex = go.transform.GetSiblingIndex();
                    //take post-elements into account
                    siblingIndex -= nrOfPostFillers;
                    go.transform.SetSiblingIndex(siblingIndex);

                    timelineDictionary.Add(lastActualizedYear, go);

                }
                //set a lock frame (years display freeze) to remove display glitches when the list is extended
                lockFrames = 2;
            }
            lastActualizedYear++;
        }
    }
    


    private GameObject CreateEvent(Timeline.C_TimelineData historyEvent)
    {
        //if there is no history event and there should be no filler: return.
        if(displayFillerEvents == false && historyEvent.historyEvent == null)
        {
            return null;
        }

        GameObject go = (GameObject)Instantiate(UITemplate);
        go.transform.SetParent(gameObject.transform,false);
        Timeline_UIItem uiItem = go.GetComponent<Timeline_UIItem>();
        if (uiItem == null)
        {
            Debug.LogWarning("The Prefab for timeline UI-items should have a 'Timeline_UIItem' script. This script was not found. Skipping display.");
            return null;
        }
        else
        {
            uiItem.SetHistoryEvent(historyEvent);
            listElements.Add(go);
            return go;
        }
    }
}
