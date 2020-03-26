using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Helper script: modify values by an event.
 */

public class addValue : MonoBehaviour
{


    [Tooltip("Define the value changes when calling 'addValues()'")]
    public EventScript.resultModifier[] valuesToChange;

    [System.Obsolete("'addValues()' is obsolete. Please use 'ChangeValues()' of the script 'changeValue.cs' instead. 'addValues()' will be removed in future releases.")]
    public void addValues()
    {
        Debug.LogWarning("(" + gameObject.name + "):'addValues()' is obsolete. Please use 'ChangeValues()' of the script 'changeValue.cs' instead. 'addValues()' will be removed in future releases.");
        foreach (EventScript.resultModifier rm in valuesToChange)
        {
            //For each other case use the standard handling of the result.
            EventScript.executeValueChange(rm);
        }
    }
}
