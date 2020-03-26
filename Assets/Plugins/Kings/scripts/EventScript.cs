
//#define ES_USE_TMPRO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;



public class EventScript : MonoBehaviour {
    [System.Serializable] public class mEvent : UnityEvent { }

    [System.Serializable]
    public class eventText
    {
        public string textContent;
#if ES_USE_TMPRO
		public TMPro.TextMeshProUGUI TMProField;
#endif
        public Text textField;
    }

    //Position is not so nice, but for compatibility reason: #ES_USE_TMPRO - Define only on one place. 
    [System.Serializable]
    public class C_Texts
    {
#if ES_USE_TMPRO
		public TMPro.TextMeshProUGUI TMProField;
#endif
        public Text textField;

        public string text
        {
            set
            {
#if ES_USE_TMPRO
                if (TMProField != null) {
                    TMProField.text = value;
                }
#endif
                if (textField != null) {
                    textField.text = value;
                }
            }
        }
    }

    [System.Serializable]
    public class eventTexts {
        public eventText titleText;
        public eventText questionText;
        public eventText answerLeft;
        public eventText answerRight;
        public eventText answerUp;
        public eventText answerDown;
        public List<eventText> additionalTexts = new List<eventText>();

        public string[] getCsvHeader() {
            string[] ret;
            ret = new string[6];

            ret[0] = "EventScript.titleText";
            ret[1] = "EventScript.questionText";
            ret[2] = "EventScript.answerLeft";
            ret[3] = "EventScript.answerRight";
            ret[4] = "EventScript.answerUp";
            ret[5] = "EventScript.answerDown";
            //additional texts not possible at the moment
            return ret;
        }

        public string[] getCsvData() {
            string[] ret;
            ret = new string[6];

            ret[0] = titleText.textContent;
            ret[1] = questionText.textContent;
            ret[2] = answerLeft.textContent;
            ret[3] = answerRight.textContent;
            ret[4] = answerUp.textContent;
            ret[5] = answerDown.textContent;
            //additional texts not possible at the moment
            return ret;
        }
        //Yes, I know. Could also be done with serialized objects in editor scripts.
        public bool setData(string variable, string data) {
            switch (variable) {
                case "titleText":
                    titleText.textContent = data;
                    break;
                case "questionText":
                    questionText.textContent = data;
                    break;
                case "answerLeft":
                    answerLeft.textContent = data;
                    break;
                case "answerRight":
                    answerRight.textContent = data;
                    break;
                case "answerUp":
                    answerUp.textContent = data;
                    break;
                case "answerDown":
                    answerDown.textContent = data;
                    break;
                default:
                    Debug.LogWarning("The variable '"+ variable + "' is unknown and could not be written.");
                    return false;
        }
            return true;
        }
    }

    #region data import, setter
    public void SetImportData(string key, string data) {
        switch (key)
        {
            //fall through for data of the text fields
            case "titleText":
            case "questionText":
            case "answerLeft":
            case "answerRight":
            case "answerUp":
            case "answerDown":
                //delegate data to text fields
                textFields.setData(key, data);
                break;
            default:
                Debug.LogWarning("The variable '" + key + "' is unknown..");
                break;
        }
    }

    #endregion

#if ES_USE_TMPRO
	public static bool useTextMeshPro = true;
#else
    public static bool useTextMeshPro = false;
#endif

    [Tooltip("Define your card texts and text fields here. The strings can be terms for the 'TranslationManager'.")]
    public eventTexts textFields;

    [Tooltip("If a card is high priority, it will be draw before all other normal cards, but after follow up cards.")]
    public bool isHighPriorityCard = false;
    [Tooltip("Only drawable cards can be randomly drawn because of their condition. Non drawable cards are follow up cards which are defined by previous cards or cards like the gameover statistics.")]
    public bool isDrawable = true;

    [Tooltip("The propability applies to all cards, which met the conditions. Cards with a higher propability are more likely to be drawn.")]
    [Range(0f, 1f)] public float cardPropability = 1f;
    [Tooltip("To limit the maximum draws of a card per game, define the 'maxDraws'.")]
    public int maxDraws = 100;
    [Tooltip("After a card is drawn, define for how many cycles it is blocked to be redrawn. This doesn't apply to follow up cards.")]
    public int redrawBlockCnt = 0;


    [System.Serializable]
    public enum E_ConditionType {
        standard,
        compareValues,
        items,
        dictionaryEquals
    }

