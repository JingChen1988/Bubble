using UnityEngine;
using System.Collections;

public class TestTab : MonoBehaviour
{

    Transform mTran;
    TabUI Tab;
    UILabel Lab;

    string KeyA = "A";
    string KeyB = "B";
    string KeyC = "C";

    void Start()
    {
        mTran = transform;
        Lab = mTran.Find("Container/Label").GetComponent<UILabel>();

        GameObject A = mTran.Find("Container/A").gameObject;
        GameObject B = mTran.Find("Container/B").gameObject;
        GameObject C = mTran.Find("Container/C").gameObject;

        Transform TabA = mTran.Find("Container/TabA");
        Transform TabB = mTran.Find("Container/TabB");
        Transform TabC = mTran.Find("Container/TabC");

        Tab = new TabUI();
        Tab.AddTab(KeyA, TabA, new GameObject[] { A });
        Tab.AddTab(KeyB, TabB, new GameObject[] { B });
        Tab.AddTab(KeyC, TabC, new GameObject[] { C });
        Tab.AddEvent(KeyA, (sender) => { Lab.text = "A"; });
        Tab.AddEvent(KeyB, (sender) => { Lab.text = "B"; });
        Tab.AddEvent(KeyC, (sender) => { Lab.text = "C"; });

        Tab.ChoiseTab(KeyA);
    }
}
