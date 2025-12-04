using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace _193251_193546
{
    public class MovingBlockerController : MonoBehaviour
    {
        private float moveSpeed = 1.0f;

        private Vector3 startPos;

        public float moveRange;




        public IEnumerator moveUp()
        {

            float distanceTraveled = 0.0f;

            while (distanceTraveled < moveRange)
            {
                transform.Translate(0.0f, moveSpeed * Time.deltaTime, 0.0f, Space.World);
                distanceTraveled += Time.deltaTime * moveSpeed;
                yield return null;
            }
            //transform.Translate(0.0f, moveSpeed  , 0.0f, Space.World);
        }

        public IEnumerator moveDown()
        {
            float distanceTraveled = transform.position.y;
            while (distanceTraveled > startPos.y)
            {
                transform.Translate(0.0f, -moveSpeed * Time.deltaTime, 0.0f, Space.World);
                distanceTraveled += Time.deltaTime * -moveSpeed;
                yield return null;
            }

            //   transform.Translate(0.0f, -moveSpeed , 0.0f, Space.World);
        }

        private void Awake()
        {
            startPos = transform.position;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}