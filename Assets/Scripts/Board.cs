using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using BlockNS;


public class Board : MonoBehaviour
{


    public Transform[] BlockArray = new Transform[5];

    public int BoardWidth = 8;
    public int BoardHeight = 8;

    private int[,] BoardGrid;

    public float SwapSpeed;

    private Color _blockColor = Color.white;

    private bool _swapEffect = false;

    [SerializeField]
    private List<Block> blockList = new List<Block>();

    // Use this for initialization
    void Awake()
    {
        BoardGrid = new int[BoardWidth, BoardHeight];
    }

    void Start()
    {

        GenBoard();
    }

    // Update is called once per frame
    void Update()
    {
         

        if(Block.Select)
        {
            if(_blockColor == Color.white)
            {
                _blockColor = Block.Select.gameObject.GetComponent<Renderer>().material.color;
                
            }
            Block.Select.gameObject.GetComponent<Renderer>().material.color = Color32.Lerp(_blockColor,Color.black,Mathf.PingPong(Time.time,0.5f));
            
        }

        if (Block.Select && Block.MoveTo)
        {
            if (CheckIfNear()==true)
            {
                if(!_swapEffect)
                {
                    _swapEffect = true;
                    StartCoroutine(SwapBlockEffect(true));
                }
            }
            else
            {
                Block.Select.gameObject.GetComponent<Renderer>().material.color = _blockColor;
                _blockColor = Color.white;
                Block.Select = null;
                Block.MoveTo = null;
            }
        }


    }

    void GenBoard()
    {
        for (int x = 0; x < BoardWidth; x++)
        {
            for (int y = 0; y < BoardHeight; y++)
            {
                AddNewBlock(x, y);
            }
        }

    }

    bool CheckIfNear()
    {
        Block selectBlock = Block.Select.gameObject.GetComponent<Block>();
        Block moveToBlock = Block.MoveTo.gameObject.GetComponent<Block>();

        //Debug.Log("CheckIfNear вызван");
        if (selectBlock.X - 1 == moveToBlock.X && selectBlock.Y == moveToBlock.Y)
        {
            //Debug.Log("Блок слева");
            return true;
        }
        if (selectBlock.X + 1 == moveToBlock.X && selectBlock.Y == moveToBlock.Y)
        {
            //Debug.Log("Блок справа");
            return true;
        }
        if (selectBlock.X == moveToBlock.X && selectBlock.Y - 1 == moveToBlock.Y)
        {
            //Debug.Log("Блок внизу");
            return true;
        }

        if (selectBlock.X == moveToBlock.X && selectBlock.Y + 1 == moveToBlock.Y)
        {
            //Debug.Log("Блок сверху");
            return true;
        }
        Debug.Log("Функция CheckifNear не вернула TRUE");
        return false;
    }

    //void Swap()
    //{
    //    Debug.Log("Function SWAP!!!");
    //    Block select = Block.Select.gameObject.GetComponent<Block>();
    //    Block move = Block.MoveTo.gameObject.GetComponent<Block>();

    //    Vector3 tempPos = select.transform.position;

    //    int tempX = select.X;
    //    int tempY = select.Y;

    //    Debug.Log("Позиция меняемого блока " + move.transform.position);
    //    select.transform.position = move.transform.position;
    //    move.transform.position = tempPos;

    //    select.X = move.X;
    //    select.Y = move.Y;

    //    move.X = tempX;
    //    move.Y = tempY;

    //    BoardGrid[select.X, select.Y] = select.TypeID;
    //    BoardGrid[move.X, move.Y] = move.TypeID;

    //}
    
