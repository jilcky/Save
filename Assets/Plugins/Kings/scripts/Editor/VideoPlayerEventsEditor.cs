using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(VideoPlayerEvents.mDescribedEvent))]
public class VideoPlayerEvents_mDescribedEvent_Drawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 100;

        float x = position.x;
        float y = position.y;
        float h = position.height;

        // Calculate rects
        //var helpR = new Rect(0f, y+20f, position.width+x, 20f);
        var eventR = new Rect(0f, y + 20f, position.width + x, h + 50f);

        float lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15f;


        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(eventR, property.FindPropertyRelative("mEvent"), GUIContent.none);

        EditorGUIUtility.labelWidth = lw;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("mEvent"), label, true) + 20f;

        return totalHeight;
    }

}
