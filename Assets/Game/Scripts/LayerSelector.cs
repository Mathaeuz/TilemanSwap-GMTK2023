using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct LayerSelector
{
    public int Value;

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LayerSelector))]
    public class _Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var layerNames = UnityEditorInternal.InternalEditorUtility.layers;
            var popupValues = new GUIContent[layerNames.Length];

            var index = property.FindPropertyRelative(nameof(Value)).intValue;

            for (int i = 0; i < layerNames.Length; i++)
            {
                popupValues[i] = new GUIContent($"{i}: {layerNames[i]}");
            }

            index = EditorGUI.Popup(position, label, index, popupValues);
            property.FindPropertyRelative(nameof(Value)).intValue = index;
        }
    }
#endif
}