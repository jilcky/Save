using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(KingsLevelUp.C_XpPerLevel))]
public class C_XpPerLevelDrawer : PropertyDrawer
{

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
        float w6 = position.width / 6f;
        float h = position.height;

        // Calculate rects
        var fromLRect = new Rect(x, y, w6, h);
        var fromRect = new Rect(x + w6, y, 2 * w6, h);
        var costLRect = new Rect(x + 3 * w6, y, w6, h);
        var costRect = new Rect(x + 4 * w6, y, 2 * w6, h);

        EditorGUI.LabelField(fromLRect, "Level");
        EditorGUI.PropertyField(fromRect, property.FindPropertyRelative("fromLevel"), GUIContent.none);
        EditorGUI.PropertyField(costRect, property.FindPropertyRelative("xpCost"), GUIContent.none);
        EditorGUI.LabelField(costLRect, "cost");

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
