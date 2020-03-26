using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Timeline_UIEventList))]

public class Timeline_UIEventListEditorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Timeline_UIEventList myScript = (Timeline_UIEventList)target;
        if (GUILayout.Button("Animate"))
        {
            myScript.AnimateScrollbar();
        }
    }
}
