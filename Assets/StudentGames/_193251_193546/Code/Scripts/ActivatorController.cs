using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193251_193546
{
    public class ActivatorController : MonoBehaviour
    {
        public GameObject[] blockers;
        public MovingBlockerController[] blockConts;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PushableCreate"))
            {
                for (int i = 0; i < blockConts.Length; i++)
                {
                    StartCoroutine(blockConts[i].moveUp());
                }
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("PushableCreate"))
            {
                for (int i = 0; i < blockConts.Length; i++)
                {
                    StartCoroutine(blockConts[i].moveDown());
                }
            }

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