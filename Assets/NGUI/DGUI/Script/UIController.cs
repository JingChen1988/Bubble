using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// UI布局控制器
/// </summary>
public abstract class UIController
{
    protected List<UIPlayTween> Tweens;//UI动画集合
    public string Layout;//UI布局
    protected List<string> UIQueue;//UI队列
    public static TweenLayer Layer;//UI层级

    protected void InitBase()
    {
        Tweens = new List<UIPlayTween>();
        UIQueue = new List<string>();
        Layer = new TweenLayer();
    }

    /// <summary>
    /// 设置布局UI
    /// </summary>
    /// <param name="layout">布局名称</param>
    /// <param name="isEnter">是否正向</param>
    protected abstract void SetLayout(string layout, bool isEnter);

    //设置布局，并进入队列
    public void SetLayout(string layout)
    {
        bool isAdd = true;
        if (UIQueue.Count > 0) isAdd = layout != UIQueue[UIQueue.Count - 1];

        if (isAdd)
        {
            UIQueue.Add(layout);
            SetLayout(layout, true);
        }

        FilterSameLayout();
    }

    //过滤相邻重复布局
    void FilterSameLayout()
    {
        for (int i = 1, len = UIQueue.Count; i < len; i++)
        {
            string layout = UIQueue[i];
            int index = 0;
            for (int j = i + 2; j < len; j++)
                if (UIQueue[j] == layout)
                {
                    index = j;
                    break;
                }

            //寻找匹配
            if (index > 0)
            {
                int same = 1;
                for (int m = i + 1, n = index + 1; m < index && n < len; m++, n++)
                    if (UIQueue[m] == UIQueue[n]) same++;

                if (same > 1 && i + same == index)
                {
                    UIQueue.RemoveRange(index, same);
                    break;
                }
            }
        }
    }

    //退回到上一个布局
    public string BackLastLayout()
    {
        string layout = null;
        if (UIQueue.Count >= 2)
        {
            layout = UIQueue[UIQueue.Count - 2];
            UIQueue.RemoveAt(UIQueue.Count - 1);
            SetLayout(layout, false);
        }
        return layout;
    }

    #region 显示UI布局
    //显示布局
    protected void ShowUI(UIPlayTween[] uis, UIPlayTween[] uis_ignore)
    {
        for (int i = 0; i < Tweens.Count; i++)
        {
            UIPlayTween ui = Tweens[i];

            bool isIgnore = false;//忽略
            if (uis_ignore != null)
                for (int j = 0, len = uis_ignore.Length; j < len; j++)
                {
                    isIgnore = ui == uis_ignore[j];
                    if (isIgnore) break;
                }

            if (!isIgnore)
            {
                bool isShow = false;
                for (int j = 0; j < uis.Length; j++)
                {
                    isShow = ui == uis[j];
                    if (isShow) break;
                }

                //显示\隐藏UI
                if (isShow) Show(ui);
                else if (!isShow) Hide(ui);
            }
        }
    }

    //显示UI
    public static void Show(UIPlayTween ui)
    {
        ui.playDirection = AnimationOrTween.Direction.Forward;
        ui.Play(true);
    }

    public static void Show(UIPlayTween[] uis)
    {
        for (int i = 0, len = uis.Length; i < len; i++) Show(uis[i]);
    }

    //隐藏UI
    public static void Hide(UIPlayTween ui)
    {
        ui.playDirection = AnimationOrTween.Direction.Reverse;
        ui.Play(true);
    }

    public static void Hide(UIPlayTween[] uis)
    {
        for (int i = 0, len = uis.Length; i < len; i++) Hide(uis[i]);
    }

    //显示层级UI
    public static void ShowLayer(UIPlayTween ui)
    {
        Layer.Show(ui);
    }

    //隐藏层级UI
    public static void HideLayer(UIPlayTween ui)
    {
        Layer.Hide(ui);
    }
    #endregion

    #region UI层级
    public class TweenLayer
    {
        public List<UIPlayTween> Layers;
        public List<LayerState> States;

        public TweenLayer()
        {
            Layers = new List<UIPlayTween>();
            States = new List<LayerState>();
        }

        //显示层级UI
        public void Show(UIPlayTween Tween)
        {
            Layers.Add(Tween);

            int count = States.Count;
            if (count == 0)
            {
                States.Add(LayerState.Open);
                UIController.Show(Tween);
            }
            else
            {
                LayerState last = States[count - 1];
                if (last == LayerState.Close)
                {
                    States.Add(LayerState.Open);
                    UIController.Show(Tween);
                }
                else
                {
                    States.Add(LayerState.Wait);
                }
            }
        }

        //隐藏层级UI
        public void Hide(UIPlayTween Tween)
        {
            for (int i = 0, len = Layers.Count; i < len; i++)
            {
                if (Layers[i] == Tween)
                {
                    UIController.Hide(Tween);
                    States[i] = LayerState.Close;

                    int index = i + 1;
                    if (index < len && States[index] == LayerState.Wait)
                    {
                        States[index] = LayerState.Open;
                        UIController.Show(Layers[index]);
                    }
                    break;
                }
            }
        }
    }

    //层级状态
    public enum LayerState
    {
        Close, Open, Wait
    }
    #endregion

    //动画回调
    protected static void AnimationCallBack(UIPlayTween Tween, EventDelegate.Callback callback)
    {
        if (callback != null)
        {
            List<EventDelegate> callbacks = new List<EventDelegate>();
            callbacks.Add(new EventDelegate(callback));
            Tween.onFinished = callbacks;
        }
        UIController.Show(Tween);
    }

    //动画回调
    public static void AnimationCallBack(GameObject sender, EventDelegate.Callback callback)
    {
        UIPlayTween Tween = null;
        Transform button = sender.transform.Find("Button");
        Tween = button != null ? button.GetComponent<UIPlayTween>() : sender.GetComponent<UIPlayTween>();
        AnimationCallBack(Tween, callback);
    }

    //注册回调
    public static void RegisteredCallBack(GameObject sender, EventDelegate.Callback callback, AudioClip clip = null)
    {
        UIEventListener.Get(sender).onClick = btn => { UIController.AnimationCallBack(btn, callback); if (clip)AudioManager.Play(clip); };
    }
}
