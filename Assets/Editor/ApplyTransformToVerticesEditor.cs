using UnityEditor;
using UnityEngine;

public class ApplyTransformToVerticesEditor : Editor
{
    [MenuItem("GameObject/Apply Transform to Vertices", false, 10)]
    static void ApplyTransformToVertices()
    {
        // Get the selected GameObject
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogError("No GameObject selected.");
            return;
        }

        // Check if the GameObject has a MeshFilter
        MeshFilter meshFilter = selectedObject.GetComponent<MeshFilter>();

        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("The selected GameObject does not have a MeshFilter or Mesh.");
            return;
        }

        // Get the mesh
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        // Apply the GameObject's transform to each vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = selectedObject.transform.TransformPoint(vertices[i]);
        }

        // Update the mesh with transformed vertices
        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        // Reset the GameObject's transform to identity (no rotation, scale, or position)
        selectedObject.transform.position = Vector3.zero;
        selectedObject.transform.rotation = Quaternion.identity;
        selectedObject.transform.localScale = Vector3.one;

        Debug.Log("Transform applied to vertices, and transform reset.");
    }

    // Validation to make sure the option appears only if there's a valid selection
    [MenuItem("GameObject/Apply Transform to Vertices", true)]
    static bool ValidateApplyTransformToVertices()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<MeshFilter>() != null;
    }
}
