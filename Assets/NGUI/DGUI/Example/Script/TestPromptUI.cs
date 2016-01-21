using UnityEngine;
using System.Collections;

public class TestPromptUI : MonoBehaviour
{
    UILabel Lab;

    void Start()
    {
        Lab = transform.Find("Container/Label").GetComponent<UILabel>();
        PromptUI.Init(transform.Find("Container/Prompt"));
    }

    void OnGUI()
    {
        if (GUILayout.Button("Prompt"))
            PromptUI.Show("Prompt", CallBack);
        else if (GUILayout.Button("Choise"))
            PromptUI.Choise("Choise", CallBack);
    }

    public void CallBack()
    {
        Lab.text = "You Show Prompt";
    }

    public void CallBack(bool isYes)
    {
        Lab.text = "You Choise >> " + (isYes ? "Yes" : "No");
    }
}
