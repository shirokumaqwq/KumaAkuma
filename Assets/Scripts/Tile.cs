using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
    public const string MaterialFolderPath = "Materials/CubeMaterials/CubeMaterial";

    public int Type = 0;
    public int Id = -1;
    public bool IsExistInView = true;
    public bool IsActive = true;
    public Vector2Int OriginPos = new Vector2Int(0, 0);


    public void SetMaterial()
    {
        if (Type == 0)
        {
            Debug.Log("Warning: Material is null!");
            return;
        }

        string materialPath = MaterialFolderPath + Type;
        Material material = Resources.Load<Material>(materialPath);

        // 检查材质是否加载成功
        if (material == null)
        {
            Debug.LogWarning("Warning: Could not find material at " + materialPath);
            return;
        }

        // 将材质赋给当前物体
        GetComponent<Renderer>().material = material;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetMaterial();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
