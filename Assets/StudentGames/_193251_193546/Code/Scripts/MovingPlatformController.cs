using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193251_193546
{

    public class NewBehaviourScript : MonoBehaviour
    {
        private bool isMovingRight = false;
        [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 2.137f;
        private float startPositionX;
        public float moveRange = 2.0f;


        void MoveRight()
        {
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }

        void MoveLeft()
        {
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }
        void Awake()
        {
            startPositionX = this.transform.position.x;
        }
        void Start()
        {
        }

        void Update()
        {
            if (isMovingRight)
            {
                if (this.transform.position.x <= startPositionX + moveRange)
                    MoveRight();
                else
                {
                    isMovingRight = false;
                    MoveLeft();
                }
            }
            else
            {
                if (this.transform.position.x >= startPositionX - moveRange)
                {
                    MoveLeft();
                }
                else
                {
                    isMovingRight = true;
                    MoveRight();
                }
            }
        }
    }
}