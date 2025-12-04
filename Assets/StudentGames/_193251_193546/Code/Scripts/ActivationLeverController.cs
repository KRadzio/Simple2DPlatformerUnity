using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace _193251_193546 { 
public class ActivationLeverController : MonoBehaviour
{
    public GameObject[] objects;
    public MovingBlockerController[] blockerConts;
    public MovingBlockerSideController[] blockerSideConts;

    private Animator animator;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.GetComponent<CircleCollider2D>().enabled = false;
            animator.SetBool("isClosed", true);
            for (int i = 0; i < blockerSideConts.Length; i++)
            {
               StartCoroutine(blockerSideConts[i].moveRight());
            }
            for (int i = 0; i < blockerConts.Length; i++)
            {
                StartCoroutine(blockerConts[i].moveUp());
            }
        }

    }

    private void Awake()
    {
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