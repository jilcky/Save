using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class valueDependentImages : MonoBehaviour {

	public Image targetImage;
	public valueDefinitions.values modifier;

	public ValueScript.valueToIcon[] valueDependingIcons;


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



	private float oldValueForIcon = -1f;
	void actualizeIconsBaseOnValue(){

		//only compute, if the value changed
		if (value != oldValueForIcon) {

			//get through the list to get the new icon
			Sprite changedSprite = null;
			for(int i = 0; i<valueDependingIcons.Length; i++){
				if (value >= valueDependingIcons [i].minValue) {
					changedSprite = valueDependingIcons [i].icon;
				}
			}

			if (changedSprite != null) {
				targetImage.overrideSprite = changedSprite;
			}
		}
		oldValueForIcon = value;
	}

}
