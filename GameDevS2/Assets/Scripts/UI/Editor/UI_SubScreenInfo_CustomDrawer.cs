using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(UI_SubScreenInfo))]
public class UI_SubScreenInfo_CustomDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        float quarterWidth = position.width / 4;
        var valueRect = new Rect(position.x, position.y, quarterWidth, position.height);
        var keyRect = new Rect(position.x + quarterWidth, position.y, quarterWidth * 3, position.height);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("button"), GUIContent.none);
        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("screen"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
