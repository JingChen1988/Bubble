using UnityEngine;

public class Layer
{
    public static int Bubble;
    public static int BubbleFall;
    public static int Ground;

    [RuntimeInitializeOnLoadMethod]
    public static void InitLayer()
    {
        Bubble = LayerMask.NameToLayer("Bubble");
        BubbleFall = LayerMask.NameToLayer("BubbleFall");
        Ground = LayerMask.NameToLayer("Ground");
    }
}