    [System.Serializable]
    public enum E_ConditionCompareType
    {
        greaterThan,
        greaterThanOrEqual,
        equals,
        equalsAsInt,
        lessThan,
        lessThanOrEqual
    }

    [System.Serializable]
    public enum E_ItemCompareType
    {
        greaterThan,
        equals,
        lessThan
    }

        [System.Serializable]
	public class condition{
        public E_ConditionType type;

		public valueDefinitions.values value;
		public float valueMin = 0f;
		public float valueMax = 100f;

        public E_ConditionCompareType compareType;
        public valueDefinitions.values rValue;

        //extension 11.03.2019: items
        public E_ItemCompareType itemCompareType = E_ItemCompareType.greaterThan;
        public InventoryItem item;
        public int itemCmpValue = 1;

        //extension 02.04.2019: dictionary
        public string gamedictionary_key;
        public string gamedictionary_comparer;
    }

	[Tooltip("Define under wich conditions this card can be drawn. E.g. a marriage card should only be possible if a value type 'age' is in the range of 18 to 100 or the value type 'marriage' is zero (not married yet)")]
	public condition[] conditions;

    [System.Serializable]
    public enum E_ModificationType
    {
        add,
        set,
        setRandom,
        addRandom,
        setRandInt,
        addRandInt
    }

    [System.Serializable]
    public class C_RndRange
    {
        public float min = 0f;
        public float max = 100f;
    }

    [System.Serializable]
	public class resultModifier{
        public E_ModificationType modificationType = E_ModificationType.add;
        public valueDefinitions.values modifier;

        //Adding all this values to enable switching by the designer and saving it by the internal serialization.
		public float valueAdd = 0f;
        public float valueSet = 0f;
        public C_RndRange rndRangeAdd = new C_RndRange();
        public C_RndRange rndRangeSet = new C_RndRange();
        //adding an event as an modifier type here would be nice, but crashes unity (serialization depth?). :(
	}

    //For backward compatibility of the setValue and addValue script.
    [System.Serializable]
    public class C_ReducedResultModifier
    {
        public valueDefinitions.values modifier;
        public float valueAdd = 0f;
    }

	[System.Serializable]
	public class resultModifierPreview{
		public resultModifier resultModification;
		public bool modificationIsRandomIndependant = true;
	}

	[System.Serializable]
	public enum resultTypes{
		simple,
		conditional,
		randomConditions,
		random
	}

    [System.Serializable]
    public enum E_ModifierTargetType
    {
        item,
        dictionary,
        quest, 
        timeline,
        gamelog
    }

    [System.Serializable]
    public class C_AdditionalModifiers
    {
        public E_ModifierTargetType targetType;
        public Inventory_ChangeItem.itemModifier itemChange;
        public GameDictionary.C_DictionaryChange dictionaryChange;
        public Quests.C_QuestChange questChange;
        public Timeline.C_TimelLineEventChange historyEvent;
        public GameLogger.C_InspectorGameLogEntry newGameLog;


        public List<string> GetTranslatableContent()
        {
            List<string> tContent = new List<string>();
            tContent.AddRange(historyEvent.GetTranslatableContent());
            tContent.AddRange(newGameLog.GetTranslatableContent());
            return tContent;
        }
    }


    [System.Serializable]
    public class C_intRange
    {
        public int min = 0;
        public int max = 0;
    }

    [System.Serializable]
	public class modifierGroup{
		public resultModifier[] valueChanges;
        public C_AdditionalModifiers[] extras;
		[Tooltip("If this path was taken, will there be a 'follow up' card which takes the story further? Can be left empty.")]
		public GameObject followUpCard;
        public C_intRange followUpDelay = new C_intRange();

        public List<string> GetTranslatableContent()
        {
            List<string> tContent = new List<string>();
            foreach (C_AdditionalModifiers addMod in extras)
            {
                tContent.AddRange(addMod.GetTranslatableContent());
            }
            return tContent;
        }

            // public mEvent _mEvent; // could work, but has to be considered in some cases. Perhaps for future updates.
        }

