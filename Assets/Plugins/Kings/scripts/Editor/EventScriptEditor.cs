using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventScript))]
public class EventScriptEditor : Editor {
	public override void OnInspectorGUI ()
	{
        serializedObject.Update();

        showScriptField ();

		showSerializedElement ("textFields");
        showSerializedElement ("isDrawable");

		EventScript es = (EventScript)serializedObject.targetObject;

		if (es.isDrawable == true) {
			showSerializedElement ("isHighPriorityCard");

			if (es.isHighPriorityCard == false) {
				showSerializedElement ("cardPropability");
			}
			showSerializedElement ("maxDraws");
            showSerializedElement("redrawBlockCnt");
		}

        //show elements depending on configuration
        

		showSerializedElement ("conditions");
        showSerializedElement("swipeType");
        showSerializedElement("additionalChoices");
		showSerializedElement ("Results");
		showSerializedElement ("changeValueOnCardDespawn");
        //showSerializedElement("changeItemOnCardDespawn");
        showSerializedElement("changeExtrasOnCardDespawn");
        showSerializedElement ("OnCardSpawn");
		showSerializedElement ("OnCardDespawn");

		showSerializedElement ("OnSwipeLeft");
		showSerializedElement ("OnSwipeRight");

        if (es.swipeType == EventScript.E_SwipeType.FourDirection)
        {
            showSerializedElement("OnSwipeUp");
            showSerializedElement("OnSwipeDown");
        }

        GUILayout.Space (15);

        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI ();
    }

	void showSerializedSubElement(string class1, string class2){
		SerializedProperty c1 = serializedObject.FindProperty (class1);
		SerializedProperty c2 = c1.FindPropertyRelative (class2);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(c2, true);
		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}

	void showSerializedElement(string class1){
		SerializedProperty c1 = serializedObject.FindProperty (class1);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(c1, true);
		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}

	void showScriptField(){
		//show the script field
		serializedObject.Update();
		SerializedProperty prop = serializedObject.FindProperty("m_Script");
		GUI.enabled = false;
		EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
		GUI.enabled = true;
		serializedObject.ApplyModifiedProperties();

	}
}

// TextFieldDrawer
[CustomPropertyDrawer(typeof(EventScript.eventText))]
public class TextFieldDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{

		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		float scndWidth = 50f;

		// Calculate rects
		var textRect = new Rect(position.x, position.y, (position.width-scndWidth) * 0.98f, position.height);
		var textFieldRect = new Rect(position.x + (position.width-scndWidth)  , position.y, scndWidth , position.height);

		//var textRect = new Rect(position.x, position.y, position.width * 0.88f, position.height);
		//var textFieldRect = new Rect(position.x + position.width*0.9f  , position.y, position.width * 0.1f , position.height);


		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(textRect, property.FindPropertyRelative("textContent"), GUIContent.none);

		if (EventScript.useTextMeshPro == false) {
			EditorGUI.PropertyField (textFieldRect, property.FindPropertyRelative ("textField"), GUIContent.none);
		} else {
			EditorGUI.PropertyField (textFieldRect, property.FindPropertyRelative ("TMProField"), GUIContent.none);
		}


		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}

