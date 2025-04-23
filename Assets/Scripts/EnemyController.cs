using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private bool isFacingRight = true;
    private bool isMovingRight = false;
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 2.137f;
    private Animator animator;
    private float startPositionX;
    public float moveRange = 1.0f;


    IEnumerator KillOnAnimationEnd()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && transform.position.y < other.gameObject.transform.position.y)
        {
            animator.SetBool("IsDead",true);
            StartCoroutine(KillOnAnimationEnd());
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x = -theScale.x;
        transform.localScale = theScale;
    }

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
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingRight)
        {
            if (this.transform.position.x <= startPositionX + moveRange)
                MoveRight();
            else
            {
                isMovingRight = false;
                Flip();
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
                Flip();
                MoveRight();
            }
        }
    }
}
