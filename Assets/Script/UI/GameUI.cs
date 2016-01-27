using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour
{
    Transform UITran;
    CannonCtl Cannon;

    void Start()
    {
        Cannon = CannonCtl.Instance;

        UITran = transform.Find("Main");
        //切换泡泡
        UIEventListener.Get(UITran.Find("BtnSwap").gameObject).onClick = sender => Cannon.SwapBubble();
        //控制炮口
        UIEventListener.Get(UITran.Find("CannonArea").gameObject).onPress = (sender, isPress) => Cannon.ControlMuzzle(isPress);
    }

    void OnGUI()
    {
        if (GUILayout.Button("Load", GUILayout.Width(80), GUILayout.Height(60))) Application.LoadLevel(0);
    }
}