// TextFieldDrawer2. For downward compability reasons.
[CustomPropertyDrawer(typeof(EventScript.C_Texts))]
public class TextFieldDrawer2 : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y;
        float w2 = position.width / 2f;
        float h = position.height;

        // Calculate rects
        var TextMeshProRect = new Rect(x, y, w2, h);
        var textRect = new Rect(x + w2, y, w2, h);


        if (EventScript.useTextMeshPro == false)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("textField"), GUIContent.none);
        }
        else
        {
            EditorGUI.PropertyField(textRect, property.FindPropertyRelative("textField"), GUIContent.none);
            EditorGUI.PropertyField(TextMeshProRect, property.FindPropertyRelative("TMProField"), GUIContent.none);
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

// Value Modifier Drawer
[CustomPropertyDrawer(typeof(EventScript.resultModifier))]
public class ModifierDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//don't alter
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y;
        float w = position.width;
        float h = 16f;
        float w2 = w / 2f;
        float w3 = w / 3f;

        //Display of this drawer is dependant of the modification type
        EventScript.E_ModificationType modifierType = (EventScript.E_ModificationType)property.FindPropertyRelative("modificationType").enumValueIndex;

        // Calculate rects
        var modTypeRectw1 = new Rect(x, y, w, h);
        var modTypeRectw2 = new Rect(x, y, w2, h);
        var modTypeRectw3 = new Rect(x, y, w3, h);

        var modRectw3 = new Rect(x+w3, y, w3, h);
        var modRectw2 = new Rect(x + w2, y, w2, h);

        var valRect = new Rect(x+2*w3,y, w3, h);
        var valMinRect = new Rect(x, y + +18f, w2, h);
        var valMaxRect = new Rect(x + w2, y + +18f, w2, h);

        var eventR = new Rect(0f, y + 18f, position.width + x, h + 50f);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        switch (modifierType)
        {
            case EventScript.E_ModificationType.add:
                EditorGUI.PropertyField(modTypeRectw3, property.FindPropertyRelative("modificationType"), GUIContent.none);
                EditorGUI.PropertyField(modRectw3, property.FindPropertyRelative("modifier"), GUIContent.none);
                EditorGUI.PropertyField(valRect, property.FindPropertyRelative("valueAdd"), GUIContent.none);
                break;
            case EventScript.E_ModificationType.set:
                EditorGUI.PropertyField(modTypeRectw3, property.FindPropertyRelative("modificationType"), GUIContent.none);
                EditorGUI.PropertyField(modRectw3, property.FindPropertyRelative("modifier"), GUIContent.none);
                EditorGUI.PropertyField(valRect, property.FindPropertyRelative("valueSet"), GUIContent.none);
                break;
            case EventScript.E_ModificationType.addRandInt:
            case EventScript.E_ModificationType.addRandom:
                EditorGUI.PropertyField(modTypeRectw2, property.FindPropertyRelative("modificationType"), GUIContent.none);
                EditorGUI.PropertyField(modRectw2, property.FindPropertyRelative("modifier"), GUIContent.none);
                EditorGUI.PropertyField(valMinRect, property.FindPropertyRelative("rndRangeAdd").FindPropertyRelative("min"), GUIContent.none);
                EditorGUI.PropertyField(valMaxRect, property.FindPropertyRelative("rndRangeAdd").FindPropertyRelative("max"), GUIContent.none);
                break;
            case EventScript.E_ModificationType.setRandInt:
            case EventScript.E_ModificationType.setRandom:
                EditorGUI.PropertyField(modTypeRectw2, property.FindPropertyRelative("modificationType"), GUIContent.none);
                EditorGUI.PropertyField(modRectw2, property.FindPropertyRelative("modifier"), GUIContent.none);
                EditorGUI.PropertyField(valMinRect, property.FindPropertyRelative("rndRangeSet").FindPropertyRelative("min"), GUIContent.none);
                EditorGUI.PropertyField(valMaxRect, property.FindPropertyRelative("rndRangeSet").FindPropertyRelative("max"), GUIContent.none);
                break;
            //            case EventScript.E_ModifierType._event:
            //               EditorGUI.PropertyField(modTypeRectw1, property.FindPropertyRelative("modifierType"), GUIContent.none);
            //                EditorGUI.PropertyField(eventR, property.FindPropertyRelative("_event"), GUIContent.none);
            //                break;
            default:
                //should not be possible...
                break;
        }



		//don't alter
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}



    // Modifier Drawer
    [CustomPropertyDrawer(typeof(EventScript.C_ReducedResultModifier))]
    public class ReducedResultModifierDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //don't alter
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var modRect = new Rect(position.x, position.y, position.width * 0.70f, position.height);
            var valRect = new Rect(position.x + position.width * 0.71f, position.y, position.width * 0.29f, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(modRect, property.FindPropertyRelative("modifier"), GUIContent.none);
            EditorGUI.PropertyField(valRect, property.FindPropertyRelative("valueAdd"), GUIContent.none);


            //don't alter
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //Display of this drawer (specially here: the height) is dependant of the modification type
        EventScript.E_ModificationType modifierType = (EventScript.E_ModificationType)property.FindPropertyRelative("modificationType").enumValueIndex;

        float totalHeight = 18f;

        switch (modifierType)
        {
            case EventScript.E_ModificationType.add:
            case EventScript.E_ModificationType.set:
                totalHeight = 18f;
                break;
            case EventScript.E_ModificationType.setRandInt:
            case EventScript.E_ModificationType.addRandInt:
            case EventScript.E_ModificationType.addRandom:
            case EventScript.E_ModificationType.setRandom:
                totalHeight = 36;
                break;
//            case EventScript.E_ModifierType._event:
//                totalHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_event"), label, true) + 18f;
//                break;
            default:
                totalHeight = 18f;
                break;
        }

        return totalHeight;
    }
}

