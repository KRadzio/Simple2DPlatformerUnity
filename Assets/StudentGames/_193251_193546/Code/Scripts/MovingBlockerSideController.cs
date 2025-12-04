using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193251_193546
{
    public class MovingBlockerSideController : MonoBehaviour
    {
        public float moveRange = 2.5f;
        private float moveSpeed = 1.0f;
        public IEnumerator moveRight()
        {
            float distanceTraveled = 0.0f;

            while (distanceTraveled < moveRange)
            {
                transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                distanceTraveled += Time.deltaTime * moveSpeed;
                yield return null;
            }
            // transform.Translate(moveSpeed, 0.0f, 0.0f, Space.World);
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