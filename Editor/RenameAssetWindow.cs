﻿using UnityEditor;
using UnityEngine;

namespace Oneiromancer.EditorHelpers
{
    public class RenameAssetWindow : EditorWindow
    {
        private Object _asset;
        private string _newName;

        [MenuItem("Assets/Rename Subasset", false, 19)]
        [MenuItem("CONTEXT/ScriptableObject/Rename")]
        private static void OpenWindow()
        {
            var editor = GetWindow<RenameAssetWindow>("Rename");
            if (Selection.activeObject != null && Selection.count == 1)
            {
                editor.InjectObject(Selection.activeObject);
            }
        }

        private void OnGUI()
        {
            _asset = EditorGUILayout.ObjectField("Asset: ", _asset, typeof(Object), false) as Object;
            _newName = EditorGUILayout.TextField("Rename to: ", _newName);
            
            if (GUILayout.Button("Rename"))
            {
                _asset.name = _newName;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void InjectObject(Object obj)
        {
            _asset = obj;
            _newName = obj.name;
        }
    }
}