using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HistoryEvent is a template for (time)events of the timeline.
/// Properties/parameters of these events are defined here.
/// </summary>

[CreateAssetMenu(fileName = "NewHistoryEvent", menuName = "New History Event", order = 51)]
public class HistoryEvent : ScriptableObject
{
    [Tooltip("The title of the history event that can also be displayed in the UI. Please use distinguishable names.")]
    public string title = "NewHistoryEvent";
    [Tooltip("An description of the history event. It can be displayed in the UI.")]
    public string description = "A new history event. There was no description added.";
    [Tooltip("Please provide an image for the history event which can be displayed in the UI.")]
    public Sprite image;
}
