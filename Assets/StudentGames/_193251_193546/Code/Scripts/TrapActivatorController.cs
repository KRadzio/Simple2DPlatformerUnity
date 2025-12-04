using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193251_193546
{
    public class TrapActivatorController : MonoBehaviour
    {
        public GameObject[] traps;
        public FallingBlockerController[] fallingBlockerConts;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                for (int i = 0; i < fallingBlockerConts.Length; i++)
                {
                    fallingBlockerConts[i].fall();
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