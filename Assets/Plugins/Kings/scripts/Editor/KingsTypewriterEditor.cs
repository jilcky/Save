using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;


[CustomEditor(typeof(KingsTypewriter))]
public class KingsTypewriterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        KingsTypewriter tw = (KingsTypewriter)target;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Text");
        tw._text = EditorGUILayout.TextArea(tw._text);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("typesPerSecond"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startDelay"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTypeCharacter"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTypeFinished"));

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(tw);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

    }
}
