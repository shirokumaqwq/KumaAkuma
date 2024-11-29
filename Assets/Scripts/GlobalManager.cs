using System.Collections;
using System.Collections.Generic;
//using Unity.Mathematics;
using UnityEngine;



public class GlobalManager : MonoBehaviour
{
    public const int Maxrows = 10;
    public const int Maxcols = 10;
    public const int MaxStackNum = 7;
    public const int MaxTileTypes = 8;
    public const float TileRadius = 0.5f;

    private float ScreenWidth;
    private float ScreenHeight;


    public Camera mainCamera; // 主相机

    public List<GameObject> TileList = new List<GameObject>();
    public LinkedList<GameObject> TileStack = new LinkedList<GameObject>();
    private int ExistNum = 0;


    public Vector2 FindPos(int row, int col)
    {
        return new Vector2(row, col);
    }



    // 创建 Tile 并返回
    public GameObject CreateTile(Vector3 position, int material, int id)
    {
        GameObject tile;
        // Load the prefab from the Resources folder
        GameObject prefab = Resources.Load<GameObject>("Models/StandardBevelCube");

        // Check if the prefab was successfully loaded
        if (prefab != null)
        {
            // Instantiate the prefab in the scene
            tile = Instantiate(prefab);
            tile.name = "StandardBevelCube"; // Optionally, set the name of the instantiated object
            // 旋转 Cube 到指定角度
            //tile.transform.rotation = Quaternion.Euler(8f, 8f, 0f);

            // 设置位置为传入的 Position
            tile.transform.position = position;


            // 为 Cube 添加 Tile.cs 脚本
            Tile tileScript = tile.AddComponent<Tile>();

            // 设置 TileType 为传入的 material 值
            tileScript.Type = material;
            tileScript.Id = id;

            // 你可以根据需求初始化其他属性，默认值会保留
            // tileScript.IsExist = true;  // 默认是 true
            // tileScript.IsActive = true; // 默认是 true

            // 返回这个创建的 GameObject
            return tile;
        }
        else
        {
            Debug.LogError("Prefab not found at Assets/Resources/Models/StandardBevelCube.prefab");
            return null;
        }

    }

    void GenerateTileLists(int level = 1)
    {
        // TODO: level template?
        int tileNum = 0;
        for (int type = 0; type < MaxTileTypes; type++)
        {
            int pairNum = Random.Range(1, 3);
            for (int j = 0; j < pairNum; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    Vector3 randomPosition = new Vector3(
                        Random.Range(0 + 1.0f, ScreenWidth - 1.0f),   // x 范围：0 到 Screen.width
                        Random.Range(2.0f, ScreenHeight - 1.0f),  // y 范围：0 到 Screen.height
                        Random.Range(0f, 10f)            // z 范围：0 到 10
                    );
                    GameObject NewTile = CreateTile(randomPosition, type + 1, tileNum);
                    TileList.Add(NewTile);
                    tileNum++;
                }

            }
        }

