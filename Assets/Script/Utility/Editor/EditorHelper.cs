using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorHelper
{
    //显示对话框
    public static bool ShowDialog1(string msg) { return EditorUtility.DisplayDialog("提示", msg, "确定"); }
    public static bool ShowDialog2(string msg) { return EditorUtility.DisplayDialog("提示", msg, "确定", "取消"); }
    public static int ShowDialog3(string msg) { return EditorUtility.DisplayDialogComplex("提示", msg, "确定", "取消", "退出"); }
    //显示消息
    public static void ShowMessage(EditorWindow window, string message) { window.ShowNotification(new GUIContent(message)); }
}