// follow up delay Drawer
[CustomPropertyDrawer(typeof(EventScript.C_intRange))]
public class C_intRangeDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //don't alter
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var minRect = new Rect(position.x, position.y, position.width * 0.5f, position.height);
        var maxRect = new Rect(position.x + position.width * 0.51f, position.y, position.width * 0.5f, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"), GUIContent.none);
        EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), GUIContent.none);


        //don't alter
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
{
     return 18f;
}
}


    // ConditionDrawer
    [CustomPropertyDrawer(typeof(EventScript.condition))]
public class ConditionDrawer : PropertyDrawer
{
    float mySize = 0f;

	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//don't alter
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;


        //.Box(position, "", (GUIStyle)"flow overlay box");
        GUI.Box(position, "");

        float x = position.x+5f;
        float y = position.y+5f;
        float w = position.width-10f;
        float w3 = w / 3f;
        float h = 15f;

        // Calculate rects

        var typeRect = new Rect(x, y, w, h);

        //draw the type selection
        EventScript.E_ConditionType ct = (EventScript.E_ConditionType)property.FindPropertyRelative("type").enumValueIndex;
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);

        y += 20;

        //depending on type selection, draw affected fields
        switch (ct)
        {
            case EventScript.E_ConditionType.standard:
                var modRect = new Rect(x, y, w * 0.58f, h);
                var valminRect = new Rect(x + w * 0.59f, y, w * 0.20f, h);
                var valmaxRect = new Rect(x + w * 0.8f, y, w * 0.20f, h);

                // Draw fields - passs GUIContent.none to each so they are drawn without labels
                EditorGUI.PropertyField(modRect, property.FindPropertyRelative("value"), GUIContent.none);
                EditorGUI.PropertyField(valminRect, property.FindPropertyRelative("valueMin"), GUIContent.none);
                EditorGUI.PropertyField(valmaxRect, property.FindPropertyRelative("valueMax"), GUIContent.none);
                break;
            case EventScript.E_ConditionType.compareValues:
                var lValueRect = new Rect(x, y, w3, h);
                var cmpTypeRect = new Rect(x + w3, y, w3, h);
                var rValueRect = new Rect(x + 2 * w3, y, w3, h);

                // Draw fields - passs GUIContent.none to each so they are drawn without labels
                EditorGUI.PropertyField(lValueRect, property.FindPropertyRelative("value"), GUIContent.none);
                EditorGUI.PropertyField(cmpTypeRect, property.FindPropertyRelative("compareType"), GUIContent.none);
                EditorGUI.PropertyField(rValueRect, property.FindPropertyRelative("rValue"), GUIContent.none);
                break;
            case EventScript.E_ConditionType.items:
                var itemRect = new Rect(x, y, w3, h);
                var itemCmpTypeRect = new Rect(x + w3, y, w3, h);
                var itemCmpValueRect = new Rect(x + 2 * w3, y, w3, h);

                EditorGUI.PropertyField(itemRect, property.FindPropertyRelative("item"), GUIContent.none);
                EditorGUI.PropertyField(itemCmpTypeRect, property.FindPropertyRelative("itemCompareType"), GUIContent.none);
                EditorGUI.PropertyField(itemCmpValueRect, property.FindPropertyRelative("itemCmpValue"), GUIContent.none);
                break;
            case EventScript.E_ConditionType.dictionaryEquals:
                var keyRect = new Rect(x, y, w3, h);
                var cmpRect = new Rect(x + w3, y, w3, h);
                var valueRect = new Rect(x + 2 * w3, y, w3, h);

                EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("gamedictionary_key"), GUIContent.none);
                EditorGUI.LabelField(cmpRect, " k == v:");
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("gamedictionary_comparer"), GUIContent.none);
                break;
        }
		//don't alter
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}

    float getSize(SerializedProperty property)
    {
        mySize = 45f;

        //Crashes. Serialization too deep?
        /*if (property != null)
        {
            SerializedProperty prop = property.FindPropertyRelative("type");

            if (prop != null)
            {
                EventScript.E_ConditionType ct = (EventScript.E_ConditionType)prop.enumValueIndex;
                switch (ct)
                {
                    case EventScript.E_ConditionType.dictionary:
                        return 90f;
                    default:
                        break;
                }
            }
        }*/

        return mySize;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

        return getSize(property);
    }
}


