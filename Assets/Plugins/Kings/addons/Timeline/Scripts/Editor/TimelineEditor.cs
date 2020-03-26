using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Timeline))]
public class TimelineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Timeline myScript = (Timeline)target;
        if (GUILayout.Button("Reset History Data"))
        {
            myScript.ResetHistoryData();
        }
    }
}





//Item Modifier Drawer
[CustomPropertyDrawer(typeof(Timeline.C_TimelLineEventChange))]
public class TimelLineEventChangeDrawer : PropertyDrawer
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

        SerializedProperty histProp = property.FindPropertyRelative("newHistoryEvent");
        SerializedProperty histAddText = property.FindPropertyRelative("additionalHistoryText");

        var historyEventRect = new Rect(x, y, w, h);
        var historyAddTextRect = new Rect(x, y + lineH, w, h);
        EditorGUI.PropertyField(historyEventRect, histProp);
        EditorGUI.PropertyField(historyAddTextRect, histAddText);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 36f;
    }

}
