using System.Collections;
using System.Collections.Generic;
//using Unity.Mathematics;
using UnityEngine;



public class GlobalManager : MonoBehaviour
{
    public const int Maxrows = 10;
    public const int Maxcols = 10;
    public const int MaxStackNum = 7;
    public const int MaxTileTypes = 5;
    public const float TileRadius = 0.5f;

    private float ScreenWidth;
    private float ScreenHeight;


    public Camera mainCamera; // �����

    public List<GameObject> TileList = new List<GameObject>();
    public LinkedList<GameObject> TileStack = new LinkedList<GameObject>();
    private int ExistNum = 0;


    public Vector2 FindPos(int row, int col)
    {
        return new Vector2(row, col);
    }



    // ���� Tile ������
    public GameObject CreateTile(Vector3 position, int material, int id)
    {
        // ����һ�� Cube ����
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // ��ת Cube ��ָ���Ƕ� (270, 0, 0)
        tile.transform.rotation = Quaternion.Euler(8f, 8f, 0f);

        // ����λ��Ϊ����� Position
        tile.transform.position = position;

        // ��������Ϊ (0.1, 0.1, 0.1)
        //tile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Ϊ Cube ��� Tile.cs �ű�
        Tile tileScript = tile.AddComponent<Tile>();

        // ���� TileType Ϊ����� material ֵ
        tileScript.Type = material;
        tileScript.Id = id;

        // ����Ը��������ʼ���������ԣ�Ĭ��ֵ�ᱣ��
        // tileScript.IsExist = true;  // Ĭ���� true
        // tileScript.IsActive = true; // Ĭ���� true

        // ������������� GameObject
        return tile;
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
                        Random.Range(0 + 1.0f, ScreenWidth - 1.0f),   // x ��Χ��0 �� Screen.width
                        Random.Range(2.0f, ScreenHeight - 1.0f),  // y ��Χ��0 �� Screen.height
                        Random.Range(0f, 10f)            // z ��Χ��0 �� 10
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


    // ÿ���ֱ��ʱ仯ʱ���øú���
    public void SetCamera()
    {
        // ��ȡ��Ļ�Ŀ�߱�
        float screenAspect = (float)Screen.width / Screen.height;

        // ���������������Ұ��С��orthographic size��
        if (mainCamera.orthographic)
        {
            // ���������������Ұ��С��������Ļ�Ŀ�߱ȵ���
            mainCamera.orthographicSize = (screenAspect < 1) ? 5 / screenAspect : 5;

            // ������Ұ��Ŀ�Ⱥ͸߶�
            ScreenWidth = mainCamera.orthographicSize * 2 * screenAspect;
            ScreenHeight = mainCamera.orthographicSize * 2;

            // �������λ�ã�ʹ�����½��� (0, 0, -10)
            mainCamera.transform.position = new Vector3(ScreenWidth / 2, ScreenHeight / 2, -10);
            Debug.Log("Screen size: " + ScreenWidth.ToString() + "  " + ScreenHeight.ToString());
        }
        else
        {
            Debug.LogWarning("Camera is not orthographic! Please use an orthographic camera.");
        }
    }


    // ��ȡ������µ� Tile ID
    public GameObject GetObjectUnderMouse()
    {
        // ��ȡ�������Ļ�ϵ�λ��
        Vector3 mousePosition = Input.mousePosition;

        // ����Ļ����ת��Ϊ��������
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldMousePosition.z = 10f; // ���� z Ϊ10��ȷ��������������� z �ᷢ��

        // �����λ�÷������߳������λ��
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        // ���߼��
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // ���������ײ�����壬���ظ������ GameObject
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

        // ����
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

        // �ж��Ƿ���
        if (TileStack.Count == MaxStackNum)
        {
            Debug.Log("��Ϸʧ��");
            return -1;
        }

        if(ExistNum == 0) 
        {
            Debug.Log("��Ϸ�ɹ�");
        }

        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        // �������
        mainCamera = Camera.main;  // ��ȡ�����
        SetCamera();  // �������

        //CreateTile(new Vector3(1, 1, 0), 2);

        GenerateTileLists();
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButtonDown(0)) // �ж��Ƿ������������ 
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
