using UnityEngine;

public class Test : MonoBehaviour
{
    public SlotTable Table;
    public Sprite ss;

    void Start()
    {
        Table = new SlotTable();
        //Camera.main.orthographicSize = Screen.height / 20f;
    }

    void Update()
    {

    }

    void OnGUI()
    {
        if (GUILayout.Button("Load", GUILayout.Width(120), GUILayout.Height(80)))
        {
            Application.LoadLevel(0);
        }
        //PanelRect = GUILayout.Window(0, PanelRect, xx, "My Window");
    }


    Rect PanelRect = new Rect(100, 100, 200, 200);

    void xx(int id)
    {
        if (GUILayout.Button("Hello World"))
            Debug.Log("Got a click");
    }
}