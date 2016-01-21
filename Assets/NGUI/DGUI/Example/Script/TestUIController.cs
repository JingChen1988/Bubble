using UnityEngine;
using System.Collections;

public class TestUIController : MonoBehaviour
{
    Transform mTran;
    UIControllerExp UIC;

    void Start()
    {
        mTran = transform;
        UIC = new UIControllerExp(mTran.Find("Container"));
        UIC.SetLayout(UIControllerExp.UILayout.Main);
    }

    void OnGUI()
    {
        if (GUILayout.Button("Main"))
            UIC.SetLayout(UIControllerExp.UILayout.Main);
        else if (GUILayout.Button("Shop"))
            UIC.SetLayout(UIControllerExp.UILayout.Shop);
        else if (GUILayout.Button("Task"))
            UIC.SetLayout(UIControllerExp.UILayout.Task);
        else if (GUILayout.Button("Back"))
            UIC.BackLastLayout();
    }
}

