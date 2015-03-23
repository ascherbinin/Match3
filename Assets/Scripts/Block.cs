using UnityEngine;
using System.Collections;

namespace BlockNS
{



    public class Block : MonoBehaviour
    {

        public int BlockType;

        public int Column;
        public int Row;

        private float _startTime;

        public static Transform Select;
        public static Transform MoveTo;

        private Vector2 _myScale;
        private bool _isSelected = false;


        private Animator _animator;



        public bool NeedFall;
        public bool ReadyToMove;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        // Use this for initialization
        void Start()
        {
            _myScale = transform.localScale;
            _startTime = Time.time;

            if (NeedFall)
            {
                StartCoroutine("FallDown");
            }

        }

        
        void Update()
        {
            if (!NeedFall && Time.time - _startTime < 3)
            {
                if (!NeedFall)
                {
                    transform.localScale = Vector2.Lerp(Vector2.zero, _myScale, (Time.time - _startTime));
                }
                if (NeedFall)
                {
                    transform.localScale = Vector2.Lerp(Vector2.zero, _myScale, (Time.time - _startTime) * 5);
                }

             
            }

            if (_isSelected)
            {
                _animator.SetBool("blockSelected",true);
            }
            else
            {
                _animator.SetBool("blockSelected", false);
            }
        }

        void OnMouseOver()
        {


            _isSelected = true;

            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log(string.Format("Выбран блок: {0}:{1} - {2} [{3}-{4}]", Column, Row, BlockType, transform.position.x, transform.position.y));
                if (!Select)
                {
                    Select = transform;
                    
                }
                else if (Select != transform && !MoveTo)
                {
                    MoveTo = transform;
                    
                }
            }

        }


        private void OnMouseExit()
        {
            if (Select != transform)
            {
                _isSelected = false;
            }
        }



        public IEnumerator DestroyBlock()
        {
            Vector3 lastScaleSize = transform.localScale;
            
            float time = 0;

            while (time < 1)
            {
                time += Time.deltaTime;
                transform.localScale = Vector3.Lerp(lastScaleSize, Vector3.zero, time);
                yield return null;
            }

            Destroy(gameObject);


        }

       

        public IEnumerator MoveDown(int rows)
        {
            Vector3 lastPos = transform.position;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y - rows * 1.95f, transform.position.z);
            float time = 0;

            while (time < 1)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(lastPos, newPos, time);
                yield return null;
            }
        }

        public IEnumerator FallDown()
        {

            Vector3 newPos = transform.position;
            Vector3 lastPos = new Vector3(transform.position.x, transform.position.y + 15f, transform.position.z);

            float time = 0;

            while (time < 1)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(lastPos, newPos, time);
                yield return null;
            }

        }
    }
}