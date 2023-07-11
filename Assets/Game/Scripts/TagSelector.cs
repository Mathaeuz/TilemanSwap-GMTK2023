using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct TagSelector
{
    public string Value;

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TagSelector))]
    public class _Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var textValues = UnityEditorInternal.InternalEditorUtility.tags;
            var popupValues = new GUIContent[textValues.Length];

            var selectedValue = property.FindPropertyRelative(nameof(Value)).stringValue;
            var index = -1;

            for (int i = 0; i < textValues.Length; i++)
            {
                popupValues[i] = new GUIContent($"{i}: {textValues[i]}");

                if (index == -1 && textValues[i] == selectedValue)
                {
                    index = i;
                }
            }

            index = EditorGUI.Popup(position, label, index, popupValues);
            property.FindPropertyRelative(nameof(Value)).stringValue = textValues[index];
        }
    }
#endif
}
