using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Helper script: modify values by an event.
 */
public class setValue : MonoBehaviour {

	[Tooltip("Define the value changes when calling 'addValues()'")]
	public EventScript.C_ReducedResultModifier[] valuesToChange;

    [System.Obsolete("'setValues()' is obsolete. Please use 'ChangeValues()' of the script 'changeValue.cs' instead. 'setValues()' will be removed in future releases.")]
    public void setValues()
    {
        Debug.LogWarning("("+gameObject.name+"):'setValues()' is obsolete. Please use 'ChangeValues()' of the script 'changeValue.cs' instead. 'setValues()' will be removed in future releases.");
        foreach (EventScript.C_ReducedResultModifier rrm in valuesToChange)
        {
            //Use the default handling of the result by changing it to a (extended) result modifier. Backward compatibility...
            EventScript.resultModifier rm = new EventScript.resultModifier();
            rm.modifier = rrm.modifier;
            rm.valueSet = rrm.valueAdd;
            rm.modificationType = EventScript.E_ModificationType.set;

            EventScript.executeValueChange(rm);
        }
    }
}
