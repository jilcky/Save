using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KingsCardStyleList))]
public class KingsCardStyleListEditor : Editor
{

}

[CustomPropertyDrawer(typeof(KingsCardStyleList.C_CardStyleNamePair))]
public class C_CardStyleNamePairDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y;
        float w5 = position.width / 5f;
        float h = position.height;

        // Calculate rects
        //var nameRect = new Rect(x, y, 2*w5, h);
        var execRect = new Rect(x , y, 2*w5, h);
        var styleRect = new Rect(x + 2*w5, y, 3*w5, h);

        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 90f;
        
        //GUILayout.ExpandWidth(false);


        //EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("styleName"), GUIContent.none);
        //EditorGUI.PropertyField(execRect, property.FindPropertyRelative("overwrite"), new GUIContent("overwrite"));
        EditorGUI.PropertyField(styleRect, property.FindPropertyRelative("style"), GUIContent.none);

        EditorGUIUtility.labelWidth = labelWidth;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

