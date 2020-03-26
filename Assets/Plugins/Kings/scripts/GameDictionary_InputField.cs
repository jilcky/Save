using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class GameDictionary_InputField : MonoBehaviour
{
    [Tooltip("Please provide an key for acessing the data from the dictionary.")]
    public string DictionaryKey = "NewKey";
    public bool allowEmptyString = false;
    private InputField inputField;

    private void Start()
    {
        inputField = gameObject.GetComponent<InputField>();
        if(CardStack.instance != null)
        {
            CardStack.instance.OnCardDestroy += OnCardDestroy;
        }
    }

    //there are two possible ways to save
    //1. calling save()
    //2. automatically save if the element gets destroyed
    public bool SaveEntry()
    {
        string input = inputField.text;
        if(allowEmptyString == false) {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
        }

        GameDictionary.SetEntry(DictionaryKey, input);
        return true;
    }

    /// <summary>
    /// Takes the information from the text field and tries to save it in the dictionary.
    /// </summary>
    public void Save()
    {
        //Debug.Log("save entry");
        SaveEntry();
    }

    private void OnCardDestroy()
    {
        SaveEntry();
        if (CardStack.instance != null)
        {
            CardStack.instance.OnCardDestroy -= OnCardDestroy;
        }
    }
}
