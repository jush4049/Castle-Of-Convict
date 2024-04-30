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

    float tileScale;                                // 기준 이미지에 대한 각 이미지의 크기 비율
    float tileSpan;                                 // 타일 간격
    float tileSpanX;                                // 배치될 각 타일 간의 간격
    float tileSpanY;                                // 배치될 각 타일 간의 간격

    List<Sprite> sprites = new List<Sprite> ();     // 자른 이미지를 저장할 리스트
    List<Transform> tiles = new List<Transform> (); // 화면에 배치할 타일의 리스트

    List<int> orders = new List<int> ();            // 타일의 번호를 저장할 리스트
    List<int> moveTiles = new List<int> () ;        // 이동해야할 타일의 번호

    int dir;                                        // 타일의 이동 방향 / 위, 오른쪽, 아래, 왼쪽 
    int tileNum;                                    // 클릭한 타일의 번호
    bool canCalculate = true;                       // 타일 이동 후, 타일 번호(orders)의 갱신 여부

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

    // 텍스처 자르기
    void SplitTexture()
    {
        // 기준 이미지
        Texture2D org = Resources.Load("MiniGame/Image_0", typeof(Texture2D)) as Texture2D;

        // 이미지 읽기
        Texture2D texture = Resources.Load("MiniGame/Image_" + imageNum, typeof(Texture2D)) as Texture2D;

        // 기준 이미지와의 비율
        tileScale = (float)org.width / texture.width;

        // 자를 조각의 크기 
        float w = texture.width / sliceCount;   
        float h = texture.height / sliceCount;

        // 텍스처를 위에서부터 자르기
        sprites.Clear();
        for (int y = sliceCount - 1; y >= 0; y--)
        {
            for (int x = 0; x < sliceCount; x++)
            {
                Rect rect = new Rect(x * w, y * h, w, h);            // 분할되는 이미지의 기준점과 크기
                Vector2 pivot = new Vector2(0, 1);                   // 잘라낸 이미지의 피봇, 왼쪽 위를 기준으로 설정

                Sprite sprite = Sprite.Create(texture, rect, pivot); // 텍스처를 잘라서 새로운 스프라이트 생성
                sprites.Add(sprite);                                 // 잘라낸 스프라이트 저장
            }
        }
    }

    void MakeTiles()
    {
        tiles.Clear();
        orders.Clear();

        Vector2 size = sprites[0].bounds.size;                  // 화면에 출력되는 스프라이트 크기
        int n = 0;                                              // 타일의 시작 번호

        for (int y = 0; y < sliceCount; y++)
        {
            for (int x = 0; x < sliceCount; x++)
            {
                MakeSingleTile(n, size);
                orders.Add(n++);                                // 리스트에 일련번호 넣기
            }
        }

        // 마지막 타일
        orders[orders.Count - 1] = -1;                          // 마지막 타일 번호는 -1
        tiles[orders.Count - 1].gameObject.SetActive(false);    // 마지막 타일은 비활성화
    }

    void MakeSingleTile(int idx, Vector2 size)
    {
        GameObject tile = Instantiate(Resources.Load("Tile")) as GameObject;
        tile.transform.localScale = new Vector3(tileScale, tileScale, 1);    // 타일의 스케일 설정

        // 타일에 분할한 Sprite 입히기
        SpriteRenderer render = tile.GetComponent<SpriteRenderer>();
        render.sprite = sprites[idx];

        render.material.SetInt("_count", sliceCount);                        // 쉐이더 속성 설정
        tile.name = "Tile" + idx;                                            // 타일 이름 설정

        BoxCollider2D collider = tile.GetComponent<BoxCollider2D>();
        collider.size = size;                                                // Box Collider2D를 타일의 크기와 일치시킴
        collider.offset = new Vector2(size.x / 2, -size.y / 2);

        tiles.Add(tile.transform);                                           // 타일 저장
    }

    void DrawTiles()
    {
        state = STATE.wait;

        Transform tiles = new GameObject("Tiles").transform;                  // 화면의 타일을 저장할 컨테이너
        tiles.localScale = new Vector3(2, 2, 2);

        // 타일 간격 구하기
        Sprite sprite = this.tiles[0].GetComponent<SpriteRenderer>().sprite;
        tileSpan = sprite.bounds.size.x * tileScale;
        tileSpanX = sprite.bounds.size.x * tileScale + 0.2f;                   // 화면의 타일 간격
        tileSpanY = sprite.bounds.size.y * tileScale + 0.2f;

        for (int y = 0; y < sliceCount; y++)
        {
            for (int x = 0; x < sliceCount; x++)
            {
                int idx = y * sliceCount + x;                                  // 2차원 좌표를 1차원으로 변환

                // 타일의 인덱스
                int n = orders[idx];                                           // 타인의 색인 읽기
                if (n == -1)
                {
                    n = orders.Count - 1;                                      // 빈 타일이면 마지막 타일번호로 설정
                }

                Vector3 pos = new Vector3(x * tileSpanX, -y * tileSpanY, 0);   // 타일의 위치 계산
                this.tiles[n].position = pos;
                this.tiles[n].parent = tiles;                                      // 타일을 컨테이너에 넣기
            }
        }
        tiles.position = new Vector3(0, -843, 0);

        state = STATE.idle;
    }

    // 타일 섞기
    void ShuffleTile()
    {
        // 테스트
        //return;
        for (int i = 0; i < orders.Count - 1; i++)
        {
            int n = Random.Range(i + 1, orders.Count); // 현재 번호보다 큰 난수 생성
            int tmp = orders[i];                       // 현재 값과 난수가 가리키는 위치 값을 교환
            orders[i] = orders[n];
            orders[n] = tmp;
            //if (!CheckValidate()) ShuffleTile();
        }
    }

    // 무결성 조사
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

        // 이동할 방향과 이동할 타일
        dir = 0;
        moveTiles.Clear();

        // 클릭할 타일과 공백 위치 찾기
        int tile = orders.FindIndex(x => x == tileNum);
        int blank = orders.FindIndex(x => x == -1);

        // 좌표 계산, 1차원 좌표를 2차원 좌표로 변환
        int x1 = tile % sliceCount;
        int y1 = tile / sliceCount;

        int x2 = blank % sliceCount;
        int y2 = blank / sliceCount;

        // 세로 방향 조사
        if (x1 == x2)                                                             // 클릭한 타일의 위나 아래에 공백이 존재
        {
            moveTiles.Add(blank);                                                 // 공백 번호 저장

            // 이동 방향과 행 간격
            dir = (y1 > y2) ? 1 : 3;
            int row = (y1 > y2) ? sliceCount : -sliceCount;
            int idx = blank + row;                                                // 공백과 인접한 타일 번호 저장

            while (true)
            {
                moveTiles.Add(idx);                                               // 타일 번호 저장
                idx += row;                                                       // 저장할 다음 타일 번호
                if ((dir == 1 && idx > tile) || (dir == 3 && idx < tile)) break;
            }
        }

        // 가로 방향 조사
        else if (y1 == y2)
        {
            moveTiles.Add(blank);

            // 이동 방향과 열 간격
            dir = (x1 > x2) ? 4 : 2;
            int col = (x1 > x2) ? 1 : -1;
            int idx = blank + col;

            while (true)
            {
                moveTiles.Add(idx);
                idx += col;
                if ((dir == 2 && idx < tile) || (dir == 4 && idx > tile)) break;  // 조사할 범위를 넘으면 종료
            }
        }
        state = (moveTiles.Count > 0) ? STATE.move : STATE.idle;                  // 이동할 타일 존재 시 이동 상태로 전환
    }

    void SetCalculation()
    {
        state = STATE.calc;
    }

    void MoveTiles()
    {
        state = STATE.wait;

        // 타일의 이동 방향 벡터
        Vector3[] vectors = { Vector3.zero, Vector3.up, Vector3.right, Vector3.down, Vector3.left }; // 각 방향의 벡터 생성

        foreach (int idx in moveTiles)                                                               // 이동할 타일의 처리
        {
            int p = orders[idx];                                                                     // 이동할 타일의 번호
            if (p == -1) continue;
            Vector3 pos = tiles[p].position;                                                         // 이동할 타일의 현재 위치
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

    // 타일 인덱스 정리
    void CalculateOrder()
    {
        if (!canCalculate)                                  // 인덱스 정렬 여부 확인
        {
            state = STATE.idle;
            return;
        }

        canCalculate = false;                               // 인덱스 중복 정렬 방지
        state = STATE.wait;

        for (int i = 0; i < moveTiles.Count - 1; i++)       // 이동한 타일 목록 전체 조사
        {
            int n1 = moveTiles[i];
            int n2 = moveTiles[i + 1];
            orders[n1] = orders[n2];                        // 인덱스를 한 칸씩 이동
        }

        // 공백 이동
        int blank = moveTiles[moveTiles.Count - 1];
        orders[blank] = -1;                                 // 이동한 마지막 타일을 공백으로 설정

        // 정리 완료했는지 조사
        bool finished = true;
        for (int i = 0; i < orders.Count - 1; i++)
        {
            if (orders[i] != i)                             // 인덱스가 일련번호 순서인지 조사
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
            tile.GetComponent<SpriteRenderer>().material.SetInt("_count", 0); // 각 타일의 테두리 제거
        }

        // 마지막 타일
        int last = orders.Count - 1;
        tiles[last].gameObject.SetActive(true);
        tiles[last].position = tiles[last - 1].position + Vector3.right * tileSpanX;

        clearPanel.SetActive(true);
    }
}
