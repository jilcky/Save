using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for linking the properties of an history event to the user interface.
/// </summary>

public class Timeline_UIItem : MonoBehaviour
{
    [ReadOnlyInspector] public Timeline.C_TimelineData _timelineData;

    [Tooltip("Please specify a text element to show the year of this element.")]
    public Text eventYear;
    [Tooltip("Please specify an image element to show the sprite of this element.")]
    public Image eventImage;
    [Tooltip("Please specify a text element to show the history event title.")]
    public Text eventTitle;
    [Tooltip("Please specify a text element to show the history event description.")]
    public Text descriptionText;
    [Tooltip("Please specify a text element to show additional texts from the event. They are translated only on generation and can not be altered later!")]
    public Text addText;
    [Tooltip("Combined text field for description text and additional text. Texts are separated by an space character.")]
    public Text combinedDescriptionText;
    //Set an history event and its year form an TimelineData class (contains more information about the event, e.g. year)
    public void SetHistoryEvent(Timeline.C_TimelineData timelineData)
    {
        _timelineData = timelineData;

        if (_timelineData != null && _timelineData.historyEvent != null)
        {
            if (eventImage != null)
            {
                eventImage.sprite = _timelineData.historyEvent.image;
            }
            if (eventTitle != null)
            {
                eventTitle.text = TranslationManager.translateIfAvail(_timelineData.historyEvent.title);
            }
            if (eventYear != null)
            {
                if (Timeline.instance != null) {
                    eventYear.text = (_timelineData.year + Timeline.instance.startYear + Timeline.instance.history.startYearOffset - 1).ToString();
                }
                else
                {
                    //if acessing timeline instance fails, show the year from the element only
                    eventYear.text = _timelineData.year.ToString();
                }
            }
            if (descriptionText != null)
            {
                descriptionText.text = TranslationManager.translateIfAvail(_timelineData.historyEvent.description);
            }
            if(addText != null)
            {
                //translation can't be applied here, because text-replacement is already finished. 
                addText.text = _timelineData.addText;
            }
            if (combinedDescriptionText != null)
            {
                combinedDescriptionText.text = TranslationManager.translateIfAvail(_timelineData.historyEvent.description) + " " + _timelineData.addText;
            }
        }
    }
}
