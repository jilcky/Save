using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultCheat : MonoBehaviour
{
    public ConditionsAndEffects.C_Changes changes;
       
    public void addCheats()
    {
        changes.ExecuteEffect();
    }
}
