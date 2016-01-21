using UnityEngine;
using UnityEditor;
using System.Collections;

public class Optimization : Editor
{
    /// <summary>
    /// 合并对象网格
    /// </summary>
    [MenuItem("Tools/Optimization/CombineMesh %#&t")]
    static void CombineMesh()
    {
        //验证选中物体
        if (Selection.activeGameObject == null) { EditorHelper.ShowDialog1("未选中任何物体"); return; }
        Transform mTran = Selection.activeGameObject.transform;
        //---------------- 先获取材质 -------------------------
        //获取自身和所有子物体中所有MeshRenderer组件
        MeshRenderer[] meshRenderers = mTran.GetComponentsInChildren<MeshRenderer>();
        //验证材质非空
        if (meshRenderers == null || meshRenderers.Length == 0) { EditorHelper.ShowDialog1("该物体无法进行合并"); return; }
        //新建材质球数组
        Material[] mats = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            //生成材质球数组 
            mats[i] = meshRenderers[i].sharedMaterial;
        }
        //---------------- 合并 Mesh -------------------------
        //获取自身和所有子物体中所有MeshFilter组件
        MeshFilter[] meshFilters = mTran.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            //矩阵(Matrix)自身空间坐标的点转换成世界空间坐标的点 
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        //为新的整体新建一个mesh
        mTran.GetComponent<MeshFilter>().mesh = new Mesh();
        //合并Mesh. 第二个false参数, 表示并不合并为一个网格, 而是一个子网格列表
        mTran.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false);
        mTran.gameObject.SetActive(true);

        //为合并后的新Mesh指定材质 ------------------------------
        mTran.GetComponent<MeshRenderer>().sharedMaterials = mats;
    }

    /// <summary>
    /// 变换组件位置归零
    /// </summary>
    [MenuItem("CONTEXT/Transform/Origin")]
    static void BackOrigin(MenuCommand command)
    {
        Transform tran = (Transform)command.context;
        tran.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 变换组件规范化
    /// </summary>
    [MenuItem("CONTEXT/Transform/Normalize")]
    static void Normalize(MenuCommand command)
    {
        Transform tran = (Transform)command.context;
        tran.localRotation = Quaternion.identity;
        tran.localScale = Vector3.one;
    }

    /// <summary>
    /// 变换组件规范化
    /// </summary>
    [MenuItem("Tools/Optimization/PivotCenter")]
    static void PivotCenter(MenuCommand command)
    {
        //验证选中物体
        if (Selection.activeGameObject == null) { EditorHelper.ShowDialog1("未选中任何物体"); return; }
        Transform parent = Selection.activeGameObject.transform;
        Vector3 postion = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;


        Vector3 center = Vector3.zero;
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renders)
        {
            center += child.bounds.center;
        }
        center /= parent.GetComponentsInChildren<Transform>().Length;
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
        {
            bounds.Encapsulate(child.bounds);
        }

        parent.position = postion;
        parent.rotation = rotation;
        parent.localScale = scale;

        foreach (Transform t in parent)
        {
            t.position = t.position - bounds.center;
        }
        parent.transform.position = bounds.center + parent.position;
    }

}
