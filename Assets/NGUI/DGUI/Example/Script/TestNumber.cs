using UnityEngine;
using System.Collections;

public class TestNumber : MonoBehaviour
{
    NumberAtlas Number;
    string text = "";

    int position;
    string[] Postions = new string[] { "左", "中", "右" };
    bool ShowAll;

    void Start()
    {
        Number = GameObject.Find("Number").GetComponent<NumberAtlas>();
    }

    void OnGUI()
    {
        text = GUILayout.TextField(text);
        if (GUILayout.Button("Show"))
            Number.SetNumber(int.Parse(text));
        position = GUILayout.SelectionGrid(position, Postions, 3);
        if ((int)Number.Anchor != position)
        {
            Number.Anchor = (NumberAtlas.AnchorNumber)position;
            Number.SetNumber(int.Parse(text));
        }
        bool Show = GUILayout.Toggle(ShowAll, "ShowAll");
        if (Show != ShowAll)
        {
            ShowAll = Show;
            Number.AllShow = ShowAll;
            Number.SetNumber(int.Parse(text));
        }
    }
}
