using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ValueMath.C_MathOperation))]
public class RangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float x = position.x;
        float y = position.y;
        float w16 = position.width / 16f;
        float h = position.height;


        var typeR = new Rect(x, y, 3 * w16, h);
        var operatorR = new Rect(x + 4 * w16, y, 3 * w16, h);

        var constR = new Rect(x + 7 * w16, y, 10 * w16, h);

        var multR = new Rect(x + 7 * w16, y, 2 * w16, h);
        var multLR = new Rect(x + 9 * w16, y, w16, h);
        var valR = new Rect(x + 10 * w16, y, 6 * w16, h);

        SerializedProperty typeProp = property.FindPropertyRelative("valueType");
        SerializedProperty operatorProp = property.FindPropertyRelative("mathOperator");

        EditorGUI.PropertyField(typeR, typeProp, GUIContent.none);
        EditorGUI.PropertyField(operatorR, operatorProp, GUIContent.none);

        ValueMath.T_MathValueType type = (ValueMath.T_MathValueType)typeProp.enumValueIndex;
        switch (type)
        {
            case ValueMath.T_MathValueType.Value:
                EditorGUI.PropertyField(multR, property.FindPropertyRelative("valueFactor"), GUIContent.none);
                EditorGUI.LabelField(multLR, "*");
                EditorGUI.PropertyField(valR, property.FindPropertyRelative("valueName"), GUIContent.none);
                break;
            case ValueMath.T_MathValueType.Const:
                EditorGUI.PropertyField(constR, property.FindPropertyRelative("constant"), GUIContent.none);
                break;
            default:
                break;
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
    }
}