        ExistNum = TileList.Count;
        UpdateActiveState();
    }


    // 每当分辨率变化时调用该函数
    public void SetCamera()
    {
        // 获取屏幕的宽高比
        float screenAspect = (float)Screen.width / Screen.height;

        // 设置相机的正交视野大小（orthographic size）
        if (mainCamera.orthographic)
        {
            // 设置正交相机的视野大小：根据屏幕的宽高比调整
            mainCamera.orthographicSize = (screenAspect < 1) ? 5 / screenAspect : 5;

            // 计算视野框的宽度和高度
            ScreenWidth = mainCamera.orthographicSize * 2 * screenAspect;
            ScreenHeight = mainCamera.orthographicSize * 2;

            // 设置相机位置，使得左下角是 (0, 0, -10)
            mainCamera.transform.position = new Vector3(ScreenWidth / 2, ScreenHeight / 2, -10);
            Debug.Log("Screen size: " + ScreenWidth.ToString() + "  " + ScreenHeight.ToString());
        }
        else
        {
            Debug.LogWarning("Camera is not orthographic! Please use an orthographic camera.");
        }
    }


    // 获取鼠标点击下的 Tile ID
    public GameObject GetObjectUnderMouse()
    {
        // 获取鼠标在屏幕上的位置
        Vector3 mousePosition = Input.mousePosition;

        // 将屏幕坐标转换为世界坐标
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldMousePosition.z = 10f; // 设置 z 为10，确保射线沿着相机的 z 轴发射

        // 从相机位置发射射线朝向鼠标位置
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        // 射线检测
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 如果射线碰撞到物体，返回该物体的 GameObject
            return hit.collider.gameObject;
        }

        return null;
    }


    bool IsTilesCovered(GameObject Tile1, GameObject Tile2)
    {
        Vector3 TileCenter1 = Tile1.transform.position;
        Vector3 TileCenter2 = Tile2.transform.position;
        if (Mathf.Abs(TileCenter1.x - TileCenter2.x) < 2 * TileRadius &&
            Mathf.Abs(TileCenter1.y - TileCenter2.y) < 2 * TileRadius &&
            TileCenter1.z > TileCenter2.z)
        {
            return true;
        }
            
        return false;
    }

    // Check all tiles state
    void UpdateActiveState()
    {
        for(int i = 0;i<TileList.Count;i++)
        {
            GameObject Tile = TileList[i];
            if(Tile.GetComponent<Tile>().IsExistInView)
            {
                Tile.GetComponent<Tile>().IsActive = true;
                for (int j = 0;j<TileList.Count;j++)
                {
                    GameObject TempTile = TileList[j];
                    if(TempTile.GetComponent<Tile>().IsExistInView)
                    {
                        if(IsTilesCovered(Tile, TempTile))
                        {
                            Tile.GetComponent<Tile>().IsActive = false;
                            break;
                        }
                    }
                }
                Tile.GetComponent<Tile>().UpdateMaterial();
            }
        }
    }

    void UpdateStackPosition()
    {
        int StackTileNum = TileStack.Count;
        Vector3 Pos = new Vector3((ScreenWidth - 7) / 2.0f, 0.5f, 0);
        LinkedListNode<GameObject> TileNode = TileStack.First;
        while (TileNode != null)
        {
            TileNode.Value.transform.position = Pos;
            Pos += new Vector3(1.0f, 0, 0);
            TileNode = TileNode.Next;
        }
    }


    int UpdateStack(GameObject NewTile)
    {
        NewTile.GetComponent<Tile>().IsActive = false;
        NewTile.GetComponent<Tile>().IsExistInView = false;
        ExistNum--;

        int NewType = NewTile.GetComponent<Tile>().Type;
        int NewTypeCount = 1;
        LinkedListNode<GameObject> TileNode = TileStack.First;
        bool HasInserted = false;
        while (TileNode != null)
        {
            int NodeType = TileNode.Value.GetComponent<Tile>().Type;
            if (NodeType == NewType)
            {
                NewTypeCount++;
                if(HasInserted == false)
                {
                    LinkedListNode<GameObject> NewNode = new LinkedListNode<GameObject>(NewTile);
                    TileStack.AddAfter(TileNode, NewNode);
                    TileNode = TileNode.Next;
                    HasInserted = true;
                }
            }
            TileNode = TileNode.Next;  
        }

        if(HasInserted == false)
        {
            TileStack.AddLast(NewTile);
            HasInserted = true;
        }

        // 三消
        if (NewTypeCount == 3)
        {
            TileNode = TileStack.First;
            while (TileNode != null)
            {
                int NodeType = TileNode.Value.GetComponent<Tile>().Type;
                if(NodeType == NewType)
                {
                    LinkedListNode<GameObject> nodeToRemove = TileNode;
                    TileNode = TileNode.Next;
                    nodeToRemove.Value.SetActive(false);

                    TileStack.Remove(nodeToRemove);
                }
                else
                {
                    TileNode = TileNode.Next;
                }
            }
        }

        UpdateStackPosition();

        // 判断是否满
        if (TileStack.Count == MaxStackNum)
        {
            Debug.Log("游戏失败");
            return -1;
        }

        if(ExistNum == 0) 
        {
            Debug.Log("游戏成功");
        }

        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 设置相机
        mainCamera = Camera.main;  // 获取主相机
        SetCamera();  // 设置相机

        //CreateTile(new Vector3(1, 1, 0), 2);

        GenerateTileLists();
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButtonDown(0)) // 判断是否是鼠标左键点击 
        {
            GameObject clickedObject = GetObjectUnderMouse();
            if(clickedObject != null)
            {
                Tile tile = clickedObject.GetComponent<Tile>();
                if(tile != null && tile.IsExistInView && tile.IsActive)
                {
                    UpdateStack(clickedObject);
                }

                UpdateActiveState();
            }
        }

    }
}
