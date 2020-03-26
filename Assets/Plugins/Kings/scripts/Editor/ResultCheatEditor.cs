using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResultCheat))]
public class ResultCheatEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ResultCheat myScript = (ResultCheat)target;

        //EditorGUILayout.LabelField("Ingame quest test control", EditorStyles.boldLabel);
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (GUILayout.Button("Cheat!"))
            {
                myScript.addCheats();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Ingame control is only available if game is in play mode.", MessageType.Info);
        }
    }
}
