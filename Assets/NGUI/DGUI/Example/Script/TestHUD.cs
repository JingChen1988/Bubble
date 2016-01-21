using UnityEngine;
using System.Collections;

public class TestHUD : MonoBehaviour
{
    Transform player;
    Transform target;
    HUD HUD;
    HUDType Type;

    int Select;
    string[] Selections = new string[] { HUDType.Rise.ToString(), HUDType.Jump.ToString(), HUDType.Erasure.ToString(), HUDType.Scale.ToString() };
    HUDType[] Types = new HUDType[] { HUDType.Rise, HUDType.Jump, HUDType.Erasure, HUDType.Scale };
    bool follow;

    const float speed = 8;

    void Start()
    {
        player = GameObject.Find("Man").transform;
        target = player.Find("pivot");
        HUD = GameObject.Find("MainUI/Container/HUD").GetComponent<HUD>();
    }

    void OnGUI()
    {
        GUILayout.Label("  WSAD 上下左右");
        GUILayout.Label("  H    文本");
        GUILayout.Label("  J    文本2");
        GUILayout.Label("  K    图片");
        GUILayout.Label("  L    图片2");
        follow = GUILayout.Toggle(follow, "跟随");
        Select = GUILayout.SelectionGrid(Select, Selections, 1);
        Type = Types[Select];
    }

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.A))
            player.Translate(-speed * Time.deltaTime, 0, 0);
        else if (Input.GetKey(KeyCode.D))
            player.Translate(speed * Time.deltaTime, 0, 0);
        else if (Input.GetKey(KeyCode.W))
            player.Translate(0, speed * Time.deltaTime, 0);
        else if (Input.GetKey(KeyCode.S))
            player.Translate(0, -speed * Time.deltaTime, 0);

        if (Input.GetKeyDown(KeyCode.H))
            HUD.Display(target, Type, "Text1", "+ 1000", follow);
        else if (Input.GetKeyDown(KeyCode.J))
            HUD.Display(target, Type, "Text2", "x 2000", follow);
        else if (Input.GetKeyDown(KeyCode.K))
            HUD.Display(target, Type, "Picture1", "combo", follow);
        else if (Input.GetKeyDown(KeyCode.L))
            HUD.Display(target, Type, "Picture2", "Item", follow);
    }
}
