using Oneiromancer.EditorHelpers.SearchProviders;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Oneiromancer.EditorHelpers.Attributes
{
    [CustomPropertyDrawer(typeof(NestedAttribute))]
    public class NestedAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) + 25f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var rect = new Rect(position.x, position.y, position.width, 20);
            EditorGUI.PropertyField(rect, property, new GUIContent(property.displayName));
            var valueHeight = EditorGUI.GetPropertyHeight(property, true);
            rect.y += valueHeight;

            if (property.objectReferenceValue == null)
            {
                var btnRect = new Rect(rect.x + 0.5f * rect.width, rect.y, 0.5f * rect.width, 20);
                if (GUI.Button(btnRect, "Add Nested"))
                {
                    rect.y += 10f;
                    var type = EditorHelper.GetTypeOfProperty(property);
                    Debug.Log(type);
                    SearchWindow.Open(
                        new SearchWindowContext(GUIUtility.GUIToScreenPoint(rect.center), rect.width),
                        new TypeSearchProvider(type, (t) => SetNestedObject(property, t)));
                }
            }
            else if (AssetDatabase.IsSubAsset(property.objectReferenceValue))
            {
                float width = 0.25f * rect.width;
                var btnRect = new Rect(rect.x + 0.5f * rect.width, rect.y, width, 20);
                if (GUI.Button(btnRect, "Clear"))
                {
                    property.objectReferenceValue = null;
                }
                
                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                btnRect = new Rect(rect.x + 0.75f * rect.width, rect.y, width, 20);
                if (GUI.Button(btnRect, "Delete Nested"))
                {
                    AssetDatabase.RemoveObjectFromAsset(property.objectReferenceValue);
                    Object.DestroyImmediate(property.objectReferenceValue, true);
                    property.objectReferenceValue = null;
                    SaveAssets();
                }
                GUI.backgroundColor = oldColor;
            }

            EditorGUI.EndProperty();
        }
        
        private void SetNestedObject(SerializedProperty property, System.Type type)
        {
            var newProperty = ScriptableObject.CreateInstance(type);
            newProperty.name = type.Name;
            AddObjectToAsset(property, newProperty);
        }
        
        private void AddObjectToAsset(SerializedProperty property, ScriptableObject newProperty)
        {
            AssetDatabase.AddObjectToAsset(newProperty, property.serializedObject.targetObject);
            
            property.objectReferenceValue = newProperty;
            property.serializedObject.ApplyModifiedProperties();
            SaveAssets();
        }
        
        private void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}