	[System.Serializable]
	public class result{
		public resultTypes resultType;
		[Tooltip("Which values are modified, if this result is selected?")]
		public modifierGroup modifiers;
		[Tooltip("Depending on further conditions the result can split into two different outcomes. If all conditions are true, the 'Modifiers True' are executed. If one of the conditions fails, the 'Modifiers False'. E.g. the user selected he wants to take a race but his 'agility' value is to low, as outcome he will lose.")]
		public condition[] conditions;
		[Tooltip("Group of value changes, if all conditions are met.")]
		public modifierGroup	modifiersTrue;
		[Tooltip("Group of value changes, if at least one of the conditions fails.")]
		public modifierGroup	modifiersFalse;
		[Tooltip("A result can be split in multible outcomes. The 'Random Mofifiers' can be predefined, the selection of the outcome is randomly one of these.")]
		public modifierGroup[]  randomModifiers;

        public List<string> GetTranslatableContent()
        {
            List<string> tContent = new List<string>();
            tContent.AddRange(modifiers.GetTranslatableContent());
            tContent.AddRange(modifiersTrue.GetTranslatableContent());
            tContent.AddRange(modifiersFalse.GetTranslatableContent());
            foreach (modifierGroup mg in randomModifiers) {
                tContent.AddRange(mg.GetTranslatableContent());
                    }

            return tContent;
        }
	}

    [System.Serializable]
    public class resultGroup
    {
        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card left.")]
        public result resultLeft;

        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card right.")]
        public result resultRight;

        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card up.")]
        public result resultUp;

        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card down.")]
        public result resultDown;


        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user selects addtional choices.")]
        public result additional_choice_0;

        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user selects addtional choices.")]
        public result additional_choice_1;

        public List<string> GetTranslatableContent()
        {
            List<string> tContent = new List<string>();
            tContent.AddRange(resultLeft.GetTranslatableContent());
            tContent.AddRange(resultRight.GetTranslatableContent());
            tContent.AddRange(resultUp.GetTranslatableContent());
            tContent.AddRange(resultDown.GetTranslatableContent());
            tContent.AddRange(additional_choice_0.GetTranslatableContent());
            tContent.AddRange(additional_choice_1.GetTranslatableContent());

            return tContent;
        }
    }

	public resultGroup Results;

    //Configuration: use 2 way swipe style or use 4 way swipe
    [System.Serializable]
    public enum E_SwipeType {
        LeftRight,
        FourDirection
    }
    [Tooltip("Define the type of the swipe for this card. \n'LeftRight' enables 2 choices. \n'FourDirection' allows 4 choices.\nFor more choices please use the multi-choice template.")]
    public E_SwipeType swipeType;

    [Tooltip("Add two additional choices for e. g. multichoice card.")]
    public bool additionalChoices = false;

    //Try to translate and write the configurated texts to their text-fields. 
    void writeTextFields()
    {
        writeEventTextField(textFields.titleText, TextReplacement.TranslateAndReplace(textFields.titleText.textContent));
        writeEventTextField(textFields.questionText, TextReplacement.TranslateAndReplace(textFields.questionText.textContent));
        writeEventTextField(textFields.answerLeft, TextReplacement.TranslateAndReplace(textFields.answerLeft.textContent));
        writeEventTextField(textFields.answerRight, TextReplacement.TranslateAndReplace(textFields.answerRight.textContent));
        writeEventTextField(textFields.answerUp, TextReplacement.TranslateAndReplace(textFields.answerUp.textContent));
        writeEventTextField(textFields.answerDown, TextReplacement.TranslateAndReplace(textFields.answerDown.textContent));


        if (textFields.additionalTexts != null)
        {
            foreach (eventText et in textFields.additionalTexts)
            {
                if (et != null && et.textContent != null && et.textField != null)
                {
                    //et.textField.text = TextReplacement.TranslateAndReplace(et.textContent);
                    writeEventTextField(et, TextReplacement.TranslateAndReplace(et.textContent));
                }
            }
        }
    }


    void writeEventTextField(eventText et, string txt)
    {
#if ES_USE_TMPRO
        if(et.TMProField != null)
        {
            et.TMProField.text = txt;
        }
#endif
        if (et.textField  != null)
        {
            et.textField.text = txt;
        }
    }

