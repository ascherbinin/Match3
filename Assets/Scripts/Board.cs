using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using BlockNS;


public class Board : MonoBehaviour
{


    public Transform[] BlockArray = new Transform[5];

    public int NumColumns = 9;
    public int NumRows = 9;

    private Block[,] BoardGrid;

    public float SwapSpeed;

    private Color _blockColor = Color.white;

    private bool _canSwap = true;
    private bool _needCheckMacthPossible = false;

    [SerializeField]
    private List<Block> blockList = new List<Block>();

    Block BlockAtColumn(int column, int row)
    {
        if ((column >= 0 && column < NumColumns) && (row >= 0 && row < NumRows))
        {
            return BoardGrid[column, row];
        }
        else
        {
            Debug.Log("Ошибка, блок находится за границей сетки");
            return null;
        }
    }

    List<Block> CreateInitialBlock()
    {
        List<Block> set = new List<Block>();

        for (int row = 0; row < NumRows; row++)
        {
            for (int column = 0; column < NumColumns; column++)
            {
                int randomBlockType;
                do
                {
                    randomBlockType = Random.Range(0, BlockArray.Length);
                }
                while ((column >= 2
                        && BoardGrid[column - 1, row].BlockType == randomBlockType
                        && BoardGrid[column - 2, row].BlockType == randomBlockType)
                        ||
                      (row >= 2
                        && BoardGrid[column, row - 1].BlockType == randomBlockType
                        && BoardGrid[column, row - 2].BlockType == randomBlockType));

                Block b = CreateBlockAtColumn(column, row, randomBlockType);
                set.Add(b);
            }
        }
        return set;
    }

    Block CreateBlockAtColumn(int column, int row, int blockType)
    {
        Transform block = Instantiate(BlockArray[blockType], new Vector3(column * 1.95f, row * 1.95f, 0f), Quaternion.identity) as Transform;
        block.transform.parent = gameObject.transform;
        block.name = "Block[X:" + column + " Y:" + row + "]";

        Block b = block.gameObject.AddComponent<Block>();
        b.BlockType = blockType;
        b.Column = column;
        b.Row = row;

        b.NeedFall = true;

        BoardGrid[column, row] = b;

        return b;
    }

    void GenerateBoard()
    {
        CreateInitialBlock();
    }


    void Awake()
    {
        BoardGrid = new Block[NumColumns, NumRows];
    }

    void Start()
    {
        GenerateBoard();

    }

