using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 选项卡UI
/// </summary>
public class TabUI
{
    public string key;
    public List<Tab> Tabs;

    public TabUI()
    {
        Tabs = new List<Tab>();
    }

    //添加选项卡，并注册点击事件
    public void AddTab(string key, Transform tab, GameObject[] panels)
    {
        Tabs.Add(new Tab(key, tab, panels));
        UIEventListener.Get(tab.gameObject).onClick += (sender) => ChoiseTab(key);
    }

    //选择选项卡
    public void ChoiseTab(string key)
    {
        //打开\关闭选项卡面板
        for (int i = 0; i < Tabs.Count; i++)
        {
            Tab tab = Tabs[i];
            if (tab.key == key)
            {
                tab.Open();
                this.key = key;
            }
            else
                tab.Close();
        }
    }

    //为选项卡添加事件
    public void AddEvent(string key, UIEventListener.VoidDelegate tabEvent)
    {
        for (int i = 0; i < Tabs.Count; i++)
        {
            Tab tab = Tabs[i];
            if (tab.key == key)
            {
                UIEventListener.Get(tab.tab).onClick += tabEvent;
                break;
            }
        }
    }

    //选项卡子项
    public class Tab
    {
        public string key;//标识
        public GameObject tab;
        public UISprite Btn;
        public GameObject[] Panels;
        public bool isChoise;

        const string _Open = "_Open";
        const string _Close = "_Close";

        public Tab(string key, Transform tab, GameObject[] panels)
        {
            this.key = key;
            this.tab = tab.gameObject;
            Btn = tab.GetComponent<UISprite>();
            Panels = panels;
        }

        public void Open()
        {
            isChoise = true;
            Btn.spriteName = key + _Open;
            foreach (GameObject panel in Panels) { panel.SetActive(true); }
        }

        public void Close()
        {
            isChoise = false;
            Btn.spriteName = key + _Close;
            foreach (GameObject panel in Panels) { panel.SetActive(false); }
        }
    }
}
