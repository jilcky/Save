using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class valueDependentConditionalImages : MonoBehaviour
{

    [Tooltip("'valueDependentConditionalImages' supports two modes.\n" +
    "automatic: conditions are evaluatet all the time. \n" +
    "manual:    conditions are only evalutated if the function 'ExecuteConditionCheck()' is called. The images are changed accordingly.")]
    public E_EventExecutionType actualizationType = E_EventExecutionType.automatic;

    public EventScript.condition[] conditionsToTest;
    [System.Serializable] public class mEvent : UnityEvent { }



    public Image targetImage;
    public valueDefinitions.values modifier;

    public ValueScript.valueToIcon[] TrueSpriteSet;
    public ValueScript.valueToIcon[] FalseSpriteSet;

    [System.Serializable]
    public enum E_EventExecutionType
    {
        automatic,
        manual
    }

    void Start()
    {

        if (actualizationType == E_EventExecutionType.automatic)
        {
            StartCoroutine(oneFrame());
        }
    }

    IEnumerator oneFrame()
    {
        yield return null;
        testAndActualize(true);
        while (true)
        {
            testAndActualize();
            yield return null;
        }

    }

    public void ExecuteConditionCheck()
    {
        testAndActualize(true);
    }

    private void testAndActualize(bool initialize = false)
    {

        bool result = valueManager.instance.AreConditinsForResultMet(conditionsToTest);

        if (result == true)
        {
            actualize(TrueSpriteSet);
        }
        else
        {
            actualize(FalseSpriteSet);
        }

    }

    void actualize(ValueScript.valueToIcon[] spriteSet)
    {
        ValueScript vs = valueManager.instance.getFirstFittingValue(modifier);

        if (vs != null)
        {
            float value = valueManager.instance.getFirstFittingValue(modifier).value;
            actualizeIconsBaseOnValue(spriteSet, value);
        }
        else
        {
            Debug.LogError("The value '" + modifier.ToString() + "' couldn't be found int the manager.");
        }
    }

    void actualizeIconsBaseOnValue(ValueScript.valueToIcon[] spriteSet, float value)
    {

        //get through the list to get the new icon
        Sprite changedSprite = null;
        for (int i = 0; i < spriteSet.Length; i++)
        {
            if (value >= spriteSet[i].minValue)
            {
                changedSprite = spriteSet[i].icon;
            }
        }

        if (changedSprite != null)
        {
            targetImage.overrideSprite = changedSprite;
        }
    }
}