    // Update is called once per frame
    void Update()
    {

        if (Block.Select)
        {
            if (_blockColor == Color.white)
            {
                _blockColor = Block.Select.gameObject.GetComponent<Renderer>().material.color;

            }
            Block.Select.gameObject.GetComponent<Renderer>().material.color = Color32.Lerp(_blockColor, Color.black, Mathf.PingPong(Time.time, 0.5f));

        }

        if (Block.Select && Block.MoveTo)
        {
            if (CheckIfNear() == true)
            {
                if (_canSwap)
                {
                    _canSwap = false;
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


    bool CheckIfNear()
    {
        Block selectBlock = Block.Select.gameObject.GetComponent<Block>();
        Block moveToBlock = Block.MoveTo.gameObject.GetComponent<Block>();

        //Debug.Log("CheckIfNear вызван");
        if (selectBlock.Column - 1 == moveToBlock.Column && selectBlock.Row == moveToBlock.Row)
        {
            //Debug.Log("Блок слева");
            return true;
        }
        if (selectBlock.Column + 1 == moveToBlock.Column && selectBlock.Row == moveToBlock.Row)
        {
            //Debug.Log("Блок справа");
            return true;
        }
        if (selectBlock.Column == moveToBlock.Column && selectBlock.Row - 1 == moveToBlock.Row)
        {
            //Debug.Log("Блок внизу");
            return true;
        }

        if (selectBlock.Column == moveToBlock.Column && selectBlock.Row + 1 == moveToBlock.Row)
        {
            //Debug.Log("Блок сверху");
            return true;
        }
        Debug.Log("Функция CheckifNear не вернула TRUE");
        return false;
    }

    //IEnumerator Swap()
    //{
    //    Block sel = Block.Select.gameObject.GetComponent<Block>();
    //    Block mov = Block.MoveTo.gameObject.GetComponent<Block>();

    //    Vector3 selTempPos = sel.transform.position;
    //    Vector3 movTempPos = mov.transform.position;



    //    int columnSelect = sel.Column;
    //    int rowSelect = sel.Row;
    //    int columnMove = mov.Column;
    //    int rowMove = mov.Row;

    //    BoardGrid[columnSelect, rowSelect] = mov;
    //    mov.Column = columnSelect;
    //    mov.Row = rowSelect;

    //    BoardGrid[columnMove, rowMove] = sel;
    //    sel.Column = columnMove;
    //    sel.Row = rowMove;

    //    float time = 0;

    //    while (time < 1)
    //    {
    //        time += Time.deltaTime * SwapSpeed;
    //        sel.transform.position = Vector3.Lerp(selTempPos, movTempPos, time);
    //        mov.transform.position = Vector3.Lerp(movTempPos, selTempPos, time);
    //        yield return null;
    //    }

    //    _canSwap = true;

    //}


    List<BlockChain> DetectHorizontalMatches()
    {
        List<BlockChain> set = new List<BlockChain>();

        for (int row = 0; row < NumRows; row++)
        {
            for (int column = 0; column < NumColumns - 2; )
            {
                if (BoardGrid[column, row].BlockType != 404)
                {
                    int matchType = BoardGrid[column, row].BlockType;

                    if (BoardGrid[column + 1, row].BlockType == matchType &&
                    BoardGrid[column + 2, row].BlockType == matchType)
                    {
                        BlockChain chain = new BlockChain();
                        chain.ChainType = ChainType.ChainTypeHorizontal;
                        do
                        {
                            chain.AddBlock(BoardGrid[column, row]);
                            column += 1;
                        }
                        while (column < NumColumns && BoardGrid[column, row].BlockType == matchType);

                        set.Add(chain);
                        continue;

                    }
                }

                column += 1;
            }
        }
        return set;
    }

    List<BlockChain> DetectVerticalMatches()
    {
        List<BlockChain> set = new List<BlockChain>();

        for (int column = 0; column < NumColumns; column++)
        {
            for (int row = 0; row < NumRows - 2; )
            {
                if (BoardGrid[column, row].BlockType != 404)
                {
                    int matchType = BoardGrid[column, row].BlockType;

                    if (BoardGrid[column, row + 1].BlockType == matchType
                        && BoardGrid[column, row + 2].BlockType == matchType)
                    {
                        BlockChain chain = new BlockChain();
                        chain.ChainType = ChainType.ChainTypeVertical;

                        do
                        {
                            chain.AddBlock(BoardGrid[column, row]);
                            row += 1;
                        }

                        while (row < NumRows && BoardGrid[column, row].BlockType == matchType);

                        set.Add(chain);
                        continue;
                    }
                }
                row += 1;
            }
        }
        return set;
    }

    //List<BlockChain> PossibleRemoveMatches()
    //{
    //    List<BlockChain> horizontalChains = DetectHorizontalMatches();
    //    List<BlockChain> verticalChains = DetectVerticalMatches();

    //    List<BlockChain> ResultChains = new List<BlockChain>(horizontalChains);
    //    ResultChains.AddRange(verticalChains);

    //    return ResultChains;
    //}

    void MarkToRemoveBlocks(List<BlockChain> chains)
    {

        foreach (var chain in chains)
        {

            foreach (Block block in chain.Blocks())
            {
                BoardGrid[block.Column, block.Row].BlockType = 404;

            }


        }


    }

    bool CheckRemoveMatches()
    {
        List<BlockChain> chainsArray = RemoveMatches();

        if(chainsArray.Count >0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    List<BlockChain> RemoveMatches()
    {
        List<BlockChain> horizontalChains = DetectHorizontalMatches();
        List<BlockChain> verticalChains = DetectVerticalMatches();

        MarkToRemoveBlocks(horizontalChains);
        MarkToRemoveBlocks(verticalChains);

        List<BlockChain> ResultChains = new List<BlockChain>(horizontalChains);
        ResultChains.AddRange(verticalChains);

        return ResultChains;
    }




    void Respawn()
    {
        for (int x = 0; x < NumColumns; x++)
        {
            for (int y = 0; y < NumRows; y++)
            {
                if (BoardGrid[x, y].BlockType == 404)
                { //Spawn it only on destroyed block
                    BoardGrid[x, y].StartCoroutine("DestroyBlock");
                    CreateBlockAtColumn(x, y, Random.Range(0, BlockArray.Length - 1));


                }
            }
        }
    }



    //public void Restart()
    //{

    //    foreach (var b in blockList)
    //    {
    //        GameObject.Destroy(b);
    //    }
    //    for (int x = 0; x < NumColumns; x++)
    //    {
    //        for (int y = 0; y < NumRows; y++)
    //        {
    //            AddNewBlock(x, y);
    //        }
    //    }
    //    gameObject.transform.position = new Vector2(-6.0f, -6.0f);
    //}

    IEnumerator SwapBlockEffect(bool match)
    {

        Block sel = Block.Select.gameObject.GetComponent<Block>();
        Block mov = Block.MoveTo.gameObject.GetComponent<Block>();

        Vector3 selTempPos = sel.transform.position;
        Vector3 movTempPos = mov.transform.position;



        int columnSelect = sel.Column;
        int rowSelect = sel.Row;
        int columnMove = mov.Column;
        int rowMove = mov.Row;

        BoardGrid[columnSelect, rowSelect] = mov;
        mov.Column = columnSelect;
        mov.Row = rowSelect;

        BoardGrid[columnMove, rowMove] = sel;
        sel.Column = columnMove;
        sel.Row = rowMove;

        #region OldDataSwap
        //int tempx = sel.x;
        //int tempy = sel.y;
        //int tempid = sel.typeid;
        //swap data
        //sel.x = mov.x;
        //sel.y = mov.y;
        //sel.typeid = mov.typeid;

        //mov.x = tempx;
        //mov.y = tempy;
        //mov.typeid = tempid;



        //change id in board
        //boardgrid[sel.x, sel.y].typeid = sel.typeid;
        //boardgrid[mov.x, mov.y].typeid = mov.typeid;

        //boardgrid [sel.x, sel.y] = mov;
        //boardgrid [mov.x, mov.y] = sel;
        #endregion

        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * SwapSpeed;
            sel.transform.position = Vector3.Lerp(selTempPos, movTempPos, time);
            mov.transform.position = Vector3.Lerp(movTempPos, selTempPos, time);
            yield return null;
        }

        //Do we want to run the code to check for match?
        if (match == true)
        {
            //Check for match3
            if (CheckRemoveMatches())
            {

                //There is match
                _canSwap = true;
                Block.Select.gameObject.GetComponent<Renderer>().material.color = _blockColor;
                _blockColor = Color.white;
                Block.Select = null;
                Block.MoveTo = null;
                Respawn();
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
            _canSwap = true; //End effect
        }

    }

    //void MarkEmpty(int x, int num)
    //{


    //    for (int i = 0; i < num; i++)
    //    {
    //        BoardGrid[x, NumRows - 1 - i].BlockType = 404;
    //    }
    //}

    //void MoveDownBlocks()
    //{
    //    Block[] allB = FindObjectsOfType(typeof(Block)) as Block[];
    //    int moveDownBy = 0;

    //    for (int column = 0; column < NumColumns; column++)
    //    {
    //        for (int row = NumRows - 1; row >= 0; row--)
    //        {
    //            if (BoardGrid[column, row].BlockType == 404)
    //            {
    //                foreach (Block b in allB)
    //                {
    //                    if (b.Column == column && b.Row > row)
    //                    {
    //                        b.ReadyToMove = true;
    //                        b.Row -= 1;
    //                    }
    //                }
    //                moveDownBy++;
    //            }
    //        }

    //        foreach (Block b in allB)
    //        {
    //            if (b.ReadyToMove)
    //            {
    //                b.StartCoroutine(b.MoveDown(moveDownBy));
    //                b.ReadyToMove = false;
    //                BoardGrid[b.Column, b.Row] = b;
    //            }
    //        }

    //        MarkEmpty(column, moveDownBy);

    //        moveDownBy = 0;
    //    }

    //    Respawn();
    //}

}
