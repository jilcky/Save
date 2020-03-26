using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayAdMob))]
public class PlayAdMobEditor : Editor {

    SerializedProperty testConfiguration;
    SerializedProperty targetAudience;
    SerializedProperty keyWords;

    SerializedProperty ids;
    SerializedProperty bannerConfiguration;
    SerializedProperty interstitialConfiguration;
    SerializedProperty rewardBasedVideoConfiguration;

    SerializedProperty generalEvents;
    SerializedProperty eventsBanner;
    SerializedProperty eventsInterstitial;
    SerializedProperty eventsRewardBasedVideo;

    bool infoFoldout = false;

    void OnEnable()
    {
        testConfiguration = serializedObject.FindProperty("testConfiguration");
        targetAudience = serializedObject.FindProperty("targetAudience");
        keyWords = serializedObject.FindProperty("keyWords");
        ids = serializedObject.FindProperty("ids");
        bannerConfiguration = serializedObject.FindProperty("bannerConfiguration");
        interstitialConfiguration = serializedObject.FindProperty("interstitialConfiguration");
        rewardBasedVideoConfiguration = serializedObject.FindProperty("rewardBasedVideoConfiguration");
        generalEvents = serializedObject.FindProperty("generalEvents");
        eventsBanner = serializedObject.FindProperty("eventsBanner");
        eventsInterstitial = serializedObject.FindProperty("eventsInterstitial");
        eventsRewardBasedVideo = serializedObject.FindProperty("eventsRewardBasedVideo");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        showScriptField();

        PlayAdMob playAdMob = (PlayAdMob)target;

        if (playAdMob.IsActivated() == false)
        {
            EditorGUILayout.HelpBox(playAdMob.notEnabledMessage, MessageType.Warning);
            EditorGUILayout.HelpBox("The advanced configuration is only available if PlayAdMob is activated.", MessageType.Warning);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Main Configuration", EditorStyles.boldLabel);
        showSerializedElement(ids);
        showSerializedElement(bannerConfiguration);
        showSerializedElement(interstitialConfiguration);
        showSerializedElement(rewardBasedVideoConfiguration);

        showEvents();

        if (playAdMob.IsActivated() == true)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Advanced Configuration", EditorStyles.boldLabel);
            showSerializedElement(testConfiguration);
            showSerializedElement(targetAudience);
            showSerializedElement(keyWords);
        }

        EditorGUILayout.Space();
        infoFoldout = EditorGUILayout.Foldout(infoFoldout, new GUIContent("Info "));
        if (infoFoldout == true)
        {
            EditorGUILayout.HelpBox("PlayAdMob can also be controlled by code or events by calling: \n" +
                "    RequestBanner()\n" +
        "    DestroyBanner()\n" +
        "    RequestInterstitial()\n" +
        "    ShowInterstitial()\n" +
        "    DestroyInterstitial()\n" +
        "    RequestRewardBasedVideo()\n" +
        "    ShowRewardBasedVideo()\n"+
        "    SetGender(bool enable, Gender gender) - not by events\n" +
        "    SetBirthday(bool enable, int day, int month, int year) - not by events\n", MessageType.Info);
            EditorGUILayout.HelpBox("This script is based on \n" +
        "https://github.com/googleads/googleads-mobile-unity/tree/master/samples/HelloWorld", MessageType.Info);
            
        }



        serializedObject.ApplyModifiedProperties();
    }

    void showEvents()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);

        float lw = EditorGUIUtility.labelWidth;

        //some pain to format for showing the tooltip
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        Rect rect = GUILayoutUtility.GetLastRect();

        EditorGUIUtility.labelWidth = rect.width;

        showSerializedElement(generalEvents);
        showSerializedElement(eventsBanner);
        showSerializedElement(eventsInterstitial);
        showSerializedElement(eventsRewardBasedVideo);

        EditorGUIUtility.labelWidth = lw;
    }


    void showSerializedElement(string class1)
    {
        SerializedProperty c1 = serializedObject.FindProperty(class1);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(c1, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
    void showSerializedElement(SerializedProperty c1)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(c1, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
    void showScriptField()
    {
        //show the script field
        serializedObject.Update();
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        GUI.enabled = false;
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties();

    }
}


// Birthday drawer
[CustomPropertyDrawer(typeof(PlayAdMob.C_Birthday))]
public class PlayAdMob_C_Birthday_Drawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        label = EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y;
        float w8 = position.width / 8f;
        float h = position.height;

        // Calculate rects
        var enR = new Rect(x, y, w8, h);
        var dayR = new Rect(x + w8, y, 2 * w8, h);
        var monthR = new Rect(x + 3 * w8, y, 2 * w8, h);
        var yearR = new Rect(x + 5 * w8, y, 3 * w8, h);

        float lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15f;

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(enR, property.FindPropertyRelative("enable"), GUIContent.none);
        EditorGUI.PropertyField(dayR, property.FindPropertyRelative("day"), new GUIContent("d"));
        EditorGUI.PropertyField(monthR, property.FindPropertyRelative("month"), new GUIContent("m"));
        EditorGUI.PropertyField(yearR, property.FindPropertyRelative("year"), new GUIContent("y"));


        EditorGUIUtility.labelWidth = lw;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(PlayAdMob.C_GenderConfig))]
public class PlayAdMob_C_GenderConfig_Drawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        label = EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y;
        float w8 = position.width / 8f;
        float h = position.height;

        // Calculate rects
        var enR = new Rect(x, y, w8, h);
        var genderR = new Rect(x + w8, y, 7 * w8, h);

        float lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15f;

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(enR, property.FindPropertyRelative("enable"), GUIContent.none);
        EditorGUI.PropertyField(genderR, property.FindPropertyRelative("gender"), GUIContent.none);

        EditorGUIUtility.labelWidth = lw;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(PlayAdMob.C_EN_YES_NO))]
public class PlayAdMob_C_EN_YES_NO_Drawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        label = EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y;
        float w8 = position.width / 8f;
        float h = position.height;

        // Calculate rects
        var enR = new Rect(x, y, w8, h);
        var yesNoR = new Rect(x + w8, y, 7 * w8, h);

        float lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15f;

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(enR, property.FindPropertyRelative("enable"), GUIContent.none);
        EditorGUI.PropertyField(yesNoR, property.FindPropertyRelative("yesNo"), GUIContent.none);

        EditorGUIUtility.labelWidth = lw;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

    [CustomPropertyDrawer(typeof(PlayAdMob.C_max_ad_content_rating))]
public class PlayAdMob_C_max_ad_content_rating_Drawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        label = EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y;
        float w8 = position.width / 8f;
        float h = position.height;

        // Calculate rects
        var enR = new Rect(x, y, w8, h);
        var rateR = new Rect(x + w8, y, 7 * w8, h);

        float lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15f;

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(enR, property.FindPropertyRelative("enable"), GUIContent.none);
        EditorGUI.PropertyField(rateR, property.FindPropertyRelative("max_ad_content_rating"), GUIContent.none);

        EditorGUIUtility.labelWidth = lw;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

// Birthday drawer
[CustomPropertyDrawer(typeof(PlayAdMob.mDescribedEvent))]
public class PlayAdMob_mDescribedEvent_Drawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        label = EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y;
        float h = position.height;

        // Calculate rects
        //var helpR = new Rect(0f, y+20f, position.width+x, 20f);
        var eventR = new Rect(0f, y+20f, position.width+x, h + 50f);

        float lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15f;

        //EditorGUI.HelpBox(helpR, property.FindPropertyRelative("description").stringValue, MessageType.Info);

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
