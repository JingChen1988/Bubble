using UnityEngine;
using System.Collections;

public class TestScrollScale : MonoBehaviour
{

    Transform mTran;
    ScrollScale ss;
    UILabel Lab;

    void Start()
    {
        mTran = transform;
        ss = mTran.Find("ScrollView/Center").GetComponent<ScrollScale>();
        Lab = mTran.Find("Item").GetComponent<UILabel>();

        UIEventListener.Get(mTran.Find("Btn_Next").gameObject).onClick = sender => { ss.ChoiseItem(true); };
        UIEventListener.Get(mTran.Find("Btn_Last").gameObject).onClick = sender => { ss.ChoiseItem(false); };

        ss.OnStartItemChange += OnItemChange;
        //ss.OnEndItemChange += OnItemChange;
    }

    void OnItemChange(GameObject sender, int index)
    {
        Lab.text = index.ToString();
    }
}