	/*
	 * Called by an event from the swipe script. 
	 * This triggers the computation of the results for swiping LEFT and afterward the spawning of a new card.
	 */
	public void onLeftSwipe(){
		result res = Results.resultLeft;
		computeResult (res);
		OnSwipeLeft.Invoke ();
	}
	/*
	 * Called by an event from the swipe script. 
	 * This triggers the computation of the results for swiping RIGHT and afterward the spawning of a new card.
	 */
	public void onRightSwipe(){
		result res = Results.resultRight;
		computeResult (res);
		OnSwipeRight.Invoke ();
	}
    /*
     * Called by an event from the swipe script. 
     * This triggers the computation of the results for swiping UP and afterward the spawning of a new card.
     * For compatibility: The execution is discarded if the swipe type is not configured for four directions.
     */
    public void onUpSwipe()
    {
        if (swipeType == E_SwipeType.FourDirection)
        {
            result res = Results.resultUp;
            computeResult(res);
            OnSwipeRight.Invoke();
        }
    }
    /*
     * Called by an event from the swipe script. 
     * This triggers the computation of the results for swiping DOWN and afterward the spawning of a new card.
     * For compatibility: The execution is discarded if the swipe type is not configured for four directions.
     */
    public void onDownSwipe()
    {
        if (swipeType == E_SwipeType.FourDirection)
        {
            result res = Results.resultDown;
            computeResult(res);
            OnSwipeRight.Invoke();
        }
    }

    public bool ExecuteAddtionalChoices(int choiceNr){
		bool executed = false;

		switch (choiceNr) {
		case 0:

			AdditionalChoice_0_Selection();
			executed = true;

			break;
		case 1:

			AdditionalChoice_1_Selection();
			executed = true;

			break;
		default:
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" is not configured in 'EventScript'.");
			break;	
		}