    void AddNewBlock(int xPos, int yPos)
    {
        int randomBlockType;
        do{
        randomBlockType = Random.Range(0, BlockArray.Length);
        }
        while((xPos >=2
            && BoardGrid[xPos - 1, yPos] == randomBlockType
            && BoardGrid[xPos - 2, yPos] == randomBlockType)
        ||
        (yPos >=2
            && BoardGrid[xPos, yPos - 1] == randomBlockType
            && BoardGrid[xPos, yPos - 2] == randomBlockType));
        Transform block = Instantiate(BlockArray[randomBlockType], new Vector3(xPos * 1.95f, yPos * 1.95f, 0f), Quaternion.identity) as Transform;
        block.transform.parent = gameObject.transform;
        block.name = "Block[X:" + xPos + " Y:" + yPos + "] Type:" + randomBlockType;

        Block b = block.gameObject.AddComponent<Block>();
        b.TypeID = randomBlockType;
        b.X = xPos;
        b.Y = yPos;
        BoardGrid[xPos, yPos] = b.TypeID;
        blockList.Add(b);
    }


    //void FindAndRemoveMatches()
    //{
    //    var matches = LookForMatches();

    //    foreach (var block in matches)
    //    {
            
    //    }
    //}

    //ArrayList LookForMatches()
    //{
    //    var matchList = new ArrayList();

    //    for (int row = 0; row < BoardHeight; row++)
    //    {
    //        for (int col = 0; col < BoardWidth-2; col++)
    //        {
    //            var match = GetMatchHoriz(col, row);
    //            if(match.Count>2)
    //            {
    //                matchList.Add(match);
    //                col += match.Count - 1;
    //            }

    //        }
    //    }

    //    for (int row = 0; row < BoardHeight; row++)
    //    {
    //        for (int col = 0; col < BoardWidth - 2; col++)
    //        {
    //            var match = GetMatchVert(col, row);
    //            if (match.Count > 2)
    //            {
    //                matchList.Add(match);
    //                row += match.Count - 1;
    //            }
    //        }
    //    }
    //    return matchList;
    //}


    //ArrayList GetMatchHoriz(int col, int row)
    //{
    //    var matchList = new ArrayList(BoardGrid[col, row]);
    //    for (int i = 1; col + i < BoardWidth; i++)
    //    {
    //        if (BoardGrid[col, row] == BoardGrid[col + i, row])
    //        {
    //            matchList.Add(BoardGrid[col + i, row]);
    //        }
    //        else
    //        {
    //            return matchList;
    //        }
    //    }
    //    return matchList;
    //}

    //ArrayList GetMatchVert(int col, int row)
    //{
    //    var matchList = new ArrayList(BoardGrid[col, row]);
    //    for (int i = 1; row + i < BoardHeight; i++)
    //    {
    //        if (BoardGrid[col, row] == BoardGrid[col, row + i])
    //        {
    //            matchList.Add(BoardGrid[col, row + i]);
    //        }
    //        else
    //        {
    //            return matchList;
    //        }
    //    }
    //    return matchList;
    //}

    //ArrayList DetectHorizontalMatches()
    //{
    //    ArrayList set = new ArrayList();
    //    for (int row = 0; row < BoardHeight; row++)
    //    {
    //        for (int column = 0; column < BoardWidth-2; )
    //        {
    //            if(BoardGrid[column,row] != 404)
    //            {
    //                int matchType = BoardGrid[column, row];

    //                if(BoardGrid[column+1,row] == matchType &&
    //                BoardGrid[column+2,row] == matchType)
    //                {
    //                    BlockChain chain = new BlockChain();
    //                    do
    //                    {
                            
    //                    }

    //                }
    //            }
    //        }
    //    }
    //}


