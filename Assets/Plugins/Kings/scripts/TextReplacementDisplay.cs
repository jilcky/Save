using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Helper script to allow using the text replacement features as standalone scrpt.
public class TextReplacementDisplay : TranslatableContent
{

    public EventScript.C_Texts textField;
    public string text = "";

    public bool autoactualization = false;

    private void Start()
    {
        TranslationManager.instance.registerTranslateableContentScript(this);
        StartCoroutine(delay());
        StartCoroutine(cyclicActualization());
    }

    IEnumerator delay()
    {
        yield return null;
        replace();
    }

    IEnumerator cyclicActualization()
    {
        while (true)
        {
            if (autoactualization == true)
            {
                replace();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void actualizeTextOutput()
    {
        StartCoroutine(delay());
    }

    private void replace()
    {
        textField.text = TextReplacement.TranslateAndReplace(text);
        //Debug.Log("Replacement: " + textField.textField.text);
    }

    //Return all possible translatable terms
    public override List<string> getTranslatableTerms()
    {
        List<string> terms = new List<string>();
        terms.Add(text);
        return terms;
    }

}
