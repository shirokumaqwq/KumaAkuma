using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class Tile : MonoBehaviour
{
    public const string MaterialFolderPath = "Materials/TileMaterials/TileMaterial";

    public int Type = 0;
    public int Id = -1;
    public bool IsExistInView = true;
    public bool IsActive = true;
    public Vector2Int OriginPos = new Vector2Int(0, 0);


    public void GenerateMaterial()
    {
        if (Type == 0)
        {
            Debug.Log("Warning: Material is null!");
            return;
        }

        // ʹ�� Shaders/TileShader ��������
        Shader shader = Shader.Find("Unlit/TileShader");
        if (shader == null)
        {
            Debug.LogWarning("Warning: Shader not found!");
            return;
        }

        Material material = new Material(shader);

        // ���ò��ʵ� _IsActive ����
        material.SetFloat("_IsActive", IsActive ? 1.0f : 0.0f);

        // ��̬�������������������·��Ϊ Materials/Textures/TileTexture + ��Ӧ��Type
        string texturePath = "Materials/Textures/TileTexture" + Type;
        Texture texture = Resources.Load<Texture>(texturePath);

        if (texture == null)
        {
            Debug.LogWarning("Warning: Could not find texture at " + texturePath);
            return;
        }

        // ���������õ����ʵ� _MainTex ����
        material.SetTexture("_MainTex", texture);

        // ������Ӧ�õ���ǰ����� Renderer ��
        GetComponent<Renderer>().material = material;
    }


    public void UpdateMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();

        // ȷ������ʹ�õ�����ȷ�� Shader
        if (renderer != null && renderer.material.shader.name == "Unlit/TileShader")
        {
            // ��ȡ����
            Material _material = renderer.material;

            _material.SetFloat("_IsActive", IsActive ? 1.0f : 0.0f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMaterial();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
