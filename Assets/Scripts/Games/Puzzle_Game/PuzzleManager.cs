using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    enum STATE { wait, idle, touch, move, calc, finish, cancel}
    STATE state = STATE.wait;

    int sliceCount = 4;
    int imageNum = 0;

    float tileScale;                                // ���� �̹����� ���� �� �̹����� ũ�� ����
    float tileSpan;                                 // Ÿ�� ����
    float tileSpanX;                                // ��ġ�� �� Ÿ�� ���� ����
    float tileSpanY;                                // ��ġ�� �� Ÿ�� ���� ����

    List<Sprite> sprites = new List<Sprite> ();     // �ڸ� �̹����� ������ ����Ʈ
    List<Transform> tiles = new List<Transform> (); // ȭ�鿡 ��ġ�� Ÿ���� ����Ʈ

    List<int> orders = new List<int> ();            // Ÿ���� ��ȣ�� ������ ����Ʈ
    List<int> moveTiles = new List<int> () ;        // �̵��ؾ��� Ÿ���� ��ȣ

    int dir;                                        // Ÿ���� �̵� ���� / ��, ������, �Ʒ�, ���� 
    int tileNum;                                    // Ŭ���� Ÿ���� ��ȣ
    bool canCalculate = true;                       // Ÿ�� �̵� ��, Ÿ�� ��ȣ(orders)�� ���� ����

    GameObject startUI;
    GameObject gameUI;
    GameObject clearPanel;

    int moveCount = 0;
    public TMP_Text moveText;

    void Awake()
    {
        InitGame();
        ShuffleTile();
        DrawTiles();

        startUI = GameObject.Find("PuzzleGameStartPanel");
        gameUI = GameObject.Find("PuzzleGameUI");
        clearPanel = GameObject.Find("PuzzleGameClearPanel");
        startUI.SetActive(false);
        gameUI.SetActive(false);
        clearPanel.SetActive(false);

        moveText.text = moveCount.ToString();
    }

    void Update()
    {
        switch (state)
        {
            case STATE.touch:
                CheckTile();
                break;
            case STATE.move:
                MoveTiles();
                moveCount++;
                moveText.text = moveCount.ToString();
                break;
            case STATE.calc:
                CalculateOrder();
                break;
            case STATE.finish:
                Debug.Log(state);
                SetFinish();
                break;
        }
    }

    void InitGame()
    {
        SplitTexture();
        MakeTiles();
    }

    // �ؽ�ó �ڸ���
    void SplitTexture()
    {
        // ���� �̹���
        Texture2D org = Resources.Load("MiniGame/Image_0", typeof(Texture2D)) as Texture2D;

        // �̹��� �б�
        Texture2D texture = Resources.Load("MiniGame/Image_" + imageNum, typeof(Texture2D)) as Texture2D;

        // ���� �̹������� ����
        tileScale = (float)org.width / texture.width;

        // �ڸ� ������ ũ�� 
        float w = texture.width / sliceCount;   
        float h = texture.height / sliceCount;

        // �ؽ�ó�� ���������� �ڸ���
        sprites.Clear();
        for (int y = sliceCount - 1; y >= 0; y--)
        {
            for (int x = 0; x < sliceCount; x++)
            {
                Rect rect = new Rect(x * w, y * h, w, h);            // ���ҵǴ� �̹����� �������� ũ��
                Vector2 pivot = new Vector2(0, 1);                   // �߶� �̹����� �Ǻ�, ���� ���� �������� ����

                Sprite sprite = Sprite.Create(texture, rect, pivot); // �ؽ�ó�� �߶� ���ο� ��������Ʈ ����
                sprites.Add(sprite);                                 // �߶� ��������Ʈ ����
            }
        }
    }

    void MakeTiles()
    {
        tiles.Clear();
        orders.Clear();

        Vector2 size = sprites[0].bounds.size;                  // ȭ�鿡 ��µǴ� ��������Ʈ ũ��
        int n = 0;                                              // Ÿ���� ���� ��ȣ

        for (int y = 0; y < sliceCount; y++)
        {
            for (int x = 0; x < sliceCount; x++)
            {
                MakeSingleTile(n, size);
                orders.Add(n++);                                // ����Ʈ�� �Ϸù�ȣ �ֱ�
            }
        }

        // ������ Ÿ��
        orders[orders.Count - 1] = -1;                          // ������ Ÿ�� ��ȣ�� -1
        tiles[orders.Count - 1].gameObject.SetActive(false);    // ������ Ÿ���� ��Ȱ��ȭ
    }

    void MakeSingleTile(int idx, Vector2 size)
    {
        GameObject tile = Instantiate(Resources.Load("Tile")) as GameObject;
        tile.transform.localScale = new Vector3(tileScale, tileScale, 1);    // Ÿ���� ������ ����

        // Ÿ�Ͽ� ������ Sprite ������
        SpriteRenderer render = tile.GetComponent<SpriteRenderer>();
        render.sprite = sprites[idx];

        render.material.SetInt("_count", sliceCount);                        // ���̴� �Ӽ� ����
        tile.name = "Tile" + idx;                                            // Ÿ�� �̸� ����

        BoxCollider2D collider = tile.GetComponent<BoxCollider2D>();
        collider.size = size;                                                // Box Collider2D�� Ÿ���� ũ��� ��ġ��Ŵ
        collider.offset = new Vector2(size.x / 2, -size.y / 2);

        tiles.Add(tile.transform);                                           // Ÿ�� ����
    }

    void DrawTiles()
    {
        state = STATE.wait;

        Transform tiles = new GameObject("Tiles").transform;                  // ȭ���� Ÿ���� ������ �����̳�
        tiles.localScale = new Vector3(2, 2, 2);

        // Ÿ�� ���� ���ϱ�
        Sprite sprite = this.tiles[0].GetComponent<SpriteRenderer>().sprite;
        tileSpan = sprite.bounds.size.x * tileScale;
        tileSpanX = sprite.bounds.size.x * tileScale + 0.2f;                   // ȭ���� Ÿ�� ����
        tileSpanY = sprite.bounds.size.y * tileScale + 0.2f;

        for (int y = 0; y < sliceCount; y++)
        {
            for (int x = 0; x < sliceCount; x++)
            {
                int idx = y * sliceCount + x;                                  // 2���� ��ǥ�� 1�������� ��ȯ

                // Ÿ���� �ε���
                int n = orders[idx];                                           // Ÿ���� ���� �б�
                if (n == -1)
                {
                    n = orders.Count - 1;                                      // �� Ÿ���̸� ������ Ÿ�Ϲ�ȣ�� ����
                }

                Vector3 pos = new Vector3(x * tileSpanX, -y * tileSpanY, 0);   // Ÿ���� ��ġ ���
                this.tiles[n].position = pos;
                this.tiles[n].parent = tiles;                                      // Ÿ���� �����̳ʿ� �ֱ�
            }
        }
        tiles.position = new Vector3(0, -843, 0);

        state = STATE.idle;
    }

    // Ÿ�� ����
    void ShuffleTile()
    {
        // �׽�Ʈ
        //return;
        for (int i = 0; i < orders.Count - 1; i++)
        {
            int n = Random.Range(i + 1, orders.Count); // ���� ��ȣ���� ū ���� ����
            int tmp = orders[i];                       // ���� ���� ������ ����Ű�� ��ġ ���� ��ȯ
            orders[i] = orders[n];
            orders[n] = tmp;
            //if (!CheckValidate()) ShuffleTile();
        }
    }

    // ���Ἲ ����
    bool CheckValidate()
    {
        int sum = 0;
        for (int i = 0; i < orders.Count - 1; i++)
        {
            if (orders[i] == -1) continue;

            for (int j = i + 1; j < orders.Count; j++)
            {
                if (orders[j] != -1 && orders[i] > orders[j]) sum++;
            }
        }

        return (sum % 2 == 0);
    }

    void SetTouch(int _tileNum)
    {
        if (state == STATE.idle)
        {
            tileNum = _tileNum;
            state = STATE.touch;
        }

        Debug.Log("TileNum : " + tileNum);
    }

    void CheckTile()
    {
        state = STATE.wait;

        // �̵��� ����� �̵��� Ÿ��
        dir = 0;
        moveTiles.Clear();

        // Ŭ���� Ÿ�ϰ� ���� ��ġ ã��
        int tile = orders.FindIndex(x => x == tileNum);
        int blank = orders.FindIndex(x => x == -1);

        // ��ǥ ���, 1���� ��ǥ�� 2���� ��ǥ�� ��ȯ
        int x1 = tile % sliceCount;
        int y1 = tile / sliceCount;

        int x2 = blank % sliceCount;
        int y2 = blank / sliceCount;

        // ���� ���� ����
        if (x1 == x2)                                                             // Ŭ���� Ÿ���� ���� �Ʒ��� ������ ����
        {
            moveTiles.Add(blank);                                                 // ���� ��ȣ ����

            // �̵� ����� �� ����
            dir = (y1 > y2) ? 1 : 3;
            int row = (y1 > y2) ? sliceCount : -sliceCount;
            int idx = blank + row;                                                // ����� ������ Ÿ�� ��ȣ ����

            while (true)
            {
                moveTiles.Add(idx);                                               // Ÿ�� ��ȣ ����
                idx += row;                                                       // ������ ���� Ÿ�� ��ȣ
                if ((dir == 1 && idx > tile) || (dir == 3 && idx < tile)) break;
            }
        }

        // ���� ���� ����
        else if (y1 == y2)
        {
            moveTiles.Add(blank);

            // �̵� ����� �� ����
            dir = (x1 > x2) ? 4 : 2;
            int col = (x1 > x2) ? 1 : -1;
            int idx = blank + col;

            while (true)
            {
                moveTiles.Add(idx);
                idx += col;
                if ((dir == 2 && idx < tile) || (dir == 4 && idx > tile)) break;  // ������ ������ ������ ����
            }
        }
        state = (moveTiles.Count > 0) ? STATE.move : STATE.idle;                  // �̵��� Ÿ�� ���� �� �̵� ���·� ��ȯ
    }

    void SetCalculation()
    {
        state = STATE.calc;
    }

    void MoveTiles()
    {
        state = STATE.wait;

        // Ÿ���� �̵� ���� ����
        Vector3[] vectors = { Vector3.zero, Vector3.up, Vector3.right, Vector3.down, Vector3.left }; // �� ������ ���� ����

        foreach (int idx in moveTiles)                                                               // �̵��� Ÿ���� ó��
        {
            int p = orders[idx];                                                                     // �̵��� Ÿ���� ��ȣ
            if (p == -1) continue;
            Vector3 pos = tiles[p].position;                                                         // �̵��� Ÿ���� ���� ��ġ
            if (vectors[dir] == Vector3.right || vectors[dir] == Vector3.left)
            {
                Vector3 target = pos + vectors[dir] * tileSpanX;
                tiles[p].SendMessage("SetMove", target);
            }
            if (vectors[dir] == Vector3.up || vectors[dir] == Vector3.down)
            {
                Vector3 target = pos + vectors[dir] * tileSpanY;
                tiles[p].SendMessage("SetMove", target);
            }
        }
        canCalculate = true;
    }

    // Ÿ�� �ε��� ����
    void CalculateOrder()
    {
        if (!canCalculate)                                  // �ε��� ���� ���� Ȯ��
        {
            state = STATE.idle;
            return;
        }

        canCalculate = false;                               // �ε��� �ߺ� ���� ����
        state = STATE.wait;

        for (int i = 0; i < moveTiles.Count - 1; i++)       // �̵��� Ÿ�� ��� ��ü ����
        {
            int n1 = moveTiles[i];
            int n2 = moveTiles[i + 1];
            orders[n1] = orders[n2];                        // �ε����� �� ĭ�� �̵�
        }

        // ���� �̵�
        int blank = moveTiles[moveTiles.Count - 1];
        orders[blank] = -1;                                 // �̵��� ������ Ÿ���� �������� ����

        // ���� �Ϸ��ߴ��� ����
        bool finished = true;
        for (int i = 0; i < orders.Count - 1; i++)
        {
            if (orders[i] != i)                             // �ε����� �Ϸù�ȣ �������� ����
            {
                finished = false;
                break;
            }
        }

        if (finished)
        {
            state = STATE.finish;
        }
        else
        {
            state = STATE.idle;
        }
    }

    public void Reset()
    {
        moveCount = 0;
        moveText.text = moveCount.ToString();
        GameObject tiles = GameObject.Find("Tiles");
        Destroy(tiles);
        InitGame();
        ShuffleTile();
        DrawTiles();
    }

    void SetFinish()
    {
        foreach (Transform tile in tiles)
        {
            tile.GetComponent<SpriteRenderer>().material.SetInt("_count", 0); // �� Ÿ���� �׵θ� ����
        }

        // ������ Ÿ��
        int last = orders.Count - 1;
        tiles[last].gameObject.SetActive(true);
        tiles[last].position = tiles[last - 1].position + Vector3.right * tileSpanX;

        clearPanel.SetActive(true);
    }
}
