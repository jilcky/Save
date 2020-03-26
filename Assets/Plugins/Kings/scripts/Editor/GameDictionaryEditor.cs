using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Drawer for the dictionary changes
[CustomPropertyDrawer(typeof(GameDictionary.C_DictionaryChange))]
public class DictionaryChangeDrawer : PropertyDrawer
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

        var actionRect = new Rect(x, y, w, h);
        //var keyLabelR = new Rect(x, y + lineH, w2, lineH);
        var keyRect = new Rect(x, y + lineH, w, h);
        //var valueLabelR = new Rect(x + 4 * w2, y + lineH, w2, lineH);
        var valueRect = new Rect(x , y + 2*lineH, w, h);

        //EditorGUIUtility.labelWidth = 1;
        EditorGUI.PropertyField(actionRect, actionProp, GUIContent.none);
        //EditorGUI.LabelField(keyLabelR, "k:");
        EditorGUI.PropertyField(keyRect, keyProp);
        //EditorGUI.LabelField(valueLabelR, "v:");
        EditorGUI.PropertyField(valueRect, valueProp);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 54f;
    }
}