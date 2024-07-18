﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Oneiromancer.EditorHelpers.SearchProviders
{
    public class TypeSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private readonly Type _type;
        private readonly Action<Type> _callback;

        public TypeSearchProvider(Type type, Action<Type> callback = null)
        {
            _type = type;
            _callback = callback;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> list = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Asset"), 0)
            };
            List<string> groups = new List<string>();
            
            var types = TypeCache.GetTypesDerivedFrom<ScriptableObject>();
            List<Tuple<string, Type>> menuNames = new List<Tuple<string, Type>>();
            foreach (var type in types)
            {
                if (type.IsAbstract || !_type.IsAssignableFrom(type)) continue;
                var assetMenuAttribute = type.GetCustomAttribute<CreateAssetMenuAttribute>();
                if (assetMenuAttribute != null)
                {
                    string menuName = assetMenuAttribute.menuName;
                    if (string.IsNullOrEmpty(assetMenuAttribute.menuName)) menuName = $"EMPTY/{type.Name}";
                    else menuName += $" - ({type.Name})";
                    menuNames.Add(new Tuple<string, Type>(menuName, type));
                }
                else
                {
                    if (!groups.Contains("UNSORTED"))
                    {
                        list.Add(new SearchTreeGroupEntry(new GUIContent("UNSORTED"), 1));
                        groups.Add("UNSORTED");
                    }
                    var entry = new SearchTreeEntry(new GUIContent(type.Name))
                    {
                        level = 2,
                        userData = type,
                    };
                    list.Add(entry);
                }
            }

            menuNames.Sort((x, y) =>
                String.Compare(GetMenuName(x.Item1), GetMenuName(y.Item1), StringComparison.Ordinal));

            foreach (var item in menuNames)
            {
                string menuName = item.Item1;
                string[] entries = menuName.Split('/');
                int indentLevel = entries.Length;
                string groupName = "";
                for (int i = 0; i < entries.Length - 1; i++)
                {
                    groupName += entries[i];
                    if (!groups.Contains(groupName))
                    {
                        list.Add(new SearchTreeGroupEntry(new GUIContent(entries[i]), i + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }
                
                var entry = new SearchTreeEntry(new GUIContent(entries[entries.Length - 1]))
                {
                    level = indentLevel,
                    userData = item.Item2,
                };
                list.Add(entry);
            }
            return list;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Type type = (Type) searchTreeEntry.userData;
            _callback?.Invoke(type);
            return true;
        }

        private string PreprocessAssetMenu(string assetMenu)
        {
            if (!string.IsNullOrEmpty(assetMenu) && assetMenu.StartsWith("ScriptableObjects"))
                assetMenu = assetMenu.Replace("ScriptableObjects/", "");
            return assetMenu;
        }

        private string GetMenuName(string x)
        {
            int idx = x.LastIndexOf('/');
            if (idx < 0) return "";
            return x.Substring(0, idx + 1);
        }
    }

    public class TypeSearchProvider<T> : TypeSearchProvider
    {
        public TypeSearchProvider(Action<Type> callback = null) : base(typeof(T), callback)
        { }
    }
}