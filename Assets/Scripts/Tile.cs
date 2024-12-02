using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class Tile : MonoBehaviour
{
    public const string MaterialFolderPath = "Materials/TileMaterials/TileMaterial";
    private const float MinVelocity = 3.0f;

    public int Type = 0; //花色
    public int Id = -1;
    public bool IsExistInView = true; // 是否还存在
    public bool IsActive = true; //是否被遮挡
    public bool IsMove = false;
    public bool Wait2Eliminate = false;

    public Vector3 targetPosition;
    public Vector3 OriginalPosition;


    public void GenerateMaterial()
    {
        if (Type == 0)
        {
            Debug.Log("Warning: Material is null!");
            return;
        }

        // 使用 Shaders/TileShader 创建材质
        Shader shader = Shader.Find("Unlit/TileShader");
        if (shader == null)
        {
            Debug.LogWarning("Warning: Shader not found!");
            return;
        }

        Material material = new Material(shader);

        // 设置材质的 _IsActive 属性
        material.SetFloat("_IsActive", IsActive ? 1.0f : 0.0f);

        // 动态加载纹理，假设材质纹理路径为 Materials/Textures/TileTexture + 对应的Type
        // string texturePath = "Materials/Textures/TileTexture" + Type;
        string texturePath = "Materials/Textures/CubeNum" + Type;
        Texture texture = Resources.Load<Texture>(texturePath);

        if (texture == null)
        {
            Debug.LogWarning("Warning: Could not find texture at " + texturePath);
            return;
        }

        // 将纹理设置到材质的 _MainTex 属性
        material.SetTexture("_MainTex", texture);

        // 将材质应用到当前物体的 Renderer 上
        GetComponent<Renderer>().material = material;
    }


    public void UpdateMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();

        // 确保材质使用的是正确的 Shader
        if (renderer != null && renderer.material.shader.name == "Unlit/TileShader")
        {
            // 获取材质
            Material _material = renderer.material;

            _material.SetFloat("_IsActive", IsActive ? 1.0f : 0.0f);
        }
    }

    public void MoveTileTo(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMaterial();
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            Vector3 velocity = (targetPosition - transform.position) / GlobalManager.TileMoveTime;
            velocity = velocity.magnitude > 1.0f ? velocity : (velocity.normalized * MinVelocity);

            if(Vector3.Dot(targetPosition - OriginalPosition, velocity) < 0 )
            {
                transform.position = targetPosition;
                IsMove = false;
            }
            else
            {
                transform.position += velocity * Time.deltaTime;           
                IsMove = true;
            }
        }
        else //到达目的地
        {
            transform.position = targetPosition;
            IsMove = false;
        }


    }

}
