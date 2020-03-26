using System.Text.RegularExpressions;
using UnityEngine;

public class TextReplacement
{

    /// <summary>
    /// Translate a string and then parse additional information into the string.
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static string TranslateAndReplace(string txt)
    {
        string tmp = TranslationManager.translateIfAvail(txt);
        tmp = ReplaceStringPlaceHolders(tmp);
        return tmp;
    }

    /// <summary>
    /// Enhance a string by replacing place holders with values. 
    /// This should be done after translations, else the translation term is not recognized anymore. 
    /// To enshure the correct order use 'TranslateAndReplace(string txt)' instead of this method.
    /// 
    /// Actual keywords are:
    /// item
    ///     - can be followed by format
    ///     - should not be followed by preview (not supported yet, gives wrong results)
    /// value
    ///     - can be followed by preview, format
    /// dictionary
    ///     - replaces dictionary entry with string, format is not supported
    /// change
    ///     - similar to preview but shows the change of the value
    ///     - value should be declared first
    ///     - can have the parameters:
    ///         * up,down,left,right,add0,add1
    ///     - can differ from value preview because an other method for determing randomness for the value is used
    /// result
    ///     - similar to change but shows the possible result of the value
    ///     - value should be declared first
    ///     - can have the parameters:
    ///         * up,down,left,right,add0,add1
    ///     - can differ from value preview because an other method for determing randomness for the value is used
    /// format
    ///     - can be used every time and defines the formatter of the string cast e.g. '0.0', '##' etc.
    ///  
    /// 
    /// Examples for place holders:
    ///  {value=army}              -> is replaced by the actual value of army (if defined in value definitions), e.g. 75
    ///  {value=army,format=0.0}   -> is replace like before but with formatting, e.g. 75.0
    ///  {value=army,preview=up}   -> is replaced by the preview for army when swiping up
    ///  {item=sword}              -> is replaced by the number of swords in the inventory
    ///  {dictionary=name}         -> is replaced by the value for the key 'name'. 
    /// </summary>
    /// <param name="input">Input string with place holder(s).</param>
    /// <returns>String with replaced parts.</returns>
    public static string ReplaceStringPlaceHolders(string input)
    {
        string output = string.Copy(input);
        string query = input;
        string match = "";
        string matchNoSpace = "";
        string pattern = @"\{(.*?)\}";
        string[] commandSeqences;
        string[] commandSplit;
        var matches = Regex.Matches(query, pattern);    //e.g. "your army strenth is {army,format=0.0}." returns "army,format=0.0" in the first element (not a string yet)

        foreach (Match m in matches)
        {
            //found a command/place holder
            match = m.Groups[1].ToString();

            //remove space characters 
            matchNoSpace = match.Replace(" ", string.Empty);

            commandSeqences = matchNoSpace.Split(',');    //e.g. "army,format=0.0" is split into "army" and "format=0.0"

            //variables resulting from different commands
            string format = "";
            ValueScript vs = null;
            bool valueChangePreviewRequested = false;
            bool valueResultPreviewRequested = false;
            bool randomIndependent = true;
            float previewValue = 0f;
            EventScript es = null;
            string SwipeDirectionString = "";
            string dictionaryValue = "";
            bool dictionaryRequested = false;

            Inventory.C_ItemAmount ia = null;
            int itemAmount = 0;
            bool itemAmountRequested = false;

            //parse the parameters
            foreach (string parameter in commandSeqences)
            {
                commandSplit = parameter.Split('='); //e.g. split "format=0.0" into "format" and "0.0". Can't use ':' here, because this can also be within the string format.
                switch (commandSplit[0])
                {
                    case "value":
                        if (commandSplit.Length > 1) {
                            //parse the matching value
                            if (valueDefinitions.values.IsDefined(typeof(valueDefinitions.values), commandSplit[1]))
                            {
                                //matching place holder was found
                                valueDefinitions.values valueType = (valueDefinitions.values)System.Enum.Parse(typeof(valueDefinitions.values), commandSplit[1], true);
                                vs = valueManager.instance.getFirstFittingValue(valueType);
                            }
                            else
                            {
                                //command/place holder is unknown
                                Debug.LogWarning("The value '"+commandSplit[1]+"' for the place holder {" + match + "} is unknown.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("The keyword 'value' for the place holder {" + match + "} is not followed by a selections, e.g. 'value=years'");
                        }
                        break;
                    case "format":
                        if (commandSplit.Length > 1)
                        {
                            format = commandSplit[1];
                        }
                        else
                        {
                            Debug.LogWarning("Command 'format' in text '" + match + "' is not followed by a valid parameter, e.g. format=0.0.");
                        }
                        break;
                    case "item":
                        itemAmountRequested = true;
                        if (commandSplit.Length > 1)
                        {
                            //find the matching item
                            if (Inventory.instance != null)
                            {
                                ia = Inventory.instance.GetItemByKey(commandSplit[1]);
                                //command/place holder is unknown
                                if (ia != null)
                                {
                                    itemAmount = ia.amount;
                                }
                                else
                                {
                                    //if the item is not within the inventory it still can be in the catalog (amount is 0)
                                    InventoryItem item = Inventory.instance.GetItemFromCatalogByKey(commandSplit[1]);
                                    itemAmount = 0;
                                    if (item != null)
                                    {
                                        //item is ok an known, amount is 0
                                    }
                                    else
                                    { 
                                        Debug.LogWarning("The item '" + commandSplit[1] + "' for the place holder {" + match + "} is unknown.");
                                    }
                                }
                            }
                            else {
                                Debug.LogWarning("The item '" + commandSplit[1] + "' for the place holder {" + match + "} can't be searched because there is no script 'LocalInventory.cs' found in the scene.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("The keyword 'item' for the place holder {" + match + "} is not followed by a selections, e.g. 'item=sword'");
                        }

                        break;
                    case "dict":    /*short writing for dictionary is the same case as */
                    case "dictionary":
                        dictionaryRequested = true;
                        dictionaryValue = GameDictionary.GetValue(commandSplit[1]);
                        break;
                    case "change":
                        if(vs != null && commandSplit.Length > 1)
                        {
                            //get the reference to the actual card
                            if (CardStack.instance != null)
                            {
                                if (CardStack.instance.spawnedCard != null) {
                                    es = CardStack.instance.spawnedCard.GetComponent<EventScript>();
                                    if (es != null)
                                    {
                                        SwipeDirectionString = commandSplit[1];
                                        //Debug.Log("Card is " + CardStack.instance.spawnedCard.name);
                                        if (!string.IsNullOrEmpty(SwipeDirectionString))
                                        {
                                            es.computeTextPreview(SwipeDirectionString, vs.valueType, ref previewValue,ref randomIndependent);
                                            valueChangePreviewRequested = true;
                                        }
                                        else
                                        {
                                            Debug.LogWarning("Command 'direction' in text '" + match + "' is not defined yet, e.g. 'direction=left'.");
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogWarning("The preview for value '" + commandSplit[1] + "' for the place holder {" + match + "} can't be searched because there is no 'EventScrip' attached to the actual spawned card.");
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("The preview for value '" + commandSplit[1] + "' for the place holder {" + match + "} can't be searched because there is no actual spawned card.");
                                }
                            }
                            else
                            {
                                Debug.LogWarning("The preview for value '" + commandSplit[1] + "' for the place holder {" + match + "} can't be searched because there is no script 'CardStack.cs' found in the scene.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("The keyword 'preview' for the place holder {" + match + "} is not followed by a selections or no value was defined yet.");
                        }
                        break;
                    case "result":
                        if (vs != null && commandSplit.Length > 1)
                        {
                            //get the reference to the actual card
                            if (CardStack.instance != null)
                            {
                                if (CardStack.instance.spawnedCard != null)
                                {
                                    es = CardStack.instance.spawnedCard.GetComponent<EventScript>();
                                    if (es != null)
                                    {
                                        SwipeDirectionString = commandSplit[1];
                                        //Debug.Log("Card is " + CardStack.instance.spawnedCard.name);
                                        if (!string.IsNullOrEmpty(SwipeDirectionString))
                                        {
                                            es.computeTextPreview(SwipeDirectionString, vs.valueType, ref previewValue, ref randomIndependent);
                                            //calculate the result
                                            previewValue += vs.value;
                                            if(previewValue < vs.limits.min)
                                            {
                                                previewValue = vs.limits.min;
                                            }
                                            if(previewValue > vs.limits.max)
                                            {
                                                previewValue = vs.limits.max;
                                            }
                                            valueResultPreviewRequested = true;
                                        }
                                        else
                                        {
                                            Debug.LogWarning("Command 'direction' in text '" + match + "' is not defined yet, e.g. 'direction=left'.");
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogWarning("The preview for value '" + commandSplit[1] + "' for the place holder {" + match + "} can't be searched because there is no 'EventScrip' attached to the actual spawned card.");
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("The preview for value '" + commandSplit[1] + "' for the place holder {" + match + "} can't be searched because there is no actual spawned card.");
                                }
                            }
                            else
                            {
                                Debug.LogWarning("The preview for value '" + commandSplit[1] + "' for the place holder {" + match + "} can't be searched because there is no script 'CardStack.cs' found in the scene.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("The keyword 'preview' for the place holder {" + match + "} is not followed by a selections or no value was defined yet.");
                        }
                        break;
                    default:
                        Debug.LogWarning("Command '" + commandSplit[0] + "' in place holder '" + match + "' is not recognized.");
                        break;
                }
            }

            //replace the original match
            if (valueChangePreviewRequested == true)
            {
                //replace place holder with change preview
                if (randomIndependent == true)
                {
                    output = output.Replace("{" + match + "}", previewValue.ToString(format));
                }
                else
                {
                    output = output.Replace("{" + match + "}", "?");
                }
            }
            else if (valueResultPreviewRequested == true)
            {
                //replace place holder with result preview
                if (randomIndependent == true)
                {
                    output = output.Replace("{" + match + "}", previewValue.ToString(format));
                }
                else
                {
                    output = output.Replace("{" + match + "}", "?");
                }
            }
            else if (dictionaryRequested == true)
            {
                //repalce place holder with dictionary entry
                output = output.Replace("{" + match + "}", dictionaryValue);
            }
            else if (vs != null)
            {
                //replace place holder with value
                output = output.Replace("{" + match + "}", vs.value.ToString(format));
            }
            else if (itemAmountRequested == true)
            {
                //replace place holder with item amount
                output = output.Replace("{" + match + "}", itemAmount.ToString(format));
            }
            else
            {
                //If nothing matches: replace placeholder with escape-string. User should not see internal structure like {value:....}. 
                output = output.Replace("{" + match + "}", "<?>");
            }



        }


        return output;
    }

}
