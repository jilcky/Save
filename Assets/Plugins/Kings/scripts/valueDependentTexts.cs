using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class valueDependentTexts : MonoBehaviour {

	public EventScript.C_Texts textTarget;
	public valueDefinitions.values modifier;

	public ValueScript.valueToText[] valueDependingTexts;


	float value = 0f;


	void Start(){
		//actualize ();
		StartCoroutine (cycActulize ());
	}


	IEnumerator cycActulize(){
        yield return null;
		while (true) {
			actualize ();
			yield return new WaitForSeconds (1f);
		}
	}

	void actualize(){
        ValueScript vs = valueManager.instance.getFirstFittingValue(modifier);

        if (vs != null)
        {
            value = valueManager.instance.getFirstFittingValue(modifier).value;
            actualizeIconsBaseOnValue();
        }
        else
        {
            Debug.LogError("The value '" + modifier.ToString() + "' couldn't be found int the manager.");
        }
	}



	private float oldValueForText = -1f;
	void actualizeIconsBaseOnValue(){

		//only compute, if the value changed
		if (value != oldValueForText) {

            //get through the list to get the new icon
            string newText = "";
			for(int i = 0; i< valueDependingTexts.Length; i++){
				if (value >= valueDependingTexts[i].minValue) {
                    newText = TextReplacement.TranslateAndReplace(valueDependingTexts[i].text); 
				}
			}

            textTarget.text = newText;
		}
        oldValueForText = value;
	}

}
