using UnityEngine;
using System.Collections;

public class UIControllerExp : UIController
{
    public class UILayout
    {
        public const string Main = "Main", Shop = "Shop", Task = "Task";
    }

    public static UIControllerExp UIC;

    UIPlayTween Main;
    UIPlayTween Shop;
    UIPlayTween Task;

    public UIControllerExp(Transform ui)
    {
        InitBase();
        UIC = this;

        Main = ui.Find("Main").GetComponent<UIPlayTween>();
        Shop = ui.Find("Shop").GetComponent<UIPlayTween>();
        Task = ui.Find("Task").GetComponent<UIPlayTween>();

        Tweens.Add(Main);
        Tweens.Add(Shop);
        Tweens.Add(Task);
    }

    //设置布局
    protected override void SetLayout(string layout, bool isEnter)
    {
        if (layout != Layout)
        {
            Layout = layout;
            switch (Layout)
            {
                case UILayout.Main:
                    ShowUI(new UIPlayTween[] { Main },null);
                    break;
                case UILayout.Shop:
                    ShowUI(new UIPlayTween[] { Shop }, null);
                    break;
                case UILayout.Task:
                    ShowUI(new UIPlayTween[] { Task }, null);
                    break;
            }
        }
    }
}