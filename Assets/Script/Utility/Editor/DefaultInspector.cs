using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityEditor.DefaultAsset))]
public class DefaultInspector : Editor
{
    public override void OnInspectorGUI()
    {
        string path = AssetDatabase.GetAssetPath(target);
        GUI.enabled = true;
        if (path.EndsWith(".unity"))
            GUILayout.Button("我是场景");
        else if (path.EndsWith(""))
            GUILayout.Label("我是文件夹");
    }
}
