using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountryNameDisplay : MonoBehaviour {


    public Text nameText;
    public Text countryText;
    public Text countryNameText;

	void Start () {
        clearTexts();
        StartCoroutine(twoFrames());
	}

    //links are generatet with one frame delay, 
    //to access in an guaranteed working order this script delays two frames
    IEnumerator twoFrames()
    {
        yield return null;
        yield return null;
        actualizeTexts();
    }

    void actualizeTexts()
    {

        if (CountryNameGenerator.instance != null)
        {

            if (nameText != null)
            {
                nameText.text = CountryNameGenerator.instance.GetNameString();
            }

            if (countryText != null) {
                countryText.text = CountryNameGenerator.instance.GetCountryString();
            }

            if (countryNameText != null) {
                countryNameText.text = CountryNameGenerator.instance.GetCountryNameString();
            }
        }
    }

    void clearTexts()
    {
        if (nameText != null)
        {
            nameText.text = "";
        }

        if (countryText != null)
        {
            countryText.text = "";
        }

        if (countryNameText != null)
        {
            countryNameText.text = "";
        }
    }

}
