using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeletePlayerPrefs))]
public class DeletePlayerPrefsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DeletePlayerPrefs dpp = (DeletePlayerPrefs)target;
        if (GUILayout.Button("Delete PlayerPrefs"))
        {
            dpp.DeleteAll();
        }
    }
}
