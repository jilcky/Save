using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class KingsCardStyleList : ScriptableObject
{

    [System.Serializable]
    public class C_CardStyleNamePair {
        //public string styleName = "default";
        public KingsCardStyle style;
        //[Tooltip("Execute style override if checked.")]
       // public bool overwrite = true;
    }



    public bool HasStyle(string style)
    {
        return (GetStyle(style) != null);
    }

    public string GetCardStyleDefinitionErrors()
    {
        string errors = "";
        for (int i = 0; i < cardStyles.Count; i++)
        {
            /*if (string.IsNullOrEmpty(cardStyles[i].style.name))
            {
                errors += "Element " + i.ToString() + " is missing a style-name.\n";
            }*/
            if (cardStyles[i].style.prefab == null)
            {
                errors += "Element " + i.ToString() + " ('" + cardStyles[i].style + "') is missing a missing a prefab.\n";
            }
            if (cardStyles[i].style.icon == null)
            {
                errors += "Element " + i.ToString() + " ('" + cardStyles[i].style + "') is missing a missing an icon.\n";
            }
        }
        return errors;
    }

    public KingsCardStyle GetStyle(string style)
    {
        for (int i = 0; i < cardStyles.Count; i++)
        {
            if (style == cardStyles[i].style.name)
            {
                return cardStyles[i].style;
            }
        }

        return null;
    }

    public bool GetOverwriteStyle(string style) {

        return true;

        /*for (int i = 0; i < cardStyles.Count; i++)
        {
            if (style == cardStyles[i].style.name)
            {
                return cardStyles[i].overwrite;
            }
        }

        return false;*/
    }

    public List<C_CardStyleNamePair> cardStyles = new List<C_CardStyleNamePair>();



}
