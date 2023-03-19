using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(Inventory_Card_Value_Pair))]
public class Inventory_Cards_CustomDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        float halfWidth = position.width / 2;
        var keyRect = new Rect(position.x, position.y, halfWidth, position.height);
        var valueRect = new Rect(position.x + halfWidth, position.y, halfWidth, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("card"), GUIContent.none);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("amount"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
