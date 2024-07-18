using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Oneiromancer.EditorHelpers.SearchProviders
{
    public class AssetSearchProvider<T>: ScriptableObject, ISearchWindowProvider where T : Object
    {
        private readonly Action<T> _callback;

        public AssetSearchProvider(Action<T> callback = null)
        {
            _callback = callback;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> list = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Find Asset"), 0)
            };
            //List<string> groups = new List<string>();
            
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset == null) continue;
                
                //TODO group by namespace/CreateAssetMenu attribute?
                var entry = new SearchTreeEntry(new GUIContent(asset.name))
                {
                    level = 1,
                    userData = asset,
                };
                list.Add(entry);
            }
            return list;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            T asset = (T) searchTreeEntry.userData;
            _callback?.Invoke(asset);
            return true;
        }
    }
}