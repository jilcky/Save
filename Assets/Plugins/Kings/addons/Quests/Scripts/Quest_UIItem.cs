using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for linking the properties of quests to the user interface.
/// </summary>

public class Quest_UIItem : MonoBehaviour
{
    [ReadOnlyInspector] public Quests.C_QuestState _questState;

    [Tooltip("Please specify a text element to show the title of this element.")]
    public Text questTitle;
    [Tooltip("Please specify an image element to show the sprite of this element.")]
    public Image questImage;
    [Tooltip("Please specify a text element to show the description of this element.")]
    public Text descriptionText;
    [Tooltip("Please specify a toggle element which shows the fullfillment.")]
    public Toggle fullfillmentToggle;
    [Tooltip("Text for the reward description.")]
    public Text rewardText;
    [Tooltip("Image for the reward description.")]
    public Image rewardImage;

    //Set a quest state
    public void SetQuestState(Quests.C_QuestState questState)
    {
        _questState = questState;

        if (_questState != null)
        {
            if (questImage != null)
            {
                questImage.sprite = _questState.quest.image;
            }
            if (questTitle != null)
            {
                questTitle.text = TranslationManager.translateIfAvail(_questState.quest.questTitle);
            }
            if (descriptionText != null)
            {
                descriptionText.text = TranslationManager.translateIfAvail(_questState.quest.description);
            }
            if(fullfillmentToggle != null)
            {
                bool isFinished = false;
                if(_questState.activeState == Quests.E_QuestActiveState.finished)
                {
                    isFinished = true;
                }
                fullfillmentToggle.isOn = isFinished;
            }
            if(rewardImage != null)
            {
                rewardImage.sprite = questState.quest.rewardImage;
            }
            if(rewardText != null)
            {
                rewardText.text = TranslationManager.translateIfAvail(_questState.quest.rewardText);
            }
        }
    }

}
