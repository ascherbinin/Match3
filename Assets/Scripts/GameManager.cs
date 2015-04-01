using BlockNS;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GameManager : MonoBehaviour {

    public float SwapSpeed;

    private bool _canSwap = true;

	// Use this for initialization
	void Start () {
        BoardManager.instance.GenerateBoard();
	}
	
	// Update is called once per frame
	void Update () {


        if (Block.Select && Block.MoveTo)
        {
            if (BoardManager.instance.CheckIfNear() == true)
            {
                if (_canSwap)
                {
                    _canSwap = false;
                    SwapBlock(false);
                    if (BoardManager.instance.CheckRemoveMatches())
                    {
                        StartCoroutine(BoardManager.instance.Respawn());
                        _canSwap = true;
                        Block.Select = null;
                        Block.MoveTo = null;
                    }
                    else
                    {
                        SwapBlock(true);
                        _canSwap = true;
                        Block.Select = null;
                        Block.MoveTo = null;
                    }


                }

            }
            else
            {


                Block.Select = null;
                Block.MoveTo = null;
            }


        }

        while (BoardManager.instance.CheckRemoveMatches())
        {

            StartCoroutine(BoardManager.instance.Respawn());
        }


	}

    void SwapBlock(bool needBackSwap)
    {
        Block sel = Block.Select.gameObject.GetComponent<Block>();
        Block mov = Block.MoveTo.gameObject.GetComponent<Block>();


        if (!needBackSwap)
        {
            StartCoroutine(SwapBlockEffect());
        }
        else
        {
            StartCoroutine(BackSwapBlockEffect());
        }

        int columnSelect = sel.Column;
        int rowSelect = sel.Row;
        int columnMove = mov.Column;
        int rowMove = mov.Row;

        BoardManager.instance.BoardGrid[columnSelect, rowSelect] = mov;
        mov.Column = columnSelect;
        mov.Row = rowSelect;

        BoardManager.instance.BoardGrid[columnMove, rowMove] = sel;
        sel.Column = columnMove;
        sel.Row = rowMove;



    }



    IEnumerator SwapBlockEffect()
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




    }

    IEnumerator BackSwapBlockEffect()
    {
        Block sel = Block.Select.gameObject.GetComponent<Block>();
        Block mov = Block.MoveTo.gameObject.GetComponent<Block>();

        Vector3 selTempPos = mov.transform.position;
        Vector3 movTempPos = sel.transform.position;


        float time = 0;

        yield return StartCoroutine(SwapBlockEffect());

        while (time < 1)
        {
            time += Time.deltaTime * SwapSpeed;
            sel.transform.position = Vector3.Lerp(selTempPos, movTempPos, time);
            mov.transform.position = Vector3.Lerp(movTempPos, selTempPos, time);

            yield return null;
        }
    }

}
