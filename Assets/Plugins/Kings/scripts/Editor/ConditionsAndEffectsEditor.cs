using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Drawer for the Results-selection of ConditionsAndResults
[CustomPropertyDrawer(typeof(ConditionsAndEffects.C_Changes))]
public class ConditionsAndResults_C_Result_Drawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //don't alter
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        //show the result type selection
        EditorGUI.PropertyField(position, property.FindPropertyRelative("result"), GUIContent.none, true);
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("result"), GUIContent.none, true);
    }
}

// Drawer for the conditions of ConditionsAndResults
[CustomPropertyDrawer(typeof(ConditionsAndEffects.C_Conditions))]
public class ConditionsAndResults_C_Condition_Drawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, property.FindPropertyRelative("conditions"), new GUIContent(property.displayName), true);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("conditions"), GUIContent.none, true);
    }
}

