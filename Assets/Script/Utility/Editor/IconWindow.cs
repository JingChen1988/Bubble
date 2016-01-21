using System;
using UnityEditor;
using UnityEngine;

public class IconWindow : EditorWindow
{
    static string[] text;
    [MenuItem("Window/IconWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(IconWindow));
        text = (EditorGUIUtility.Load("IconList.txt") as TextAsset).text.Split("\n"[0]);
    }
    public Vector2 scrollPosition;
    void OnGUI()
    {
        if (text == null) return;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        //鼠标放在按钮上的样式
        foreach (MouseCursor item in Enum.GetValues(typeof(MouseCursor)))
        {
            GUILayout.Button(Enum.GetName(typeof(MouseCursor), item));
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), item);
            GUILayout.Space(10);
        }

        //内置图标
        for (int i = 0; i < text.Length; i += 8)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 8; j++)
            {
                int index = i + j;
                if (index < text.Length)
                {
                    try
                    {
                        //Texture2D texture = EditorGUIUtility.FindTexture(text[index]);
                        //GUIContent content = EditorGUIUtility.IconContent(text[index]);
                        //if (content != null) GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(30));
                        Debug.Log(i.ToString() + ">>" + text[index]);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
}
