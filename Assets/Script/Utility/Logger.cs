using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// 日志输出工具
/// </summary>
public class Logger : MonoBehaviour
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum Level
    {
        ALL = 0, DEBUG = 1, INFO = 2, WARM = 3, ERROR = 4, FATAL = 5, OFF = 6
    }

    public static Level LogLevelOut = Level.OFF;//日志输出等级
    static List<string> mLines;
    static List<string> mWriteTxt;

    string outpath;//日志路径
    string Info = string.Empty;//日志信息
    static byte CountLimit = 30;//日志数量

    #region 设置参数
    public Level LogLevel = Level.DEBUG;//设置日志等级
    public GUISkin Skin;//日志样式
    public bool isButton;
    #endregion

    void Awake()
    {
        if (enabled)
        {
            mLines = new List<string>();
            mWriteTxt = new List<string>();
            if (outpath == null)
            {
                LogLevelOut = LogLevel;
                outpath = Application.persistentDataPath + "/outLog.txt";
                //清除之前的Log
                if (System.IO.File.Exists(outpath))
                    File.Delete(outpath);
                //注册Log监听
                //Application.RegisterLogCallback(HandleLog);
                Application.logMessageReceived += HandleLog;
            }
        }
    }

    void Update()
    {
        //日志写入文件
        if (mWriteTxt.Count > 0)
        {
            string[] temp = mWriteTxt.ToArray();
            foreach (string t in temp)
            {
                using (StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8))
                    writer.WriteLine(t);
                mWriteTxt.Remove(t);
            }
        }
    }

    //日志输出处理
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        mWriteTxt.Add(logString);
        if (type == LogType.Log)
            LogOut(logString);
        else if (type == LogType.Error || type == LogType.Exception)
        {
            LogOut(logString);
            LogOut(stackTrace);
        }
    }

    //将信息保存起来，用于输出
    static public void LogOut(params object[] objs)
    {
        StringBuilder text = new StringBuilder(objs[0].ToString());
        for (int i = 1; i < objs.Length; ++i)
            text.Append(", " + objs[i].ToString());
        if (Application.isPlaying)
        {
            if (mLines.Count > CountLimit)
                mLines.RemoveAt(0);
            mLines.Add(text.ToString());
        }
    }

    //清空日志信息
    static public void ClearLog()
    {
        mLines.Clear();
        mWriteTxt.Clear();
    }

    //日志信息显示到UI
    void OnGUI()
    {
        if (Skin != null && GUI.skin != Skin) GUI.skin = Skin;//设置皮肤
        //显示详细信息
        if (Info != string.Empty)
        {
            GUILayout.TextArea(Info, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height * .9f));
            if (GUILayout.Button(">>>Back<<<")) Info = string.Empty;
            return;
        }

        //按钮模式
        if (isButton)
        {
            for (int i = 0, imax = mLines.Count; i < imax; ++i)
            {
                if (GUILayout.Button(mLines[i]))
                {
                    Info = mLines[i];
                    break;
                }
            }
        }
        else
        {//文本模式
            for (int i = 0, imax = mLines.Count; i < imax; ++i)
                GUILayout.Label(mLines[i]);
        }
    }

    #region 日志输出-类函数
    public static void Log(object message, Level level) { if (level >= LogLevelOut) UnityEngine.Debug.Log(level + " : " + message); }
    public static void Log(object message) { Log(message, Level.DEBUG); }
    public static void Debug(object message) { Log(message, Level.DEBUG); }
    public static void INFO(object message) { Log(message, Level.INFO); }
    public static void WARM(object message) { Log(message, Level.WARM); }
    public static void ERROR(object message) { Log(message, Level.ERROR); }
    public static void FATAL(object message) { Log(message, Level.FATAL); }
    #endregion
}