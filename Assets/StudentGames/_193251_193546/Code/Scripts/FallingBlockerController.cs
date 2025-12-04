using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193251_193546
{
    public class FallingBlockerController : MonoBehaviour
    {

        private Rigidbody2D rigidBody;

        public void fall()
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            rigidBody.mass = 15;
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
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