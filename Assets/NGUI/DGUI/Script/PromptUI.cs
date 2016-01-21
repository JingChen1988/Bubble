using UnityEngine;
using System;
/// <summary>
/// 提示框UI
/// </summary>
public class PromptUI
{
    static Transform UI;//UI对象
    static UIPlayTween Tween;//组件动画
    static UILabel PromptLab;//提示文本
    static GameObject OkBtn;//确认按钮
    static GameObject YesBtn;//肯定按钮
    static GameObject NoBtn;//否定按钮

    public static void Init(Transform ui)
    {
        UI = ui;
        Tween = UI.GetComponent<UIPlayTween>();
        PromptLab = UI.Find("PromptBg/Prompt").GetComponent<UILabel>();
        OkBtn = UI.Find("PromptBg/Ok").gameObject;
        YesBtn = UI.Find("PromptBg/Yes").gameObject;
        NoBtn = UI.Find("PromptBg/No").gameObject;
    }

    //显示提示框
    public static void Show(string prompt, System.Action callBack = null)
    {
        PromptLab.text = prompt;
        OkBtn.SetActive(true);
        YesBtn.SetActive(false);
        NoBtn.SetActive(false);

        UIEventListener.Get(OkBtn).onClick = (sender) => UIController.AnimationCallBack(sender, delegate { Hide(callBack); });
        UIController.Show(Tween);
    }

    //隐藏提示框
    static void Hide(System.Action callBack)
    {
        UIController.Hide(Tween);
        if (callBack != null) callBack();
        //AudioManager.Play(AudioList.UI_Button);
    }

    //显示选择框
    public static void Choise(string prompt, Action<bool> callBack = null)
    {
        PromptLab.text = prompt;
        OkBtn.SetActive(false);
        YesBtn.SetActive(true);
        NoBtn.SetActive(true);

        UIEventListener.Get(YesBtn).onClick = (sender) => UIController.AnimationCallBack(sender, delegate { Choise(true, callBack); });
        UIEventListener.Get(NoBtn).onClick = (sender) => UIController.AnimationCallBack(sender, delegate { Choise(false, callBack); });
        UIController.Show(Tween);
    }

    //触发选择事件
    static void Choise(bool isYes, Action<bool> callBack)
    {
        UIController.Hide(Tween);
        if (callBack != null) callBack(isYes);
        //AudioManager.Play(AudioList.UI_Button);
    }
}
