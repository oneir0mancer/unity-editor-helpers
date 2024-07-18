﻿using UnityEditor;
using UnityEngine;

namespace Oneiromancer.EditorHelpers.Utils
{
    [CustomPropertyDrawer(typeof(UnityLayer))]
    public class UnityLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            
            SerializedProperty layerIndex = property.FindPropertyRelative("_layerIndex");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (layerIndex != null)
            {
                layerIndex.intValue = EditorGUI.LayerField(position, layerIndex.intValue);
            }
            
            EditorGUI.EndProperty();
        }
    }
}