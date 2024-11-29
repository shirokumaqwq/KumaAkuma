using UnityEditor;
using UnityEngine;

public class SaveModifiedMesh : Editor
{
    [MenuItem("GameObject/Save Modified Mesh", false, 10)]
    static void SaveMesh()
    {
        // 获取选中的物体
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogError("No GameObject selected.");
            return;
        }

        // 获取该物体的 MeshFilter 组件
        MeshFilter meshFilter = selectedObject.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("Selected GameObject does not have a MeshFilter or Mesh.");
            return;
        }

        // 获取当前的 mesh 数据
        Mesh originalMesh = meshFilter.sharedMesh;

        // 创建一个新的 Mesh
        Mesh newMesh = new Mesh();

        // 设置新 mesh 的顶点、法线、UV 和三角形
        newMesh.vertices = originalMesh.vertices;
        newMesh.normals = originalMesh.normals;
        newMesh.uv = originalMesh.uv;
        newMesh.triangles = originalMesh.triangles;

        // 如果原始 mesh 有颜色，也可以添加颜色
        if (originalMesh.colors.Length > 0)
        {
            newMesh.colors = originalMesh.colors;
        }

        // 计算新的 Mesh 的边界
        newMesh.RecalculateBounds();

        // 保存新的 Mesh 到文件夹中（例如：Assets/Resources/Models/ModifiedMesh.asset）
        string assetPath = "Assets/Resources/Models/ModifiedMesh_" + selectedObject.name + ".asset";
        AssetDatabase.CreateAsset(newMesh, assetPath);
        AssetDatabase.SaveAssets();

        // 输出路径
        Debug.Log("New Mesh saved at: " + assetPath);
    }
}