    bool CheckMatch()
    {
        //Get all blocks in scene
        Block[] allb = FindObjectsOfType(typeof(Block)) as Block[];
        Block sel = Block.Select.gameObject.GetComponent<Block>();
        Block mov = Block.MoveTo.gameObject.GetComponent<Block>();

        int countSU = 0; //Count Up
        int countSD = 0; //Count Down
        int countSL = 0; //Count Left
        int countSR = 0; //Count RIght

        //Left
        for (int l = sel.X - 1; l >= 0; l--)
        {
            if (BoardGrid[l, sel.Y] == sel.TypeID)
            {//If block have same ID
                countSL++;
            }
            if (BoardGrid[l, sel.Y] != sel.TypeID)
            {//If block doesn't have same ID
                //Stop
                break;
            }
        }
        //Right
        for (int r = sel.X; r < BoardWidth; r++)
        {
            if (BoardGrid[r, sel.Y] == sel.TypeID)
            {
                countSR++;
            }
            if (BoardGrid[r, sel.Y] != sel.TypeID)
            {
                break;
            }
        }
        //Down
        for (int d = sel.Y - 1; d >= 0; d--)
        {
            if (BoardGrid[sel.X, d] == sel.TypeID)
            {
                countSD++;
            }
            if (BoardGrid[sel.X, d] != sel.TypeID)
            {
                break;
            }
        }

        //Up
        for (int u = sel.Y; u < BoardHeight; u++)
        {
            if (BoardGrid[sel.X, u] == sel.TypeID)
            {
                countSU++;
            }

            if (BoardGrid[sel.X, u] != sel.TypeID)
            {
                break;
            }
        }

        int countMU = 0; //Count Up
        int countMD = 0; //Count Down
        int countML = 0; //Count Left
        int countMR = 0; //Count RIght

        //Left
        for (int l = mov.X - 1; l >= 0; l--)
        {
            if (BoardGrid[l, mov.Y] == mov.TypeID)
            {//If block have same ID
                countML++;
            }
            if (BoardGrid[l, mov.Y] != mov.TypeID)
            {//If block doesn't have same ID
                //Stop
                break;
            }
        }
        //Right
        for (int r = mov.X; r < BoardWidth; r++)
        {
            if (BoardGrid[r, mov.Y] == mov.TypeID)
            {
                countMR++;
            }
            if (BoardGrid[r, mov.Y] != mov.TypeID)
            {
                break;
            }
        }
        //Down
        for (int d = mov.Y - 1; d >= 0; d--)
        {
            if (BoardGrid[mov.X, d] == mov.TypeID)
            {
                countMD++;
            }
            if (BoardGrid[mov.X, d] != mov.TypeID)
            {
                break;
            }
        }

        //Up
        for (int u = mov.Y; u < BoardHeight; u++)
        {
            if (BoardGrid[mov.X, u] == mov.TypeID)
            {
                countMU++;
            }

            if (BoardGrid[mov.X, u] != mov.TypeID)
            {
                break;
            }
        }


        //Проверка цепочек у выбранного блока
        if (countML + countMR >= 3 || countMU + countMD >= 3 || countSL + countSR >= 3 || countSD + countSU >= 3)
        {
            if (countSL + countSR >= 3)
            {
                //Destroy and mark empty block
                for (int cl = 0; cl <= countSL; cl++)
                {
                    foreach (Block b in allb)
                    {
                        if (b.X == sel.X - cl && b.Y == sel.Y)
                        {
                            b.StartCoroutine("DestroyBlock");
                            BoardGrid[b.X, b.Y] = 404; ; //To mark empty block
                        }
                    }
                }
                for (int cr = 0; cr < countSR; cr++)
                {
                    foreach (Block b in allb)
                    {
                        if (b.X == sel.X + cr && b.Y == sel.Y)
                        {
                            b.StartCoroutine("DestroyBlock");
                            BoardGrid[b.X, b.Y] = 404; ;
                        }
                    }
                }
            }
            if (countSD + countSU >= 3)
            {
                for (int cd = 0; cd <= countSD; cd++)
                {
                    foreach (Block b in allb)
                    {
                        if (b.X == sel.X && b.Y == sel.Y - cd)
                        {
                            BoardGrid[b.X, b.Y] = 404; ;
                            b.StartCoroutine("DestroyBlock");
                        }
                    }
                }
                for (int cu = 0; cu < countSU; cu++)
                {
                    foreach (Block blc in allb)
                    {
                        if (blc.X == sel.X && blc.Y == sel.Y + cu)
                        {
                            BoardGrid[blc.X, blc.Y] = 404; ;
                            blc.StartCoroutine("DestroyBlock");
                        }
                    }
                }
            }

            if (countML + countMR >= 3)
            {
                //Destroy and mark empty block
                for (int cl = 0; cl <= countML; cl++)
                {
                    foreach (Block b in allb)
                    {
                        if (b.X == mov.X - cl && b.Y == mov.Y)
                        {
                            BoardGrid[b.X, b.Y] = 404; //To mark empty block
                            b.StartCoroutine("DestroyBlock");
                        }
                    }
                }
                for (int cr = 0; cr < countMR; cr++)
                {
                    foreach (Block b in allb)
                    {
                        if (b.X == mov.X + cr && b.Y == mov.Y)
                        {

                            BoardGrid[b.X, b.Y] = 404; //To mark empty block
                            b.StartCoroutine("DestroyBlock");
                        }
                    }
                }
            }

            if (countMU + countMD >= 3)
            {
                for (int cd = 0; cd < countMD; cd++)
                {
                    foreach (Block blc in allb)
                    {
                        if (blc.X == mov.X && blc.Y == mov.Y - cd)
                        {
                            BoardGrid[blc.X, blc.Y] = 404;
                            blc.StartCoroutine("DestroyBlock");
                        }
                    }
                }

                for (int cu = 0; cu < countMU; cu++)
                {
                    foreach (Block blc in allb)
                    {
                        if (blc.X == mov.X && blc.Y == mov.Y + cu)
                        {
                            BoardGrid[blc.X, blc.Y] = 404;
                            blc.StartCoroutine("DestroyBlock");
                        }
                    }
                }
            }
            Respawn();
            return true;

        }
        return false;
    }




