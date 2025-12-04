using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193251_193546
{

    public class LeverController : MonoBehaviour
    {
        private Animator animator;
        private Rigidbody2D rigidBody;

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                animator.SetBool("isClosed", true);
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
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