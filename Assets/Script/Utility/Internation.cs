using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 国际化工具
/// </summary>
public static class Internation
{
    /// <summary>
    /// 语言
    /// </summary>
    public enum Language
    {
        Null = 0,
        Chinese = 1,
        English = 2
    }

    public static Language LocalLanguage;//当前语言
    public static Dictionary<string, string> Properties;//语言配置信息

    //根据设置，加载不同语言信息
    public static void LoadLanguage()
    {
        //使用系统默认语言
        if (LocalLanguage == Language.Null) UseDefaultLangugae();

        //读取文件字符
        TextAsset binAsset = Resources.Load("Language/" + LocalLanguage.ToString(), typeof(TextAsset)) as TextAsset;
        string[] lineArray = binAsset.text.Split("\n"[0]);

        if (Properties == null) Properties = new Dictionary<string, string>();
        else Properties.Clear();

        for (int i = 0; i < lineArray.Length; i++)
        {
            string line = lineArray[i];
            if (line.IndexOf('=') >= 0)
            {
                string[] array = line.Split('=');
                Properties.Add(array[0], array[1]);
            }
        }
    }

    //获取对应字符
    public static string GetProperty(string key)
    {
        string property = null;
        if (Properties.ContainsKey(key))
            property = Properties[key];
        else
            Logger.ERROR("Cann't find the key >> [" + key + "], please check the configuration file");
        return property;
    }

    //使用系统默认语言
    static void UseDefaultLangugae()
    {
        string language = Application.systemLanguage.ToString();
        if (language == "Chinese" || language == "ChineseSimplified" || language == "ChineseTraditional")
            LocalLanguage = Language.Chinese;
        else
            LocalLanguage = Language.English;
        Logger.WARM("Use system default language >> [" + LocalLanguage + "]");
    }
}
