using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// QuestDefinition is a template for quests.
/// Properties/parameters of quests are defined here.
/// </summary>

[CreateAssetMenu(fileName = "NewQuest", menuName = "New Quest Definition", order = 51)]
public class QuestDefinition : ScriptableObject
{
    //Enumeration of the repeatability for an quest.
    public enum E_QuestRepeatType
    {
        onlyOnce,
        repeatable
    }

    [Tooltip("Define repetition type of quests. With the selection 'repeatable' the quest can be selected more than once.")]
    public E_QuestRepeatType questRepeatType = E_QuestRepeatType.onlyOnce;
    [Tooltip("Conditions for enabling a quest. These conditions are evaluated when new quests are added.")]
    public ConditionsAndEffects.C_Conditions activatabilityConditions;
    [Tooltip("The name of the quest that can also be displayed in the UI. Please use distinguishable names for quests")]
    public string questTitle = "NewQuest";
    [Tooltip("An description of the quest. It can be displayed in the UI.")]
    public string description = "A new quest. There was no description added.";
    [Tooltip("Add a description text for the reward to show the user in which way he will benefit from fullfilling this quest.")]
    public string rewardText = "";
    [Tooltip("Add an image for the reward to show the user in which way he will benefit from fullfilling this quest.")]
    public Sprite rewardImage = null;

    [Tooltip("Please provide an image for the quest which can be displayed in the UI.")]
    public Sprite image;

    [Tooltip("Conditions for automatically fullfill the quest. The condition is evaluated if an card is despawned. It is only checked, if at least one condition is configured. It can also be fullfilled by a trigger.")]
    public ConditionsAndEffects.C_Conditions fullfilmentConditions;


    [System.Serializable]
    public class C_QuestResults
    {
        public EventScript.resultModifier[] valueChanges;
        public Inventory_ChangeItem.itemModifier[] itemChanges;
    }

    public ConditionsAndEffects.C_Changes fullfillmentResult;
}
