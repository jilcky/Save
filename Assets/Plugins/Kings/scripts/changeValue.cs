using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Helper script: modify values by an event. Replaces addValue and setValue scripts.
 */

public class changeValue : MonoBehaviour
{

    [Tooltip("Define the value changes when calling 'ChangeValues()'")]
    public EventScript.resultModifier[] valuesToChange;
    //public Inventory_ChangeItem.itemModifier[] itemChanges;


    public void ChangeValues()
    {
        foreach (EventScript.resultModifier rm in valuesToChange)
        {
            EventScript.executeValueChange(rm);
        }
        /*foreach(Inventory_ChangeItem.itemModifier im in itemChanges)
        {
            Inventory_ChangeItem.executeItemChange(im);
        }*/
    }
}