		return executed;
    }

	/* 
	 * This triggers the computation of the results for additonal options and afterward the spawning of a new card.
	 */
	public void AdditionalChoice_0_Selection(){
		result res = Results.additional_choice_0;
		computeResult (res);
		OnAdditionalChoice_0.Invoke ();
	}
	// Preview method for addition choice 0
	public void AdditionalChoice_0_Preview(){
		result res = Results.additional_choice_0;
		resetResultPreviews ();
		computeResultPreview (res);
	}

	/* 
	 * This triggers the computation of the results for additonal options and afterward the spawning of a new card.
	 */
	public void AdditionalChoice_1_Selection(){
		result res = Results.additional_choice_1;
		computeResult (res);
		OnAdditionalChoice_1.Invoke ();
	}
	// Preview method for addition choice 1
	public void AdditionalChoice_1_Preview(){
		result res = Results.additional_choice_1;
		resetResultPreviews ();
		computeResultPreview (res);
	}


	// Methods for previews of changes in values 

	/*
	 * Called by an event from the swipe script.
	 * This triggers the generation of a preview on the values for an LEFT swipe.
	 */
	public void onLeftSpwipePreview(){
		result res = Results.resultLeft;
		resetResultPreviews ();
		computeResultPreview (res);
		computeEventResultsPreview (OnSwipeLeft);
		computeEventResultsPreview (OnCardDespawn);
	}
	/*
	 * Called by an event from the swipe script.
	 * This triggers the generation of a preview on the values for an RIGHT swipe.
	 */
	public void onRightSpwipePreview(){
		result res = Results.resultRight;
		resetResultPreviews ();
		computeResultPreview (res);
		computeEventResultsPreview (OnSwipeRight);
		computeEventResultsPreview (OnCardDespawn);
	}
    /*
     * Called by an event from the swipe script.
     * This triggers the generation of a preview on the values for an UP swipe.
     */
    public void onUpSpwipePreview()
    {
        result res = Results.resultUp;
        resetResultPreviews();
        computeResultPreview(res);
        computeEventResultsPreview(OnSwipeUp);
        computeEventResultsPreview(OnCardDespawn);
    }
    /*
     * Called by an event from the swipe script.
     * This triggers the generation of a preview on the values for an DOWN swipe.
     */
    public void onDownSpwipePreview()
    {
        result res = Results.resultDown;
        resetResultPreviews();
        computeResultPreview(res);
        computeEventResultsPreview(OnSwipeDown);
        computeEventResultsPreview(OnCardDespawn);
    }

    //get possible results from events calling addValueToValue and add them to the preview
    public void computeEventResultsPreview(mEvent _mEvent){
		int eventCnt = _mEvent.GetPersistentEventCount();
		addValueToValue avtv;
		Object o;

		//check for each persistent call, if it is a 'addValueToValue' script
		for (int i = 0; i < eventCnt; i++) {

			//get the type of the persistent object, if it is correct cast it to 'addValueToValue'
			o = _mEvent.GetPersistentTarget (i);
			if (o.GetType () == typeof(addValueToValue)) {
				avtv = (addValueToValue)o;
			} else {
				avtv = null;
			}

			if (avtv != null) {

				//the object was a 'addValueToValue' script, therefore generate the previews

				previewModifiers.Clear();

				if (avtv.valuesToChange.Length > 0) {
					float rValue = 0f;
					float diff = 0f;
                    foreach (addValueToValue.resultModifierForAddingValueToValue valueAddValue in avtv.valuesToChange)
                    {

                        //generate a preview instance the change
                        resultModifierPreview preview = new resultModifierPreview();
                        preview.resultModification = new resultModifier();
                        preview.resultModification.modificationType = E_ModificationType.add;

                        rValue = valueManager.instance.getFirstFittingValue(valueAddValue.rArgument).value;
                        diff = rValue * valueAddValue.multiplier;
                        preview.resultModification.modifier = valueAddValue.lArgument;  // get the affected value
                        preview.resultModification.valueAdd = diff;
                        preview.modificationIsRandomIndependant = true;

                        previewModifiers.Add(preview);

                        //Debug.Log ("added event preview for " + preview.resultModification.modifier.ToString () + ", change is " + diff.ToString ());
                    }
				}

				//Tell the valueManager to tell the ValueScripts to show these previews.
				valueManager.instance.setPreviews(ref previewModifiers);
			}
		}
	}

	/*
	 * Called by an event from the swipe script.
	 * This resets the preview on the values.
	 */
	public void onSwipePreviewReset(){
		previewModifiers.Clear ();
		resetResultPreviews ();
	}

	public void PreviewAddtionalChoices(int choiceNr){
		switch (choiceNr) {
		case 0:

			AdditionalChoice_0_Preview();

			break;
		case 1:

			AdditionalChoice_1_Preview();

			break;
		default:
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" is not configured in 'EventScript'.");
			break;	
		}
	}

	//Computation logic for executing a result.
	//Depending on the configuration of the card the corresponding results are selected.
	void computeResult(result res){

        ComputeResultTypeDependant(res);

        executeExtraChanges(changeExtrasOnCardDespawn);

        foreach (resultModifier rm in  changeValueOnCardDespawn) {
            executeValueChange(rm);
		}

        //foreach(Inventory_ChangeItem.itemModifier im in changeItemOnCardDespawn)
        //{
        //    Inventory_ChangeItem.executeItemChange(im);
        //}
			
		OnCardDespawn.Invoke ();
	}

    public static void ComputeResultTypeDependant(result res, bool actualizeFollowUpCard = true)
    {
        switch (res.resultType)
        {
            case resultTypes.simple:
                //If the result is configured as 'simple' just execute the value modifiers.
                executeValueChanges(res.modifiers, actualizeFollowUpCard);
                break;
            case resultTypes.conditional:
                //If the result is configured as 'conditional' validate the conditions and
                //execute the depending modifiers.
                if (AreConditinsForResultMet(res.conditions))
                {
                    executeValueChanges(res.modifiersTrue, actualizeFollowUpCard);
                }
                else
                {
                    executeValueChanges(res.modifiersFalse, actualizeFollowUpCard);
                }
                break;
            case resultTypes.randomConditions:
                //If the result is configured as 'randomConditions':
                //1. Randomize the borders of predefined value-typ dependencies.
                //2. Validate the new conditions.
                //3. Execute outcome dependent value changes.

                float rndCResult = 1f;
                ValueScript v = null;
                foreach (condition c in res.conditions)
                {
                    rndCResult = Random.Range(0f, 1f);
                    v = valueManager.instance.getFirstFittingValue(c.value);

                    if (v != null)
                    {
                        //set the minimum border for the conditon between min and max, 
                        //if the real value is over min, the path 'true' is executed
                        c.valueMin = v.limits.min + rndCResult * (v.limits.max - v.limits.min);
                        c.valueMax = v.limits.max;
                    }
                    else
                    {
                        Debug.LogWarning("Missing value type: " + c.value);
                    }

                }

                if (AreConditinsForResultMet(res.conditions))
                {
                    executeValueChanges(res.modifiersTrue, actualizeFollowUpCard);
                }
                else
                {
                    executeValueChanges(res.modifiersFalse, actualizeFollowUpCard);
                }
                break;
            case resultTypes.random:
                //If the result is configured as 'random':
                //Select randomly a modifier-group out of the defined pool and execute the value changes.
                if (res.randomModifiers.Length != 0)
                {
                    int rndResult = Random.Range(0, res.randomModifiers.Length);
                    executeValueChanges(res.randomModifiers[rndResult], actualizeFollowUpCard);
                }
                else
                {
                    Debug.LogWarning("Missing random results-list");
                }
                break;
            default:
                Debug.LogError("Path not reachable?");
                break;
        }
    }

