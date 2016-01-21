using UnityEngine;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// 持久化工具类
/// </summary>
public static class Persistence
{
    //持久化文件路径
    static string DataPath = Application.persistentDataPath + "/";
    //配置文件路径
#if UNITY_EDITOR
    static string AssetPath = "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
    static string AssetPath = Application.streamingAssetsPath+ "/";
#endif
    const string DataFile = "data.sav";
    const string OperFile = "OperationSetting";

    #region 通用函数
    /// <summary>
    /// 持久化数据
    /// </summary>
    private static void Save(string fileName, object obj, bool isEncrypt = true)
    {
        CreateXML(DataPath + fileName, SerializeObject(obj, isEncrypt));
    }

    /// <summary>
    /// 加载XML数据
    /// </summary>
    private static object Load(string filePath, System.Type type, bool isEncrypt = true)
    {
        string pXmlizedString = LoadXML(filePath);
        if (pXmlizedString != null) return DeserializeObject(type, pXmlizedString, isEncrypt);
        else return null;
    }

    /// <summary>
    /// 加载XML数据
    /// </summary>
    private static object LoadText(string pXmlizedString, System.Type type)
    {
        return DeserializeObject(type, pXmlizedString, false);
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    private static string LoadAsset(string fileName)
    {
        string path = AssetPath + fileName;
        Logger.INFO("LoadAsset: " + path);
        WWW www = new WWW(path);
        while (!www.isDone)
        {//加载中,阻塞线程
        }
        return www.text;
    }

    /// <summary>
    /// 创建XML文件
    /// </summary>
    private static void CreateXML(string filePath, string dataStr)
    {
        Logger.INFO("Save: " + filePath);
        StreamWriter writer;
        FileInfo t = new FileInfo(filePath);
        if (!t.Exists)
            writer = t.CreateText();
        else
        {
            t.Delete();
            writer = t.CreateText();
        }
        writer.Write(dataStr);
        writer.Close();
        writer.Dispose();
    }

    /// <summary>
    ///加载XML文件
    /// </summary>
    private static string LoadXML(string filePath)
    {
        Logger.INFO("Load: " + filePath);
        StreamReader r = File.OpenText(filePath);
        string info = r.ReadToEnd();
        r.Close();
        r.Dispose();
        return info;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    private static void DeleteDataFile()
    {
        string filePath = DataPath + DataFile;
        Logger.INFO("Delete: " + filePath);
        FileInfo t = new FileInfo(filePath);
        if (t.Exists)
            t.Delete();
    }

    /// <summary>
    /// 序列化对象
    /// </summary>
    private static string SerializeObject(object pObject, bool isEncrypt)
    {
        string XmlizedString = null;
        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(pObject.GetType());
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        if (isEncrypt) XmlizedString = Security.RijndaelEncrypt(XmlizedString);
        return XmlizedString;
    }

    /// <summary>
    /// 反序列化对象
    /// </summary>
    private static object DeserializeObject(System.Type type, string pXmlizedString, bool isEncrypt)
    {
        XmlSerializer xs = new XmlSerializer(type);
        if (isEncrypt) pXmlizedString = Security.RijndaelDecrypt(pXmlizedString);
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        return xs.Deserialize(memoryStream);
    }

    /// <summary>
    /// 将二进制转换为字符串
    /// </summary>
    private static string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);
        return (constructedString);
    }

    /// <summary>
    /// 将字符串转换为二进制
    /// </summary>
    private static byte[] StringToUTF8ByteArray(string pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }
    #endregion

    #region 用户信息
    //存储信息
    public static void SaveData(Data data)
    {
        Save(DataFile, data);
    }

    //加载用户数据
    public static Data LoadData()
    {
        string path = DataPath + DataFile;
        FileInfo file = new FileInfo(path);
        if (!file.Exists) return null;
        return Load(path, typeof(Data)) as Data;
    }
    #endregion

    #region 加载游戏数据
    
    #endregion

    #region 辅助函数
    //获取属性值
    static int GetAttInt(XmlElement ele, string name) { return int.Parse(ele.GetAttribute(name)); }
    static byte GetAttByte(XmlElement ele, string name) { return byte.Parse(ele.GetAttribute(name)); }
    static string GetAttStr(XmlElement ele, string name) { return ele.GetAttribute(name); }
    static float GetAttFloat(XmlElement ele, string name) { return float.Parse(ele.GetAttribute(name)); }
    static int GetItemInt(XmlNodeList list, int index) { return int.Parse(list.Item(index).InnerText); }
    static ushort GetItemShort(XmlNodeList list, int index) { return ushort.Parse(list.Item(index).InnerText); }
    static float GetItemFloat(XmlNodeList list, int index) { return float.Parse(list.Item(index).InnerText); }
    static string GetItemStr(XmlNodeList list, int index) { return list.Item(index).InnerText; }
    static byte GetItemByte(XmlNodeList list, int index) { return byte.Parse(list.Item(index).InnerText); }

    //创建子元素
    static XmlElement CreateElement(XmlDocument xml, string name, string value)
    {
        XmlElement Ele = xml.CreateElement(name);
        Ele.InnerText = value;
        return Ele;
    }
    #endregion
}
