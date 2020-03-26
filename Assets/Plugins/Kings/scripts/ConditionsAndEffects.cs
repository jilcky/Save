using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Conditions and effects (former Results) are not used solely by the value script anymore.
/// To allow for use of conditions and effects this script allwos for a similar mechanism for all scripts.
/// It still references to the elements of the eventScript but without some of the overhead (e.g. FollowUpCard which is removed here intentionally)
/// The original parts from the eventScript are not removed for backward compatibility.
/// </summary>


public class ConditionsAndEffects : MonoBehaviour
{
    //mapping of the original classes
    [System.Serializable]
    public class C_Conditions
    {
        public EventScript.condition[] conditions;
        public bool AreConditionsMet()
        {
            return ConditionsAndEffects.AreConditionsMet(this);
        }
    }

    [System.Serializable]
    public class C_Changes
    {
        public EventScript.result result;
        public void ExecuteEffect()
        {
            ConditionsAndEffects.ComputeEffects(this);
        }
        public List<string> GetTranslatableContent()
        {
            return result.GetTranslatableContent();
        }
    }

    /// <summary>
    /// Computation logic for executing a result/effect. Refers to (now) static method of Eventscript.
    /// Depending on the configuration of the card the corresponding results are selected.    
    /// </summary>
    /// <param name="result"></param>
    public static void ComputeEffects(C_Changes result)
    {
        EventScript.ComputeResultTypeDependant(result.result,false);
    }

    /// <summary>
    ///     Test if conditions are met.
    /// </summary>
    public static bool AreConditionsMet(C_Conditions conditions)
    {
        return EventScript.AreConditinsForResultMet(conditions.conditions);
    }

}
