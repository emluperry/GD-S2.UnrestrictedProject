using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(Scene_ButtonInfo))]
public class Scene_ButtonInfo_CustomDrawer : PropertyDrawer
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
        float thirdWidth = position.width / 3;
        var keyRect = new Rect(position.x, position.y, thirdWidth, position.height);
        var valueRect = new Rect(position.x + thirdWidth, position.y, thirdWidth, position.height);
        var intRect = new Rect(position.x + (2 * thirdWidth), position.y, thirdWidth, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("scene"), GUIContent.none);
        EditorGUI.PropertyField(intRect, property.FindPropertyRelative("levelNum"), GUIContent.none);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("button"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
