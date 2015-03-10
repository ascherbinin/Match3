using UnityEngine;
using System.Collections;

namespace BlockNS
{


public class Block : MonoBehaviour {

    public int TypeID;

	public int X;
	public int Y;

	public static Transform Select;
	public static Transform MoveTo;

	private Vector2 _myScale;
    private Color _blockColor;

	// Use this for initialization
	void Start () {
		_myScale = transform.localScale;
        _blockColor = this.gameObject.GetComponent<Renderer>().material.color;
        StartCoroutine(RespawnBlock());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver()
	{
		transform.localScale = new Vector2 (_myScale.x + 0.2f, _myScale.y + 0.2f);

			if(Input.GetButtonDown("Fire1"))
		{
		    Debug.Log (string.Format("Выбран блок: {0}:{1} - {2} [{3}-{4}]",X,Y,TypeID,transform.position.x,transform.position.y));
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
            //transform.localPosition = Vector3.Lerp(lastPosition, new Vector3(lastPosition.x,lastPosition.y-1f,lastPosition.z), time);
            transform.gameObject.GetComponent<Renderer>().material.color = Color32.Lerp(_blockColor, new Color32(0, 0, 0, 0), time);
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
}
}