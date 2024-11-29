using UnityEditor;
using UnityEngine;

public class SaveModifiedMesh : Editor
{
    [MenuItem("GameObject/Save Modified Mesh", false, 10)]
    static void SaveMesh()
    {
        // ��ȡѡ�е�����
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogError("No GameObject selected.");
            return;
        }

        // ��ȡ������� MeshFilter ���
        MeshFilter meshFilter = selectedObject.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("Selected GameObject does not have a MeshFilter or Mesh.");
            return;
        }

        // ��ȡ��ǰ�� mesh ����
        Mesh originalMesh = meshFilter.sharedMesh;

        // ����һ���µ� Mesh
        Mesh newMesh = new Mesh();

        // ������ mesh �Ķ��㡢���ߡ�UV ��������
        newMesh.vertices = originalMesh.vertices;
        newMesh.normals = originalMesh.normals;
        newMesh.uv = originalMesh.uv;
        newMesh.triangles = originalMesh.triangles;

        // ���ԭʼ mesh ����ɫ��Ҳ���������ɫ
        if (originalMesh.colors.Length > 0)
        {
            newMesh.colors = originalMesh.colors;
        }

        // �����µ� Mesh �ı߽�
        newMesh.RecalculateBounds();

        // �����µ� Mesh ���ļ����У����磺Assets/Resources/Models/ModifiedMesh.asset��
        string assetPath = "Assets/Resources/Models/ModifiedMesh_" + selectedObject.name + ".asset";
        AssetDatabase.CreateAsset(newMesh, assetPath);
        AssetDatabase.SaveAssets();

        // ���·��
        Debug.Log("New Mesh saved at: " + assetPath);
    }
}
