using UnityEngine;
using System.Collections;

public class Destoryer : MonoBehaviour
{
    GameObject mObj;

    void Start()
    {
        mObj = gameObject;
    }

    public void Hiding()
    {
        mObj.SetActive(false);
    }
}
