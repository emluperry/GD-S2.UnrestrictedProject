using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(Scene_SceneInfo))]
public class Scene_SceneInfo_CustomDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        float quarterWidth = position.width / 4;
        float eighthWidth = quarterWidth / 2;
        var keyRect = new Rect(position.x, position.y, quarterWidth, position.height);
        var valueRect = new Rect(position.x + quarterWidth, position.y, quarterWidth, position.height);
        var intRect = new Rect(position.x + (2 * quarterWidth), position.y, eighthWidth, position.height);
        var boolTextRect = new Rect(position.x + (5 * eighthWidth), position.y, eighthWidth, position.height);
        var boolRect = new Rect(position.x + (3 * quarterWidth), position.y, eighthWidth, position.height);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("sEnum"), GUIContent.none);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("sceneName"), GUIContent.none);
        EditorGUI.PropertyField(intRect, property.FindPropertyRelative("levelNum"), GUIContent.none);
        EditorGUI.LabelField(boolTextRect, new GUIContent("Pause?"));
        EditorGUI.PropertyField(boolRect, property.FindPropertyRelative("isPausable"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