    void Respawn()
    {
        for (int x = 0; x < BoardWidth; x++)
        {
            for (int y = 0; y < BoardHeight; y++)
            {
                if (BoardGrid[x, y] == 404)
                { //Spawn it only on destroyed block
                    AddNewBlock(x, y);


                }
            }
        }
    }

    public void Restart()
    {
        
        foreach (var b in blockList)
        {
            GameObject.Destroy(b);
        }
        for (int x = 0; x < BoardWidth; x++)
        {
            for (int y = 0; y < BoardHeight; y++)
            {
                AddNewBlock(x, y);
            }
        }
        gameObject.transform.position = new Vector2(-6.0f, -6.0f);
    }

    IEnumerator SwapBlockEffect(bool match)
    {

        Block sel = Block.Select.gameObject.GetComponent<Block>();
        Block mov = Block.MoveTo.gameObject.GetComponent<Block>();

        Vector3 selTempPos = sel.transform.position;
        Vector3 movTempPos = mov.transform.position;

        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * SwapSpeed;
            sel.transform.position = Vector3.Lerp(selTempPos, movTempPos, time);
            mov.transform.position = Vector3.Lerp(movTempPos, selTempPos, time);
            yield return null;
        }

        int tempX = sel.X;
        int tempY = sel.Y;

        //Swap data
        sel.X = mov.X;
        sel.Y = mov.Y;

        mov.X = tempX;
        mov.Y = tempY;

        //Change ID in board
        BoardGrid[sel.X, sel.Y] = sel.TypeID;
        BoardGrid[mov.X, mov.Y] = mov.TypeID;


        //Do we want to run the code to check for match?
        if (match == true)
        {
            //Check for match3
            if (CheckMatch() == true)
            {

                //There is match
                _swapEffect = false;//End effect
                Block.Select.gameObject.GetComponent<Renderer>().material.color = _blockColor;
                _blockColor = Color.white;
                Block.Select = null;
                Block.MoveTo = null;
            }
            else
            {
                //There is no match, return them in their default position
                StartCoroutine(SwapBlockEffect(false));//Swap their position and data using new swap effect, without checking for match3
                Block.Select.gameObject.GetComponent<Renderer>().material.color = _blockColor;
                _blockColor = Color.white;
                Block.Select = null;
                Block.MoveTo = null;
            }
        }
        else
        {//We don't
            _swapEffect = false; //End effect
        }

    }

}
