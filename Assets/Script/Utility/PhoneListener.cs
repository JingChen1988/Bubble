using UnityEngine;

public delegate void PhoneEvent();
/// <summary>
/// 手机事件监听器
/// </summary>
public class PhoneListener : MonoBehaviour
{
    /// <summary>
    /// Back按钮事件
    /// </summary>
    public static PhoneEvent BackEvent;
    /// <summary>
    /// Home按钮事件
    /// </summary>
    public static PhoneEvent HomeEvent;
    /// <summary>
    /// 游戏暂停事件：系统将游戏挂起到后台
    /// </summary>
    public static PhoneEvent PauseEvent;
    /// <summary>
    /// 游戏激活事件：系统回到游戏中
    /// </summary>
    public static PhoneEvent ResumeEvent;
    /// <summary>
    /// 游戏退出事件
    /// </summary>
    public static PhoneEvent ExitEvent;

    void Awake()
    {
        BackEvent = null;
        HomeEvent = null;
        PauseEvent = null;
        ResumeEvent = null;
        ExitEvent = null;
    }

    void Update() { Listening(); }

    //事件监听
    void Listening()
    {
        //Back-退出游戏
        if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape)))
            if (BackEvent != null) BackEvent();

        //Home-保存数据
        if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Home)))
            if (HomeEvent != null) HomeEvent();
    }

    //游戏运行
    void OnApplicationPause(bool isPause)
    {
        if (isPause && PauseEvent != null)
            PauseEvent();//游戏暂停

        if (!isPause && ResumeEvent != null)
            ResumeEvent();//回到游戏
    }

    //程序焦点
    void OnApplicationFocus(bool isFocus)
    {
        if (isFocus) Logger.INFO("得到焦点");  //得到焦点
        else Logger.INFO("失去焦点");  //失去焦点
    }

    //程序退出
    void OnApplicationQuit()
    {
        if (ExitEvent != null) ExitEvent();
    }
}
