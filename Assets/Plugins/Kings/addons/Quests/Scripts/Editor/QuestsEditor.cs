using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quests))]
public class QuestsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        Quests myScript = (Quests)target;

        EditorGUILayout.LabelField("Ingame quest test control", EditorStyles.boldLabel);
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (GUILayout.Button("Set one random quest as active"))
            {
                myScript.SetRandomQuestActive();
            }
            if (GUILayout.Button("Fill active quests"))
            {
                myScript.FillActiveQuests();
            }
            if (GUILayout.Button("Reselect active quests"))
            {
                myScript.ReselectActiveQuests();
            }
            if (GUILayout.Button("Fullfill active quests"))
            {
                myScript.FullfillActiveQuests();
            }
            if (GUILayout.Button("Abort active quests"))
            {
                myScript.AbortActiveQuests();
            }
            if (GUILayout.Button("Reset Questbook"))
            {
                myScript.ResetQuestbook();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Ingame control is only available if game is in play mode.",MessageType.Info);
        }
    }
}

// Drawer for the dictionary changes
[CustomPropertyDrawer(typeof(Quests.C_QuestChange))]
public class QuestChangeDrawer : PropertyDrawer
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

        SerializedProperty actionProp = property.FindPropertyRelative("questAction");
        SerializedProperty questProp = property.FindPropertyRelative("quest");

        var actionRect = new Rect(x, y, w, h);
        var questRect = new Rect(x, y+lineH, w, h);

        EditorGUI.PropertyField(actionRect, actionProp, GUIContent.none);
        EditorGUI.PropertyField(questRect, questProp, GUIContent.none);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 36f;
    }
}