// Drawer for the Results-selection
[CustomPropertyDrawer(typeof(EventScript.result))]
public class ResultDrawer : PropertyDrawer
{

	float mySize = 0f;

	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{

        //show property depending on configuration
        Object parentO = property.serializedObject.targetObject;
        bool show = true;

        if (parentO.GetType() == typeof(EventScript))
        {
            EventScript parent = (EventScript)parentO;
            if ((property.name == "additional_choice_0" || property.name == "additional_choice_1") && parent.additionalChoices == false) show = false;
            if ((property.name == "resultUp" || property.name == "resultDown") && parent.swipeType == EventScript.E_SwipeType.LeftRight) show = false;
        }

        if (show)
        {

            //don't alter
            EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		float startY = position.y;

            //show the result type selection
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, position.height), property.FindPropertyRelative("resultType"), GUIContent.none, true);

            position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("resultType"), GUIContent.none, true);

            //dependent on selection
            EventScript.resultTypes res = (EventScript.resultTypes)property.FindPropertyRelative("resultType").enumValueIndex;
            if (res == EventScript.resultTypes.simple)
            {

                EditorGUI.PropertyField(new Rect(50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative("modifiers"), true);
                position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("modifiers"), GUIContent.none, true);

            }
            else if (res == EventScript.resultTypes.conditional || res == EventScript.resultTypes.randomConditions)
            {

                EditorGUI.PropertyField(new Rect(50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative("conditions"), true);
                position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("conditions"), GUIContent.none, true);

                EditorGUI.PropertyField(new Rect(50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative("modifiersTrue"), true);
                position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("modifiersTrue"), GUIContent.none, true);

                EditorGUI.PropertyField(new Rect(50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative("modifiersFalse"), true);
                position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("modifiersFalse"), GUIContent.none, true);

            }
            else if (res == EventScript.resultTypes.random)
            {
                EditorGUI.PropertyField(new Rect(50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative("randomModifiers"), true);
                position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("randomModifiers"), GUIContent.none, true);
            }

		//draw the events
		//EditorGUI.PropertyField (new Rect (50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative ("OnSwipe"), true); 
		//position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("OnSwipe"),GUIContent.none ,true);

		mySize = position.y - startY;

		//don't alter
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();

        }
        else
        {
            //EditorGUI.LabelField(new Rect(position.x, position.y, position.width, position.height), "");
        }
    }

    float getSize(SerializedProperty property)
    {
        mySize = 0f;

        mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("resultType"), GUIContent.none, true);
        //dependent on selection
        EventScript.resultTypes res = (EventScript.resultTypes)property.FindPropertyRelative("resultType").enumValueIndex;
        if (res == EventScript.resultTypes.simple)
        {

            mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("modifiers"), GUIContent.none, true);

        }
        else if (res == EventScript.resultTypes.conditional || res == EventScript.resultTypes.randomConditions)
        {

            mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("conditions"), GUIContent.none, true);
            mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("modifiersTrue"), GUIContent.none, true);
            mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("modifiersFalse"), GUIContent.none, true);

        }
        else if (res == EventScript.resultTypes.random)
        {
            mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("randomModifiers"), GUIContent.none, true);
        }

        //show property depending on configuration

        Object parentO = property.serializedObject.targetObject;
        if (parentO.GetType() == typeof(EventScript))
        {
            EventScript parent = (EventScript)property.serializedObject.targetObject;

            if ((property.name == "additional_choice_0" || property.name == "additional_choice_1") && parent.additionalChoices == false)
            {
                mySize = 0f;
            }
            if ((property.name == "resultUp" || property.name == "resultDown") && parent.swipeType == EventScript.E_SwipeType.LeftRight)
            {
                mySize = 0f;
            }
        }

        return mySize;
    }

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{

		return getSize (property);
		//return mySize;

	}
		
}

