using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Item Modifier Drawer
[CustomPropertyDrawer(typeof(GameLogger.C_InspectorGameLogEntry))]
public class InspectorGameLogEntryDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float x = position.x;
        float y = position.y;
        float w = position.width;
        //float w2 = position.width / 8f;
        float h = 15f;
        float lineH = 18f;

        SerializedProperty actionProp = property.FindPropertyRelative("dictionaryAction");
        SerializedProperty keyProp = property.FindPropertyRelative("key");
        SerializedProperty valueProp = property.FindPropertyRelative("value");

        var subLogRect = new Rect(x, y, w, h);
        var textRect = new Rect(x, y + lineH, w, h);

        EditorGUI.PropertyField(subLogRect, property.FindPropertyRelative("subLogSelection"));
        EditorGUI.PropertyField(textRect, property.FindPropertyRelative("text"));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 36f;
    }

}

