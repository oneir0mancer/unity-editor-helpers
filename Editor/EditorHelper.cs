//Adapted from: https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Oneiromancer.EditorHelpers
{
    public static class EditorHelper
    {
        /// Get target object if it is a part of a nested class inside SerializedObject
        public static object GetTargetObjectOfProperty(SerializedProperty prop, Object serializedObject)
        {
            var elements = prop.propertyPath.Split('.');
            object targetObj = serializedObject;
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    targetObj = GetValue_Imp(targetObj, elementName, index);
                }
                else
                {
                    targetObj = GetValue_Imp(targetObj, element);
                }
            }
            return targetObj;
        }
        
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null) return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null) return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null) return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }
        
        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }

        public static Type GetTypeOfProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');
            
            Type targetType = prop.serializedObject.targetObject.GetType();
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    
                    targetType = GetFieldType(targetType, elementName, index);
                }
                else
                {
                    targetType = GetFieldType(targetType, element);
                }
            }
            return targetType;
        }
        
        private static Type GetFieldType(Type source, string name)
        {
            if (source == null) return null;
            while (source != null)
            {
                var f = source.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null) return f.FieldType;

                var p = source.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null) return p.PropertyType;

                source = source.BaseType;
            }
            return null;
        }
        
        private static Type GetFieldType(Type source, string name, int index)
        {
            var enumerable = GetFieldType(source, name);

            if (enumerable.IsGenericType)    //type is List<T>
            {
                return enumerable.GetGenericArguments().Single();
            }
            
            return enumerable.GetElementType();    //type is array T[]
        }
    }
}