// Drawer for the modifierGroup
[CustomPropertyDrawer(typeof(EventScript.modifierGroup))]
public class modifierGroupDrawer : PropertyDrawer
{

    float mySize = 0f;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        float x = position.x;
        float y = position.y;
        float w = position.width;
        float h = position.height;
        float lineH = 18f;
    
        SerializedProperty vChangesProp = property.FindPropertyRelative("valueChanges");
        SerializedProperty iChangesProp = property.FindPropertyRelative("extras");

        float vh = EditorGUI.GetPropertyHeight(vChangesProp);
        float ih = EditorGUI.GetPropertyHeight(iChangesProp);

        var labelRect = new Rect(x, y, w, 18f);
        var vChangesRect = new Rect(x, y+ lineH, w, vh);
        var iChangesRect = new Rect(x, y+vh+ lineH, w, ih);
        var followUpCardRect = new Rect(x, y+vh+ih+ lineH, w,18f);
        var follupUpCardDelayRect = new Rect(x, y + vh + ih + 2*lineH, w, 18f);

        EditorGUI.LabelField(labelRect, property.displayName, EditorStyles.boldLabel);
        EditorGUI.PropertyField(vChangesRect, vChangesProp, true);
        EditorGUI.PropertyField(iChangesRect, iChangesProp, true);

    //show property depending on configuration
    Object parentO = property.serializedObject.targetObject;
        if (parentO.GetType() == typeof(EventScript))
        {
            //only show follow up card for the eventScript
            EditorGUI.PropertyField(followUpCardRect, property.FindPropertyRelative("followUpCard"), true);
            EditorGUI.PropertyField(follupUpCardDelayRect, property.FindPropertyRelative("followUpDelay"),true);
        }

        EditorGUI.EndProperty();
    }

    float getSize(SerializedProperty property)
    {
        mySize = 18f;

        mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("valueChanges"), GUIContent.none, true);
        mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("extras"), GUIContent.none, true);

        Object parentO = property.serializedObject.targetObject;
        if (parentO.GetType() == typeof(EventScript))
        {
            mySize += 36f; //Space for followUpCard
        }

        return mySize;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return getSize(property);
    }

}