#region text preview replacement
    //Computation logic for precalculating the change for one value. Needed for the text replacement
    public void computeTextPreview(string direction, valueDefinitions.values v, ref float newValue, ref bool randomIndependent)
    {
        result res = GetResultByDirectionString(direction);
        
        if (res.resultType == resultTypes.simple)
        {

            //If the result is configured as 'simple' just add the value modifiers to list.
            addTextValueChanges(res.modifiers,v, ref  newValue, ref randomIndependent);

        }
        else if (res.resultType == resultTypes.conditional)
        {

            //If the result is configured as 'conditional' validate the conditions and
            //execute the depending modifiers.
            if (AreConditinsForResultMet(res.conditions))
            {
                addTextValueChanges(res.modifiersTrue, v, ref  newValue, ref randomIndependent);
            }
            else
            {
                addTextValueChanges(res.modifiersFalse, v, ref  newValue, ref randomIndependent);
            }

        }
        else if (res.resultType == resultTypes.randomConditions)
        {

            //If the result is configured as 'randomConditions':
            //Value changes are unknown for the preview. Mark them as unknown.
            //Is it possible to preview this correctly? In some circumstances, but I think this is not worth the effort.

            //Add both possibilities, mark them as 'randomIndependent' = false
            //addTextValueChanges(res.modifiersTrue, v, ref change, ref set, ref randomIndependent);
            //addTextValueChanges(res.modifiersFalse, v, ref change, ref set, ref randomIndependent);
            randomIndependent = false;

        }
        else if (res.resultType == resultTypes.random)
        {

            //If the result is configured as 'random':
            //Add all possible value changes for the preview to the list, marked as 'randomIndependent' = false
            /*if (res.randomModifiers.Length != 0)
            {
                for (int i = 0; i < res.randomModifiers.Length; i++)
                {
                    addTextValueChanges(res.randomModifiers[i]);
                }
            }
            else
            {
                Debug.LogWarning("Missing random results-list");
            }*/
            randomIndependent = false;

        }
        else
        {
            Debug.LogError("Path not reachable?");
        }
    }
    public result GetResultByDirectionString(string direction)
    {
        result res = null;
        switch (direction)
        {
            case "left":
                res = Results.resultLeft;
                break;
            case "right":
                res = Results.resultRight;
                break;
            case "up":
                res = Results.resultUp;
                break;
            case "down":
                res = Results.resultDown;
                break;
            case "add0":
                res = Results.additional_choice_0;
                break;
            case "add1":
                res = Results.additional_choice_1;
                break;
            default:
                Debug.LogWarning("The swipe direction '" + direction + "' for the text replacement can not be determined. Possible strings are 'left','right','up','down','add0' or 'add1'");
                return null;
        }
        return res;
    }
    void addTextValueChanges(modifierGroup modsGroup, valueDefinitions.values v, ref float newValue, ref bool randomIndependent)
    {
        //Debug.Log("Result 1 has " + modsGroup.valueChanges.Length + " elements");
        foreach (resultModifier rm in modsGroup.valueChanges)
        {
            if (rm.modifier == v)//only for matching values
            {


                switch (rm.modificationType)
                {
                    case E_ModificationType.add:
                        newValue += rm.valueAdd;
                        break;
                    case E_ModificationType.set:
                        ValueScript vs = valueManager.instance.getFirstFittingValue(rm.modifier);
                        newValue = rm.valueSet - vs.value;
                        break;
                    case E_ModificationType.addRandom:
                    case E_ModificationType.addRandInt:
                    case E_ModificationType.setRandInt:
                    case E_ModificationType.setRandom:
                    default:
                        //change/set is unknown...
                        newValue = 0f;
                        randomIndependent = false;
                        break;
                }
                //Debug.Log(v.ToString() +" "+ newValue.ToString());
            }
        }
    }
