using UnityEngine;

public class GSM : MonoBehaviour
{
    public static GSM Instance;

    SlotTable SlotTable;
    CannonCtl CannonCtl;

    void Start()
    {
        Instance = this;
        SlotTable = SlotTable.Instance;
        CannonCtl = CannonCtl.Instance;
        //忽略碰撞
        Physics2D.IgnoreLayerCollision(Layer.Bubble, Layer.BubbleFall);
        Physics2D.IgnoreLayerCollision(Layer.Bubble, Layer.Ground);
        Physics2D.IgnoreLayerCollision(Layer.BubbleFall, Layer.BubbleFall);

        StartCoroutine(SlotTable.ShowTable());
    }

    public void ShowTableFinish()
    {
        StartCoroutine(CannonCtl.InitShell());
    }
}