// Drawer for the dictionary changes
[CustomPropertyDrawer(typeof(EventScript.C_AdditionalModifiers))]
public class AdditionalModifiersDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        float x = position.x;
        float y = position.y + 5f;
        float w = position.width-5f;
        //float w2 = position.width / 8f;
        float h = position.height;
        float lineH = 18f;

        GUI.Box(position, "");

        EventScript.E_ModifierTargetType type = (EventScript.E_ModifierTargetType)property.FindPropertyRelative("targetType").enumValueIndex;

        var typeRect = new Rect(x, y, w, lineH);
        SerializedProperty typeProp = property.FindPropertyRelative("targetType");

        EditorGUI.PropertyField(typeRect, typeProp);

        switch (type)
        {
            case EventScript.E_ModifierTargetType.item:
                SerializedProperty itemProp = property.FindPropertyRelative("itemChange");
                float itemH = EditorGUI.GetPropertyHeight(itemProp);
                var itemRect = new Rect(x, y+ lineH, w, itemH);
                EditorGUI.PropertyField(itemRect, itemProp);
                break;
            case EventScript.E_ModifierTargetType.dictionary:
                SerializedProperty dictProp = property.FindPropertyRelative("dictionaryChange");
                float dictH = EditorGUI.GetPropertyHeight(dictProp);
                var dictRect = new Rect(x, y + lineH, w, dictH);
                EditorGUI.PropertyField(dictRect, dictProp);
                break;
            case EventScript.E_ModifierTargetType.quest:
                SerializedProperty questProp = property.FindPropertyRelative("questChange");
                float questH = EditorGUI.GetPropertyHeight(questProp);
                var questRect = new Rect(x, y + lineH, w, questH);
                EditorGUI.PropertyField(questRect, questProp);
                break;
            case EventScript.E_ModifierTargetType.timeline:
                SerializedProperty historyProp = property.FindPropertyRelative("historyEvent");
                float histH = EditorGUI.GetPropertyHeight(historyProp);
                var histRect = new Rect(x, y + lineH, w, histH);
                EditorGUI.PropertyField(histRect, historyProp,true);
                break;
            case EventScript.E_ModifierTargetType.gamelog:
                SerializedProperty gamelogProp = property.FindPropertyRelative("newGameLog");
                float loggerH = EditorGUI.GetPropertyHeight(gamelogProp);
                var loggerRect = new Rect(x, y + lineH, w, loggerH);
                EditorGUI.PropertyField(loggerRect, gamelogProp, true);
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = 24f;

        EventScript.E_ModifierTargetType type = (EventScript.E_ModifierTargetType)property.FindPropertyRelative("targetType").enumValueIndex;

        switch (type)
        {
            case EventScript.E_ModifierTargetType.item:
                SerializedProperty itemProp = property.FindPropertyRelative("itemChange");
                float itemH = EditorGUI.GetPropertyHeight(itemProp);
                h += itemH;
                break;
            case EventScript.E_ModifierTargetType.dictionary:
                SerializedProperty dictProp = property.FindPropertyRelative("dictionaryChange");
                float dictH = EditorGUI.GetPropertyHeight(dictProp);
                h += dictH;
                break;
            case EventScript.E_ModifierTargetType.quest:
                SerializedProperty questProp = property.FindPropertyRelative("questChange");
                float questH = EditorGUI.GetPropertyHeight(questProp);
                h += questH;
                break;
            case EventScript.E_ModifierTargetType.timeline:
                SerializedProperty historyProp = property.FindPropertyRelative("historyEvent");
                float histH = EditorGUI.GetPropertyHeight(historyProp);
                h += histH;
                break;
            case EventScript.E_ModifierTargetType.gamelog:
                SerializedProperty gamelogProp = property.FindPropertyRelative("newGameLog");
                float loggerH = EditorGUI.GetPropertyHeight(gamelogProp);
                h += loggerH;
                break;
        }

        return h;
    }
}

