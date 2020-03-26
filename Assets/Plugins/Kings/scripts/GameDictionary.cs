using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Game Dictionary (string,string) allows arbitrary entries at runtime. 
/// It can be used for different purposes like memorizing drawn cards, gamestates etc.
/// The Game Dictionary is included in the Condition and Result System.
/// 
/// It is not necessary to instantiate the GameDictionary. It's completely static, loads bevore the first access and saves on each change.
/// </summary>


public class GameDictionary:MonoBehaviour
{
    private static string jsonKey = "GameDictionary";
    private static bool isLoaded = false;

    private static void mDebug(string txt)
    {
        //Debug.Log(txt);
    }

    [System.Serializable]
    public enum E_DictionaryAction
    {
        set /*further actions to be defined */
    }

    [System.Serializable]
    public class C_DictionaryChange
    {
        public E_DictionaryAction dictionaryAction;
        public string key;
        public string value;
    }
       
    [System.Serializable]
    public class C_DictWrapper
    {
        public Dictionary<string, string> dictionary;

        //clone method to allow casting from static to non static and vice versa
        public C_DictWrapper(C_DictWrapper dictionaryWrapper)
        {
            dictionary = new Dictionary<string, string>(dictionaryWrapper.dictionary);
        }
        public C_DictWrapper(Dictionary<string, string> original)
        {
            dictionary = new Dictionary<string, string>(original);
        }
        public C_DictWrapper()
        {
            dictionary = new Dictionary<string, string>();
        }
        public override string ToString()
        {
            string data = "";
            foreach (var kvp in dictionary)
            {
                data += "{" + kvp.Key + "," + kvp.Value + "}";
            }
            return data;
        }
    }

    //Dictionary<string,string> serialization workaround
    [System.Serializable]
    public class C_keyValuePair
    {
        public C_keyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key;
        public string Value;
    }
    [System.Serializable]
    public class C_KeyValueArray
    {
        public List<C_keyValuePair> pairs = new List<C_keyValuePair>();
        public Dictionary<string, string> dictionary = new Dictionary<string, string>();
        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach(C_keyValuePair p in pairs)
            {
                dictionary.Add(p.Key, p.Value);
            }
            return dictionary;
        }
        public C_KeyValueArray ToKeyValuePairs(Dictionary<string, string> tmpDictionary)
        {
            pairs = new List<C_keyValuePair>();
            foreach (var p in tmpDictionary)
            {
                C_keyValuePair kvp = new C_keyValuePair(p.Key, p.Value);
                pairs.Add(kvp);

            }
            return this;
        }
        public override string ToString()
        {
            string data = "";
            foreach(C_keyValuePair kvp in pairs)
            {
                data += "{" + kvp.Key + "," + kvp.Value + "}";
            }
            return data;
        }
    }
    // end of workaround

    public static C_DictWrapper dictionaryWrapper;

    public static void executeDictionaryResult(C_DictionaryChange result)
    {
        switch (result.dictionaryAction)
        {
            case E_DictionaryAction.set:
                SetEntry(result.key, result.value);
                break;
            default:
                Debug.LogWarning("The game dictionary action '" + result.dictionaryAction.ToString() + "' is unknown and can not be executed.");
                break;
        }
    }

    [System.Serializable]
    public enum E_GameDictionaryConditionTypes
    {
        equals,
        contains
    }

    [System.Serializable]
    public class C_GameDictionaryCondition
    {
        public E_GameDictionaryConditionTypes conditionType = E_GameDictionaryConditionTypes.equals;
        public string key;
        public string comparer;
    }

    public static bool ConditionMet(C_GameDictionaryCondition condition)
    {
        switch (condition.conditionType)
        {
            case E_GameDictionaryConditionTypes.equals:
                return EqualsValue(condition.key, condition.comparer);
            case E_GameDictionaryConditionTypes.contains:
                return ContainsKey(condition.key);
            default:
                break;
        }
        return false;
    }

    /// <summary>
    /// Return true if the Game Dictionary contains specific key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ContainsKey(string key)
    {
        loadIfnecessary();
        return dictionaryWrapper.dictionary.ContainsKey(key);
    }

    /// <summary>
    /// Return the value of a specific key. Returns empty string if key is not available.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetValue(string key)
    {
        loadIfnecessary();
        string retString = "";
        if (dictionaryWrapper.dictionary.TryGetValue(key, out retString))
        {
            return retString;
        }
        return "";
    }

    /// <summary>
    /// Compare if value entry for key equals comparer. Returns true if it matches.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EqualsValue(string key, string comparer)
    {
        loadIfnecessary();
        string value = GetValue(key);
        return (comparer == value);
    }

    /// <summary>
    /// Set a new or existing value for a specific key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetEntry(string key, string value)
    {

        mDebug("Set key '"+key+"' to '"+value+"'");

        loadIfnecessary();
        if (dictionaryWrapper.dictionary.ContainsKey(key))
        {
            dictionaryWrapper.dictionary[key] = value;
        }
        else
        {
            dictionaryWrapper.dictionary.Add(key, value);
        }
        save();
    }

    /// <summary>
    /// Clears the Game Dictionary;
    /// </summary>
    public static void StaticClear()
    {
        loadIfnecessary();
        dictionaryWrapper.dictionary.Clear();
        save();
    }

    /// <summary>
    /// Clears the Game Dictionary (not static to allow adding this script and calling from an event)
    /// </summary>
    public void Clear()
    {
        StaticClear();
    }

    private static void load()
    {
        //load quests states from PlayerPrefs
        string json = SecurePlayerPrefs.GetString(jsonKey);

        if (!string.IsNullOrEmpty(json))
        {
            C_KeyValueArray kva = new C_KeyValueArray();
            JsonUtility.FromJsonOverwrite(json, kva);

            dictionaryWrapper = new C_DictWrapper(kva.ToDictionary());
            //Debug.Log("load kva: " + kva.ToString());
           // Debug.Log("load kvp: " + dictionaryWrapper.ToString());
        }
        else
        {
            dictionaryWrapper = new C_DictWrapper();
        }


        isLoaded = true;
    }

    private static void loadIfnecessary()
    {
        if (dictionaryWrapper == null)
        {
            load();
        }
    }

    private static void save()
    {
        if (isLoaded)
        {
            C_KeyValueArray kva = new C_KeyValueArray();
            string json = JsonUtility.ToJson(kva.ToKeyValuePairs(dictionaryWrapper.dictionary));
            SecurePlayerPrefs.SetString(jsonKey, json);
            //Debug.Log("kva save: " + kva.ToString());
            //Debug.Log("save: "+json);
        }
        else
        {
            Debug.LogError("Game dictionary can not be altered bevore it is loaded. Your change is lost.");
        }
    }
}
