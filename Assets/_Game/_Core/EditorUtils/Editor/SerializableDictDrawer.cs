using UnityEditor;
using UnityEngine;

/// <summary>
/// Draws the dictionary and a warning-box if there are duplicate keys.
/// Taken from: https://github.com/upscalebaby/generic-serializable-dictionary/
/// </summary>
[CustomPropertyDrawer(typeof(SerializableDict<,>))]
public class SerializableDictDrawer : PropertyDrawer
{
    static readonly float lineHeight = EditorGUIUtility.singleLineHeight;
    static readonly float vertSpace = EditorGUIUtility.standardVerticalSpacing;
    const float warningBoxHeight = 1.5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Draw list of key/value pairs.
        var list = property.FindPropertyRelative("list");
        EditorGUI.PropertyField(position, list, label, true);

        // Draw key collision warning.
        var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            position.y += EditorGUI.GetPropertyHeight(list, true);
            if (!list.isExpanded)
            {
                position.y += vertSpace;
            }
            position.height = lineHeight * warningBoxHeight;
            position = EditorGUI.IndentedRect(position);
            EditorGUI.HelpBox(position, "Duplicate keys will not be serialized.", MessageType.Warning);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Height of KeyValue list.
        float height = 0f;
        var list = property.FindPropertyRelative("list");
        height += EditorGUI.GetPropertyHeight(list, true);

        // Height of key collision warning.
        bool keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            height += warningBoxHeight * lineHeight;
            if (!list.isExpanded)
            {
                height += vertSpace;
            }
        }
        return height;
    }
}