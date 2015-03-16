using UnityEngine;
using System.Collections;

namespace BlockNS
{

    

public class Block : MonoBehaviour {

    public int BlockType;

	public int Column;
	public int Row;

    private float _startTime;

	public static Transform Select;
	public static Transform MoveTo;

	private Vector2 _myScale;
    

    public bool NeedFall;
    public bool ReadyToMove;

	// Use this for initialization
	void Start () {
		_myScale = transform.localScale;
        _startTime = Time.time;
       if(NeedFall)
       {
           StartCoroutine("FallDown");
       }
        
	}
	
	// Update is called once per frame
	void Update () {
	if(!NeedFall && Time.time - _startTime<3)
    {
        if(!NeedFall)
        {
            transform.localScale = Vector2.Lerp(Vector2.zero, _myScale, (Time.time - _startTime));
        }
        if(NeedFall)
        {
            transform.localScale = Vector2.Lerp(Vector2.zero, _myScale, (Time.time - _startTime) * 5);
        }
    }
	}

	void OnMouseOver()
	{
		transform.localScale = new Vector2 (_myScale.x + 0.2f, _myScale.y + 0.2f);

			if(Input.GetButtonDown("Fire1"))
		{
		    Debug.Log (string.Format("Выбран блок: {0}:{1} - {2} [{3}-{4}]",Column,Row,BlockType,transform.position.x,transform.position.y));
			if(!Select)
			{
				Select = transform;

			}
			else if (Select != transform && !MoveTo)
			{
				MoveTo = transform;
			}
		}
	}

	void OnMouseExit()
	{
		transform.localScale = _myScale;
	}

    public IEnumerator DestroyBlock()
    {
        Vector3 lastScaleSize = transform.localScale;
        Vector3 lastPosition = transform.localPosition;
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(lastScaleSize, Vector3.zero, time);
            yield return null;
        }

        Destroy(gameObject);


    }

    public IEnumerator RespawnBlock()
    {
        Vector3 lastScaleSize = transform.localScale;
        float time = 0;

        while(time < 1)
        {
            time += Time.deltaTime;

            transform.localScale = Vector3.Lerp(Vector3.zero, lastScaleSize, time);
            yield return null;
        }


    }

    public IEnumerator MoveDown(int rows)
    {
        Vector3 lastPos = transform.position;
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y - rows*1.95f, transform.position.z);
        float time = 0;

        while(time<1)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(lastPos, newPos, time);
            yield return null;
        }
    }

    public IEnumerator FallDown()
    {
        
        Vector3 newPos = transform.position;
        Vector3 lastPos = new Vector3(transform.position.x, transform.position.y + 15f,transform.position.z);

        float time = 0;

        while(time<1)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(lastPos, newPos, time);
            yield return null;
        }

    }
}
}