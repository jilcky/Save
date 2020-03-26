using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Inventory))]
public class LocalInventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //float labelWidth = EditorGUIUtility.labelWidth;
        //EditorGUIUtility.labelWidth = 100f;

        base.OnInspectorGUI();

       // EditorGUIUtility.labelWidth = labelWidth;
        serializedObject.ApplyModifiedProperties();
    }
}

//Item Modifier Drawer
[CustomPropertyDrawer(typeof(Inventory.C_ItemEvent))]
public class ItemEventDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //don't alter
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y+3f;
        float y1 = y + 18f;
        float y2 = y1 + 18f;
        float y3 = y2 + 18f;
        float w = (position.width);
        float h = 16f;

        // Calculate rects

        var itemRect = new Rect(x, y1, w, h);
        var typeRect = new Rect(x, y2, w, h);
        var eventRect = new Rect(x, y3, w, h);
        
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("eventType"));
         EditorGUI.PropertyField(itemRect, property.FindPropertyRelative("item"));
        EditorGUI.PropertyField(eventRect, property.FindPropertyRelative("OnEvent"), GUIContent.none);

        //don't alter
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        float mSize = 60f;

        mSize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("OnEvent"), GUIContent.none, true);

        return mSize;
    }
}

//Item Modifier Drawer
[CustomPropertyDrawer(typeof(Inventory_ChangeItem.itemModifier))]
public class ItemModifierDrawer : PropertyDrawer
{
    private static GUIStyle s_TempStyle = new GUIStyle();

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //don't alter
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float imgSize = 36f;
        float x = position.x + imgSize;
        float y1 = position.y;
        float y2 = y1 + 18f;
        float w = (position.width - imgSize);
        float h = 16f;
        float w2 = w / 2f;

        //Display of this drawer is dependant of the modification type
        Inventory_ChangeItem.E_ItemModificationType modifierType = (Inventory_ChangeItem.E_ItemModificationType)property.FindPropertyRelative("modificationType").enumValueIndex;

        // Calculate rects
        var imgRect = new Rect(position.x, y1, imgSize, imgSize);
        var itemRect = new Rect(x, y1, w, h);
        var modTypeRect = new Rect(x, y2, w2, h);
        var modRect = new Rect(x + w2, y2, w2, h);


        //draw a preview and the item-selection

        SerializedProperty item = property.FindPropertyRelative("item");
        EditorGUI.PropertyField(itemRect, item, GUIContent.none);
        InventoryItem so = (InventoryItem)item.objectReferenceValue;

        //flickers :(
        /*if (so != null)
        {
            GUI.enabled = false;
            EditorGUI.ObjectField(imgRect, so.image, typeof(Texture2D), true);
            GUI.enabled = true;

        }*/
        //solution from https://forum.unity.com/threads/how-can-i-create-a-texture-sprite-field-in-inspector-like-this.351790/ : 
        if (!(Event.current.type != EventType.Repaint))
        {
            if (so != null && so.image != null)
            {
                //draw a sprite
                Sprite sp = so.image;
                s_TempStyle.normal.background = sp.texture;
                s_TempStyle.Draw(imgRect, GUIContent.none, false, false, false, false);
                //Kudos to ecesis_llc
            }
        }

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        switch (modifierType)
        {
            case Inventory_ChangeItem.E_ItemModificationType.add:
                EditorGUI.PropertyField(modTypeRect, property.FindPropertyRelative("modificationType"), GUIContent.none);
                EditorGUI.PropertyField(modRect, property.FindPropertyRelative("itemAmountAdd"), GUIContent.none);

                break;
            case Inventory_ChangeItem.E_ItemModificationType.set:
                EditorGUI.PropertyField(modTypeRect, property.FindPropertyRelative("modificationType"), GUIContent.none);
                EditorGUI.PropertyField(modRect, property.FindPropertyRelative("itemAmountSet"), GUIContent.none);
                break;
            default:
                //should not be possible...
                break;
        }
        //don't alter
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //Display of this drawer (specially here: the height) is dependant of the modification type
        Inventory_ChangeItem.E_ItemModificationType modifierType = (Inventory_ChangeItem.E_ItemModificationType)property.FindPropertyRelative("modificationType").enumValueIndex;

        float totalHeight = 36f;

        //for future extension:

        /*switch (modifierType)
        {
            case EventScript.E_ItemModificationType.add:
            case EventScript.E_ItemModificationType.set:
                totalHeight = 18f;
                break;
            default:
                totalHeight = 18f;
                break;
        }*/

        return totalHeight;
    }
}




