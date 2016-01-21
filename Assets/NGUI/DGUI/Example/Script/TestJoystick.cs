using UnityEngine;
using System.Collections;

public class TestJoystick : MonoBehaviour
{
    UIJoystick Joystick;
    UILabel Offset;

    void Start()
    {
        Joystick = transform.Find("Container/Joystick").GetComponent<UIJoystick>();
        Offset = transform.Find("Container/Offset").GetComponent<UILabel>();
    }

    void Update()
    {
        Offset.text = string.Format("X:{0:f1} Y:{1:f1}", Joystick.Offset.x, Joystick.Offset.y);
    }
}
