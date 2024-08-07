﻿//adapted from https://forum.unity.com/threads/editor-tool-better-scriptableobject-inspector-editing.484393
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Oneiromancer.EditorHelpers.Attributes
{
    //TODO add another attribute for pure classes, and add param to check if parent has this attribute
    [CustomPropertyDrawer(typeof(ExpandableAttribute), true)]
    public class ExpandableAttributeDrawer : PropertyDrawer
    {
        #region Style Setup
        private enum BackgroundStyles { None, HelpBox, Darken, Lighten }
        
        private static bool SHOW_SCRIPT_FIELD = false;
        private static float INNER_SPACING = 6.0f;
        private static float OUTER_SPACING = 4.0f;
        private static BackgroundStyles BACKGROUND_STYLE = BackgroundStyles.HelpBox;
        private static Color DARKEN_COLOUR = new Color(0.0f, 0.0f, 0.0f, 0.2f);
        private static Color LIGHTEN_COLOUR = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        #endregion
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;

            if (!typeof(UnityEngine.Object).IsAssignableFrom(EditorHelper.GetTypeOfProperty(property))) return totalHeight;    //HACK
            
            if (property.objectReferenceValue == null) return totalHeight;
            if (!property.isExpanded) return totalHeight;
 
            SerializedObject targetObject = new SerializedObject(property.objectReferenceValue);
            //if (targetObject == null) return totalHeight;
            SerializedProperty field = targetObject.GetIterator();
 
            field.NextVisible(true);
            if (SHOW_SCRIPT_FIELD)
            {
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
 
            while (field.NextVisible(false))
            {
                totalHeight += EditorGUI.GetPropertyHeight(field, true) + EditorGUIUtility.standardVerticalSpacing;
            }
 
            totalHeight += INNER_SPACING * 2;
            totalHeight += OUTER_SPACING * 2;
 
            return totalHeight;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect fieldRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };

            EditorGUI.PropertyField(fieldRect, property, label, true);
            
            if (!typeof(UnityEngine.Object).IsAssignableFrom(EditorHelper.GetTypeOfProperty(property)))
                return;    //HACK
            
            if (property.objectReferenceValue == null) return;
     
            property.isExpanded = EditorGUI.Foldout(fieldRect, property.isExpanded, GUIContent.none, true);
            if (!property.isExpanded) return;
 
            SerializedObject targetObject = new SerializedObject(property.objectReferenceValue);
            //if (targetObject == null) return;
            
            List<Rect> propertyRects = FormatPropertyRects(fieldRect, targetObject, out Rect bodyRect);
            DrawBackground(bodyRect);
            DrawPropertyFields(propertyRects, targetObject);
            targetObject.ApplyModifiedProperties();
        }

        private List<Rect> FormatPropertyRects(Rect fieldRect, SerializedObject targetObject, out Rect bodyRect)
        {
            List<Rect> propertyRects = new List<Rect>();
            Rect marchingRect = new Rect(fieldRect);
 
            bodyRect = new Rect(fieldRect);
            bodyRect.xMin += EditorGUI.indentLevel * 14;
            bodyRect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
                                                               + OUTER_SPACING;
     
            SerializedProperty field = targetObject.GetIterator();
            field.NextVisible(true);
 
            marchingRect.y += INNER_SPACING + OUTER_SPACING;
 
            if (SHOW_SCRIPT_FIELD)
            {
                propertyRects.Add(marchingRect);
                marchingRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
 
            while (field.NextVisible(false))
            {
                marchingRect.y += marchingRect.height + EditorGUIUtility.standardVerticalSpacing;
                marchingRect.height = EditorGUI.GetPropertyHeight(field, true);
                propertyRects.Add(marchingRect);
            }
 
            marchingRect.y += INNER_SPACING;
            bodyRect.yMax = marchingRect.yMax;
            return propertyRects;
        }

        private void DrawPropertyFields(List<Rect> propertyRects, SerializedObject targetObject)
        {
            EditorGUI.indentLevel++;
            int index = 0;
            SerializedProperty field = targetObject.GetIterator();
            field.NextVisible(true);
 
            if (SHOW_SCRIPT_FIELD)    //Show the disabled script field
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(propertyRects[index], field, true);
                EditorGUI.EndDisabledGroup();
                index++;
            }
            
            //Replacement for "editor.OnInspectorGUI();" so we have more control on how we draw the editor
            while (field.NextVisible(false))
            {
                try
                {
                    EditorGUI.PropertyField(propertyRects[index], field, true);
                }
                catch (StackOverflowException)
                {
                    field.objectReferenceValue = null;
                    Debug.LogError("Detected self-nesting causing a StackOverflowException, avoid using the same " +
                                   "object inside a nested structure.");
                }
                index++;
            }
            EditorGUI.indentLevel--;
        }

        private void DrawBackground(Rect rect)
        {
            switch (BACKGROUND_STYLE) 
            {
                case BackgroundStyles.HelpBox:
                    EditorGUI.HelpBox(rect, "", MessageType.None);
                    break;
                case BackgroundStyles.Darken:
                    EditorGUI.DrawRect(rect, DARKEN_COLOUR);
                    break;
                case BackgroundStyles.Lighten:
                    EditorGUI.DrawRect(rect, LIGHTEN_COLOUR);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}