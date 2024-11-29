using UnityEditor;
using UnityEngine;

public class ApplyTransformToVerticesEditor : Editor
{
    [MenuItem("GameObject/Apply Transform to Vertices (Modify Asset)", false, 10)]
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

        // Get the mesh from the asset (sharedMesh points to the original asset)
        Mesh mesh = meshFilter.sharedMesh;

        // Get the asset path of the Mesh
        string assetPath = AssetDatabase.GetAssetPath(mesh);

        // Load the Mesh asset directly (this will modify the original asset file)
        Mesh assetMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

        if (assetMesh == null)
        {
            Debug.LogError("Failed to load Mesh asset.");
            return;
        }

        // Apply the GameObject's transform to each vertex
        Vector3[] vertices = assetMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = selectedObject.transform.TransformPoint(vertices[i]);
        }

        // Update the mesh with transformed vertices
        assetMesh.vertices = vertices;
        assetMesh.RecalculateBounds();  // Recalculate bounds to ensure the mesh is correctly rendered

        // Save the modified mesh asset
        EditorUtility.SetDirty(assetMesh); // Mark the mesh as dirty to indicate changes
        AssetDatabase.SaveAssets();  // Save changes to the asset


        Debug.Log("Transform applied to vertices and saved to asset.");
        // Reset the GameObject's transform to identity (no rotation, scale, or position)
        selectedObject.transform.position = Vector3.zero;
        selectedObject.transform.rotation = Quaternion.identity;
        selectedObject.transform.localScale = Vector3.one;
    }

    // Validation to make sure the option appears only if there's a valid selection
    [MenuItem("GameObject/Apply Transform to Vertices (Modify Asset)", true)]
    static bool ValidateApplyTransformToVertices()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<MeshFilter>() != null;
    }
}
