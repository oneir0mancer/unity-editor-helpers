using System.IO;
using Oneiromancer.EditorHelpers.SearchProviders;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Oneiromancer.EditorHelpers
{
    public class CreateScriptableObjectWindow
    {
        [MenuItem("Assets/Create/Create Scriptable Object", false, -1000)]
        private static void CreateSo()
        {
            if (Selection.activeObject == null) return;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!Directory.Exists(path)) path = Path.GetDirectoryName(path);

            var point = EditorWindow.focusedWindow.position;
            SearchWindow.Open(
                new SearchWindowContext(point.position, 400),
                new TypeSearchProvider<ScriptableObject>((t) => CreateAsset(t, path)));
        }
        
        private static void CreateAsset(System.Type type, string path)
        {
            var so = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(so, $"{path}/{type.Name}.asset");
            EditorGUIUtility.PingObject(so);
        }
    }
}