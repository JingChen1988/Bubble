using UnityEngine;
/// <summary>
/// 摇杆UI
/// </summary>
public class UIJoystick : MonoBehaviour
{
    int radius;
    Transform mTran;
    Transform Joystick;//摇杆对象
    Plane mPlane;//虚拟平面
    Vector3 mLastPos;//位置信息
    Vector2 position;//偏移记录

    public bool isPress;
    public bool AnchorBorder;

    void Start()
    {
        mTran = transform;
        Joystick = mTran.GetChild(0);
        mPlane = new Plane(Vector3.back, Vector3.zero);
        int border = AnchorBorder ? Joystick.GetComponent<UIWidget>().width / 2 : 0;
        radius = GetComponent<UIWidget>().width / 2 - border;

        //监听按下和拖拽事件
        UIEventListener.Get(Joystick.gameObject).onPress = PressJoystick;
        UIEventListener.Get(Joystick.gameObject).onDrag = DragJoystick;
    }

    void PressJoystick(GameObject go, bool pressed)
    {
        isPress = pressed;
        if (pressed)
            mLastPos = UICamera.lastHit.point;
        else
        {
            Joystick.localPosition = Vector3.zero;
            position = Vector2.zero;
        }
    }

    void DragJoystick(GameObject go, Vector2 delta)
    {
        UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
        Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
        float dist = 0f;
        if (mPlane.Raycast(ray, out dist))
        {
            Vector3 currentPos = ray.GetPoint(dist);
            Vector3 offset = currentPos - mLastPos;
            mLastPos = currentPos;

            if (offset.x != 0f || offset.y != 0f)
            {
                offset = Joystick.InverseTransformDirection(offset);
                offset = Joystick.TransformDirection(offset);
            }

            offset.z = 0;
            Joystick.position += offset;

            float length = Joystick.localPosition.magnitude;

            if (length > radius)
                Joystick.localPosition = Vector3.ClampMagnitude(Joystick.localPosition, radius);

            position = new Vector2((Joystick.localPosition.x) / radius, (Joystick.localPosition.y) / radius);
        }
    }

    public Vector2 Offset { get { return position; } }
}
