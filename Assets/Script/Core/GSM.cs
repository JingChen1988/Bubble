using UnityEngine;
using System.Collections;

public class GSM : MonoBehaviour
{

    public SlotTable Table;

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
        if (GUILayout.Button("Load", GUILayout.Width(120), GUILayout.Height(80))) Application.LoadLevel(0);
    }
}