#endregion

    //Computation logic for generating a previews of a result.
    //Depending on the configuration of the card the corresponding results are selected.

    List<resultModifierPreview> previewModifiers = new List<resultModifierPreview>(); // List of values which will be modified with additional informations

	void computeResultPreview(result res){

		previewModifiers.Clear ();

		if (res.resultType == resultTypes.simple) {

			//If the result is configured as 'simple' just add the value modifiers to list.
			addPreviewValueChanges (res.modifiers);

		} else if (res.resultType == resultTypes.conditional) {

			//If the result is configured as 'conditional' validate the conditions and
			//execute the depending modifiers.
			if (AreConditinsForResultMet (res.conditions)) {
				addPreviewValueChanges (res.modifiersTrue);
			} else {
				addPreviewValueChanges (res.modifiersFalse);
			}

		} else if (res.resultType == resultTypes.randomConditions) {

			//If the result is configured as 'randomConditions':
			//Value changes are unknown for the preview. Mark them as unknown.
			//Is it possible to preview this correctly? In some circumstances, but I think this is not worth the effort.

			//Add both possibilities, mark them as 'randomIndependent' = false
			addPreviewValueChanges (res.modifiersTrue, false);
			addPreviewValueChanges (res.modifiersFalse, false);

		} else if (res.resultType == resultTypes.random) {

			//If the result is configured as 'random':
			//Add all possible value changes for the preview to the list, marked as 'randomIndependent' = false
			if (res.randomModifiers.Length != 0) {
				for(int i = 0; i<res.randomModifiers.Length; i++){
					addPreviewValueChanges (res.randomModifiers[i]);
				}
			} else {
				Debug.LogWarning ("Missing random results-list");
			}

		} else {
			Debug.LogError ("Path not reachable?");
		}

		//Now all the possible value changes are known.

		//Attention! There can be still duplicates in the list because of randomization.
		//If this happens, the value script itself will detect the setting of duplicates and then setting the outcome to 'randomIndependant' = true

		//Tell the valueManager to tell the ValueScripts to show these previews.
		valueManager.instance.setPreviews(ref previewModifiers);
	}

	//Reset for all value previews.
	void resetResultPreviews(){
		valueManager.instance.clearAllPreviews ();
	}

	//execution of a group of value modifications
	static void executeValueChanges(modifierGroup modsGroup, bool actualizeFollowUpCard)
    {

		//reset the user info
		//InfoDisplay.instance.clearDisplay ();

		foreach (resultModifier rm in  modsGroup.valueChanges) {
            executeValueChange(rm);
		}

        executeExtraChanges(modsGroup.extras);

        //Tell the cardstack the follow up card.
        //Follow up card can be NULL, the cardstack itself checks the cards before spawning.
        if (actualizeFollowUpCard == true)
        {
            CardStack.instance.decreaseFollowUpStackTime();

            if (modsGroup.followUpCard != null)
            {
                int followUpDelay = Random.Range(modsGroup.followUpDelay.min, modsGroup.followUpDelay.max + 1);

                if (followUpDelay == 0)
                {
                    CardStack.instance.followUpCard = modsGroup.followUpCard;
                }
                else
                {
                    CardStack.instance.followUpCard = CardStack.instance.getNextFollowUpCard();
                    CardStack.instance.addToFollowUpStack(modsGroup.followUpCard, followUpDelay);
                }
            }
            else
            {
                CardStack.instance.followUpCard = CardStack.instance.getNextFollowUpCard();
            }
        }

		//show the value changes over the animation (if available)
		//InfoDisplay.instance.startAnimationIfNotEmpty();
	}

    static void executeExtraChanges(C_AdditionalModifiers[] extras)
    {
        foreach (C_AdditionalModifiers addModifier in extras)
        {
            switch (addModifier.targetType)
            {
                case E_ModifierTargetType.item:
                    Inventory_ChangeItem.executeItemChange(addModifier.itemChange);
                    break;
                case E_ModifierTargetType.dictionary:
                    GameDictionary.executeDictionaryResult(addModifier.dictionaryChange);
                    break;
                case E_ModifierTargetType.timeline:
                    if (Timeline.instance != null && addModifier.historyEvent != null)
                    {
                        Timeline.instance.AddHistoryEvent(addModifier.historyEvent);
                    }
                    break;
                case E_ModifierTargetType.gamelog:
                    if (GameLogger.instance != null)
                    {
                        GameLogger.instance.addGameLog(addModifier.newGameLog.text, addModifier.newGameLog.subLogSelection);
                    }
                    break;
                case E_ModifierTargetType.quest:
                    if (Quests.instance != null)
                    {
                        Quests.instance.computeQuestChange(addModifier.questChange);
                    }
                    break;
                default:
                    Debug.LogWarning("Modification for Type '" + addModifier.targetType.ToString() + "' is not implemented yet.");
                    break;
                    
            }
        }
    }

    //execute a single value modification
    public static void executeValueChange(resultModifier rm)
    {
        switch (rm.modificationType)
        {
            case E_ModificationType.add:
                valueManager.instance.changeValue(rm.modifier, rm.valueAdd);
                break;
            case E_ModificationType.set:
                valueManager.instance.setValue(rm.modifier, rm.valueSet);
                break;
            case E_ModificationType.addRandom:
                float randomValueToAdd = Random.Range(rm.rndRangeAdd.min, rm.rndRangeAdd.max);
                valueManager.instance.changeValue(rm.modifier, randomValueToAdd);
                break;
            case E_ModificationType.addRandInt:
                float randomIntValueToAdd = Random.Range(rm.rndRangeAdd.min, rm.rndRangeAdd.max);
                randomIntValueToAdd = Mathf.RoundToInt(randomIntValueToAdd);
                valueManager.instance.changeValue(rm.modifier, randomIntValueToAdd);
                break;
            case E_ModificationType.setRandInt:
                float randomIntValueToSet = Random.Range(rm.rndRangeSet.min, rm.rndRangeSet.max);
                randomIntValueToSet = Mathf.RoundToInt(randomIntValueToSet);
                valueManager.instance.setValue(rm.modifier, randomIntValueToSet);
                break;
            case E_ModificationType.setRandom:
                float randomValueToSet = Random.Range(rm.rndRangeSet.min, rm.rndRangeSet.max);
                valueManager.instance.setValue(rm.modifier, randomValueToSet);
                break;
        }
    }

    //add element preview of a group of value modifications
    void addPreviewValueChanges(modifierGroup modsGroup, bool randomIndependent = true){
		if (modsGroup.valueChanges == null) {
			Debug.LogError ("Can not show preview, modifier group is null.");
			return;
		}

		foreach (resultModifier rm in  modsGroup.valueChanges) {

			resultModifierPreview rmp = new resultModifierPreview ();
			rmp.resultModification = new resultModifier ();

            switch (rm.modificationType)
            {
                case E_ModificationType.add:
                    rmp.resultModification.valueAdd = rm.valueAdd;
                    rmp.modificationIsRandomIndependant = randomIndependent;
                    break;
                case E_ModificationType.set:
                    ValueScript vs = valueManager.instance.getFirstFittingValue(rm.modifier);
                    rmp.resultModification.valueAdd = rm.valueSet - vs.value;
                    rmp.modificationIsRandomIndependant = randomIndependent;
                    break;
                case E_ModificationType.addRandInt:
                case E_ModificationType.setRandInt:
                case E_ModificationType.addRandom:
                case E_ModificationType.setRandom:
                default:
                    rmp.resultModification.valueAdd = 0f;
                    rmp.modificationIsRandomIndependant = false;
                    break;
            }

			rmp.resultModification.modifier = rm.modifier;

			previewModifiers.Add (rmp);
		}
	}

	//check for a set of conditions if everything is met
	public static bool AreConditinsForResultMet(condition[] cond){
		
        //if there are no conditions to fullfill: all conditions are fullfilled
        if(cond == null)
        {
            return true;
        }

		bool conditionOk = true;

		foreach (EventScript.condition c in cond) {
			if (valueManager.instance.getConditionMet (c) == true) {
				//condition is ok.
			} else {
				conditionOk = false;
				break;
			}
		}

		return conditionOk;
	}


	void Awake(){
		//writeTextFields ();
	}
		
	void Start () {
		OnCardSpawn.Invoke ();
        writeTextFields(); //Later text field actualization because of text replacement. At this point the actual card is the actual card.
	}


    [Tooltip("Changes of items/dictionary/quest after the computation of the conditional results. Useful if a item is changed independent of the result.")]
    public C_AdditionalModifiers[] changeExtrasOnCardDespawn;

    [Tooltip("Changes of values after the computation of the conditional results. Useful if a value is changed independent of the result, like 'Age +1'.")]
	public resultModifier[] changeValueOnCardDespawn;


    public mEvent OnCardSpawn;
	public mEvent OnCardDespawn;

	public mEvent OnSwipeLeft;
	public mEvent OnSwipeRight;
    public mEvent OnSwipeUp;
    public mEvent OnSwipeDown;


    public mEvent OnAdditionalChoice_0;
	public mEvent OnAdditionalChoice_1;

}

