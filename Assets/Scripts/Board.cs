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
                    StartCoroutine(Swap());
                    Block.Select = null;
                    Block.MoveTo = null;
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

    IEnumerator Swap()
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

        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * SwapSpeed;
            sel.transform.position = Vector3.Lerp(selTempPos, movTempPos, time);
            mov.transform.position = Vector3.Lerp(movTempPos, selTempPos, time);
            yield return null;
        }

        _canSwap = true;

    }




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

    List<BlockChain> PossibleRemoveMatches()
    {
        List<BlockChain> horizontalChains = DetectHorizontalMatches();
        List<BlockChain> verticalChains = DetectVerticalMatches();

        List<BlockChain> ResultChains = new List<BlockChain>(horizontalChains);
        ResultChains.AddRange(verticalChains);

        return ResultChains;
    }

    //bool RemoveBlocks(List<BlockChain> chains)
    //{
    //    if (chains.Count > 0)
    //    {
    //        foreach (var chain in chains)
    //        {

    //            foreach (Block block in chain.Blocks())
    //            {
    //                BoardGrid[block.Column, block.Row].BlockType = 404;

    //            }


    //        }
    //        Respawn();
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    //bool CheckMatch()
    //{
    //    //Get all blocks in scene
    //    Block[] allb = FindObjectsOfType(typeof(Block)) as Block[];
    //    Block sel = Block.Select.gameObject.GetComponent<Block>();
    //    Block mov = Block.MoveTo.gameObject.GetComponent<Block>();

    //    int countSU = 0; //Count Up
    //    int countSD = 0; //Count Down
    //    int countSL = 0; //Count Left
    //    int countSR = 0; //Count RIght

    //    //Left
    //    for (int l = sel.X - 1; l >= 0; l--)
    //    {
    //        if (BoardGrid[l, sel.Y] == sel.TypeID)
    //        {//If block have same ID
    //            countSL++;
    //        }
    //        if (BoardGrid[l, sel.Y] != sel.TypeID)
    //        {//If block doesn't have same ID
    //            //Stop
    //            break;
    //        }
    //    }
    //    //Right
    //    for (int r = sel.X; r < BoardWidth; r++)
    //    {
    //        if (BoardGrid[r, sel.Y] == sel.TypeID)
    //        {
    //            countSR++;
    //        }
    //        if (BoardGrid[r, sel.Y] != sel.TypeID)
    //        {
    //            break;
    //        }
    //    }
    //    //Down
    //    for (int d = sel.Y - 1; d >= 0; d--)
    //    {
    //        if (BoardGrid[sel.X, d] == sel.TypeID)
    //        {
    //            countSD++;
    //        }
    //        if (BoardGrid[sel.X, d] != sel.TypeID)
    //        {
    //            break;
    //        }
    //    }

    //    //Up
    //    for (int u = sel.Y; u < BoardHeight; u++)
    //    {
    //        if (BoardGrid[sel.X, u] == sel.TypeID)
    //        {
    //            countSU++;
    //        }

    //        if (BoardGrid[sel.X, u] != sel.TypeID)
    //        {
    //            break;
    //        }
    //    }

    //    int countMU = 0; //Count Up
    //    int countMD = 0; //Count Down
    //    int countML = 0; //Count Left
    //    int countMR = 0; //Count RIght

    //    //Left
    //    for (int l = mov.X - 1; l >= 0; l--)
    //    {
    //        if (BoardGrid[l, mov.Y] == mov.TypeID)
    //        {//If block have same ID
    //            countML++;
    //        }
    //        if (BoardGrid[l, mov.Y] != mov.TypeID)
    //        {//If block doesn't have same ID
    //            //Stop
    //            break;
    //        }
    //    }
    //    //Right
    //    for (int r = mov.X; r < BoardWidth; r++)
    //    {
    //        if (BoardGrid[r, mov.Y] == mov.TypeID)
    //        {
    //            countMR++;
    //        }
    //        if (BoardGrid[r, mov.Y] != mov.TypeID)
    //        {
    //            break;
    //        }
    //    }
    //    //Down
    //    for (int d = mov.Y - 1; d >= 0; d--)
    //    {
    //        if (BoardGrid[mov.X, d] == mov.TypeID)
    //        {
    //            countMD++;
    //        }
    //        if (BoardGrid[mov.X, d] != mov.TypeID)
    //        {
    //            break;
    //        }
    //    }

    //    //Up
    //    for (int u = mov.Y; u < BoardHeight; u++)
    //    {
    //        if (BoardGrid[mov.X, u] == mov.TypeID)
    //        {
    //            countMU++;
    //        }

    //        if (BoardGrid[mov.X, u] != mov.TypeID)
    //        {
    //            break;
    //        }
    //    }


    //    //Проверка цепочек у выбранного блока
    //    if (countML + countMR >= 3 || countMU + countMD >= 3 || countSL + countSR >= 3 || countSD + countSU >= 3)
    //    {
    //        if (countSL + countSR >= 3)
    //        {
    //            //Destroy and mark empty block
    //            for (int cl = 0; cl <= countSL; cl++)
    //            {
    //                foreach (Block b in allb)
    //                {
    //                    if (b.X == sel.X - cl && b.Y == sel.Y)
    //                    {
    //                        b.StartCoroutine("DestroyBlock");
    //                        BoardGrid[b.X, b.Y] = 404; ; //To mark empty block
    //                    }
    //                }
    //            }
    //            for (int cr = 0; cr < countSR; cr++)
    //            {
    //                foreach (Block b in allb)
    //                {
    //                    if (b.X == sel.X + cr && b.Y == sel.Y)
    //                    {
    //                        b.StartCoroutine("DestroyBlock");
    //                        BoardGrid[b.X, b.Y] = 404; ;
    //                    }
    //                }
    //            }
    //        }
    //        if (countSD + countSU >= 3)
    //        {
    //            for (int cd = 0; cd <= countSD; cd++)
    //            {
    //                foreach (Block b in allb)
    //                {
    //                    if (b.X == sel.X && b.Y == sel.Y - cd)
    //                    {
    //                        BoardGrid[b.X, b.Y] = 404; ;
    //                        b.StartCoroutine("DestroyBlock");
    //                    }
    //                }
    //            }
    //            for (int cu = 0; cu < countSU; cu++)
    //            {
    //                foreach (Block blc in allb)
    //                {
    //                    if (blc.X == sel.X && blc.Y == sel.Y + cu)
    //                    {
    //                        BoardGrid[blc.X, blc.Y] = 404; ;
    //                        blc.StartCoroutine("DestroyBlock");
    //                    }
    //                }
    //            }
    //        }

    //        if (countML + countMR >= 3)
    //        {
    //            //Destroy and mark empty block
    //            for (int cl = 0; cl <= countML; cl++)
    //            {
    //                foreach (Block b in allb)
    //                {
    //                    if (b.X == mov.X - cl && b.Y == mov.Y)
    //                    {
    //                        BoardGrid[b.X, b.Y] = 404; //To mark empty block
    //                        b.StartCoroutine("DestroyBlock");
    //                    }
    //                }
    //            }
    //            for (int cr = 0; cr < countMR; cr++)
    //            {
    //                foreach (Block b in allb)
    //                {
    //                    if (b.X == mov.X + cr && b.Y == mov.Y)
    //                    {

    //                        BoardGrid[b.X, b.Y] = 404; //To mark empty block
    //                        b.StartCoroutine("DestroyBlock");
    //                    }
    //                }
    //            }
    //        }

    //        if (countMU + countMD >= 3)
    //        {
    //            for (int cd = 0; cd < countMD; cd++)
    //            {
    //                foreach (Block blc in allb)
    //                {
    //                    if (blc.X == mov.X && blc.Y == mov.Y - cd)
    //                    {
    //                        BoardGrid[blc.X, blc.Y] = 404;
    //                        blc.StartCoroutine("DestroyBlock");
    //                    }
    //                }
    //            }

    //            for (int cu = 0; cu < countMU; cu++)
    //            {
    //                foreach (Block blc in allb)
    //                {
    //                    if (blc.X == mov.X && blc.Y == mov.Y + cu)
    //                    {
    //                        BoardGrid[blc.X, blc.Y] = 404;
    //                        blc.StartCoroutine("DestroyBlock");
    //                    }
    //                }
    //            }
    //        }
    //        Respawn();
    //        return true;

    //    }
    //    return false;
    //}




    //void Respawn()
    //{
    //    for (int x = 0; x < NumColumns; x++)
    //    {
    //        for (int y = 0; y < NumRows; y++)
    //        {
    //            if (BoardGrid[x, y].BlockType == 404)
    //            { //Spawn it only on destroyed block
    //                BoardGrid[x,y].StartCoroutine("DestroyBlock");
    //                AddNewBlock(x, y);
    //                RemoveBlocks(PossibleRemoveMatches());

    //            }
    //        }
    //    }
    //}

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

        ////Do we want to run the code to check for match?
        //if (match == true)
        //{
        //    //Check for match3
        //    if (RemoveBlocks(PossibleRemoveMatches()) == true)
        //    {

        //        //There is match
        //        _swapEffect = false;//End effect
        //        Block.Select.gameObject.GetComponent<Renderer>().material.color = _blockColor;
        //        _blockColor = Color.white;
        //        Block.Select = null;
        //        Block.MoveTo = null;
        //    }
        //    else
        //    {
        //        //There is no match, return them in their default position
        //        StartCoroutine(SwapBlockEffect(false));//Swap their position and data using new swap effect, without checking for match3
        //        Block.Select.gameObject.GetComponent<Renderer>().material.color = _blockColor;
        //        _blockColor = Color.white;
        //        Block.Select = null;
        //        Block.MoveTo = null;
        //    }
        //}
        //else
        //{//We don't
        //    _swapEffect = false; //End effect
        //}

    }

}
