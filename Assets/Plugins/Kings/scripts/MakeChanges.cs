using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Helper Script to make effects (former called results, like on the EventScript) usable from Events.

public class MakeChanges : TranslatableContent
{
    //Definition of an effect, configuration is done from inspector.
    public ConditionsAndEffects.C_Changes effect;


    private void Start()
    {
        //if this script is used directly, register it at the Translation manager. If it is on a card, it should be detected by the CardStack.
        TranslationManager.instance.registerTranslateableContentScript(this);
    }

    /// <summary>
    /// Execution of the effect of this script.
    /// </summary>
    public void ExecuteEffect()
    {
        effect.ExecuteEffect();
    }

    public List<string> GetTranslatableContent()
    {
        return effect.GetTranslatableContent();
    }

    /*
 * Return all possible translatable terms
 */
    public override List<string> getTranslatableTerms()
    {
        return GetTranslatableContent();
    }

}
