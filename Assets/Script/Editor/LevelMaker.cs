using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 关卡编辑器
/// </summary>
public class LevelMaker : EditorWindow
{
    static GUISkin Skin;
    static string[] LevelsName;//关卡列表
    int LevelIndex;//关卡索引
    char[][] Tables;//泡泡表格

    Vector2 scrollPosition;//下拉框位置
    int Length;//行数

    bool ActivePanel;//激活选择面板
    Rect PanelRect;//面板位置
    int Row;//选择行数
    int Column;//选择列数

    const int Width = 42;//宽度
    const int Height = 32;//高度
    const int X = 24;//初始X
    const int Y = 32;//初始Y
    const int Space = 32;//间隔

    [MenuItem("Window/Level Maker #`")]
    public static void ShowWin()
    {
        EditorWindow.GetWindowWithRect(typeof(LevelMaker), new Rect(0, 0, 620, 500), true, "关卡编辑器");
        Skin = EditorGUIUtility.Load("EditorSkin.guiskin") as GUISkin;
        LoadLevelFile();
    }

    void OnGUI()
    {
        ShowSetting();
        if (Tables != null)
        {
            ShowBubblePanel();
            ShowBubbeTable();
            ShowBubblePanel();//UI置顶
        }
    }

    void ShowSetting()
    {
        int fileIndex = EditorGUILayout.Popup("【关卡列表】", LevelIndex, LevelsName, GUILayout.Width(300));
        if (fileIndex != 0 && LevelIndex != fileIndex)
        {
            LevelIndex = fileIndex;
            LoadLevel();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("【行数】" + Length, GUILayout.Width(80));
        Length = (int)GUILayout.HorizontalSlider(Length, 1, 100, GUILayout.Width(120));
        if (GUILayout.Button("重置", GUILayout.Width(160), GUILayout.Width(80))) ResetTables();
        if (GUILayout.Button("加载", GUILayout.Width(160), GUILayout.Width(80))) LoadLevel();
        if (GUILayout.Button("保存", GUILayout.Width(160), GUILayout.Width(80))) SaveLevel();
        GUILayout.EndHorizontal();
    }

    void ShowBubbeTable()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(500), GUILayout.Height(400));
        for (int i = 0, len = Tables.Length; i < len; i++)
        {
            GUILayout.BeginHorizontal();
            char[] solts = Tables[i];

            bool odd = i % 2 == 1;
            if (odd) GUILayout.Space(Space);
            for (int j = 0, len2 = solts.Length; j < len2; j++)
            {
                char slot = solts[j];
                if (GUILayout.Button(new GUIContent((j + 1).ToString(), slot.ToString()), Skin.GetStyle("Bubble_" + slot))) OpenPanel(i, j);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    void ShowBubblePanel()
    {
        if (ActivePanel)
        {
            GUI.Box(PanelRect, "");
            GUILayout.BeginArea(PanelRect);
            GUILayout.Button("选择泡泡");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("", "A"), Skin.GetStyle("Bubble_A"))) ChoiseBubble('A');
            if (GUILayout.Button(new GUIContent("", "B"), Skin.GetStyle("Bubble_B"))) ChoiseBubble('B');
            if (GUILayout.Button(new GUIContent("", "C"), Skin.GetStyle("Bubble_C"))) ChoiseBubble('C');
            if (GUILayout.Button(new GUIContent("", "D"), Skin.GetStyle("Bubble_D"))) ChoiseBubble('D');
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("", "E"), Skin.GetStyle("Bubble_E"))) ChoiseBubble('E');
            if (GUILayout.Button(new GUIContent("", "F"), Skin.GetStyle("Bubble_F"))) ChoiseBubble('F');
            if (GUILayout.Button(new GUIContent("", "G"), Skin.GetStyle("Bubble_G"))) ChoiseBubble('G');
            if (GUILayout.Button(new GUIContent("", "-"), Skin.GetStyle("Bubble_-"))) ChoiseBubble('-');
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }

    //打开泡泡选择面板
    void OpenPanel(int row, int column)
    {
        Row = row;
        Column = column;
        ActivePanel = true;
        int x = column * Width + X + (row % 2 == 1 ? Space : 0);
        int y = row * Height + Y;
        PanelRect = new Rect(x, y, Width * 4, Height * 3);
    }

    //选择泡泡
    void ChoiseBubble(char c)
    {
        Tables[Row][Column] = c;
        ActivePanel = false;
    }

    //读取关卡列表
    static void LoadLevelFile()
    {
        string path = Application.dataPath + "/Resources/Data";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] files = dir.GetFiles();
        LevelsName = new string[files.Length / 2 + 1];
        LevelsName[0] = "请选择";

        for (int i = 0, len = files.Length; i < len; i += 2)
        {
            string fileName = files[i].Name;
            LevelsName[i / 2 + 1] = fileName.Substring(0, fileName.IndexOf('.'));
        }

        foreach (string s in LevelsName) Debug.Log(s);
    }

    //加载泡泡表格
    void LoadLevel()
    {
        //加载关卡配置
        TextAsset binAsset = Resources.Load<TextAsset>("Data/" + LevelsName[LevelIndex]);
        string[] lineArray = binAsset.text.Split('\n');
        //读取行数设置
        int RowCount = int.Parse(lineArray[0]);
        Tables = new char[RowCount][];
        //初始化列表
        for (byte i = 0; i < RowCount; i++)
        {
            //偶数行=10，奇数行=9
            int columnCount = (i % 2 == 0 ? 10 : 9);
            Tables[i] = new char[columnCount];
            //读取配置信息，不足空槽补齐
            if (i + 1 < lineArray.Length)
            {
                string columnArray = lineArray[i + 1].Trim();
                for (byte j = 0; j < columnCount; j++)
                    Tables[i][j] = columnArray[j];
            }
            else
            {
                for (byte j = 0; j < columnCount; j++)
                    Tables[i][j] = '-';
            }
        }
        Length = RowCount;
    }

    //保存泡泡表格
    void SaveLevel()
    {
        string path = Application.dataPath + "/Resources/Data/" + LevelsName[LevelIndex] + ".txt";
        StringBuilder line = new StringBuilder();
        for (int i = 0, len = Tables.Length; i < len; i++)
        {
            char[] slots = Tables[i];
            //空行检查
            int emptyCount = 0;
            for (int j = 0, len2 = slots.Length; j < len2; j++)
                if (slots[j] == '-') emptyCount++;
            if (emptyCount == slots.Length) break;
            line.AppendLine();
            //输入行数据
            for (int j = 0, len2 = slots.Length; j < len2; j++)
                line.Append(slots[j]);
        }

        using (StreamWriter file = new StreamWriter(path, false))
        {
            file.Write(Tables.Length);
            file.Write(line);
            file.Flush();
            int count = Tables.Length;
            EditorHelper.ShowMessage(this, string.Format("保存完成 {0}/{1}", (line.Length - (count % 2 == 1 ? count / 2 + 1 : count / 2)) / 9, count));
        }
    }

    //重置表格
    void ResetTables()
    {
        int Rows = Tables.Length;
        if (Rows == Length) return;
        char[][] newTable = new char[Length][];
        if (Rows < Length)
        {
            for (int i = 0, len = newTable.Length; i < len; i++)
            {
                if (Rows > i)
                    newTable[i] = Tables[i];
                else
                {
                    int columnCount = (i % 2 == 0 ? 10 : 9);
                    newTable[i] = new char[columnCount];
                    for (int j = 0; j < columnCount; j++)
                        newTable[i][j] = '-';
                }
            }
        }
        else if (Rows > Length)
        {
            for (int i = 0, len = newTable.Length; i < len; i++)
                newTable[i] = Tables[i];
        }
        Tables = newTable;
        EditorHelper.ShowMessage(this, string.Format("重新生成表 {0} -> {1}", Rows, Length));
    }
}

