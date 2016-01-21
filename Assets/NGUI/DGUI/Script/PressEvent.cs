using UnityEngine;
/// <summary>
/// 按键下压组件
/// </summary>
public class PressEvent : MonoBehaviour
{
    public int Offset = 4;//下压偏移量
    bool isPress;

    void Start()
    {
        UIEventListener.Get(gameObject).onPress = Press;
    }

    void Press(GameObject gameObject, bool isPressed)
    {
        isPress = isPressed;
        foreach (Transform t in transform)
        {
            UIRect rect = t.GetComponent<UIRect>();
            if (rect)
            {
                if (isPress)
                {
                    rect.bottomAnchor.absolute -= Offset;
                    rect.topAnchor.absolute -= Offset;
                }
                else
                {
                    rect.bottomAnchor.absolute += Offset;
                    rect.topAnchor.absolute += Offset;
                }
            }
        }
    }
}
