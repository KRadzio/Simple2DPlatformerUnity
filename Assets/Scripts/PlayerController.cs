
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 2.137f; // moving speed of the player
    [Space(10)]
    [Range(0.01f, 20.0f)] [SerializeField] private float jumpForce = 6.0f;
    [SerializeField] AudioClip bSound;

    private AudioSource source;
    private int keysFound = 0;
    private const int keysNumber = 3;
    private Animator animator;
    private bool IsWalking = false;
    private bool isFacingRight = true;
    private Vector2 startPosition;
    private Rigidbody2D rigidBody;
    public LayerMask groundLayer;
    const float rayLength = 1.5f;

    // Start is called before the first frame update


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bonus"))
        {
            GameManager.Instance.AddPoints(5);
           // Debug.Log("Score: " + score);
            other.gameObject.SetActive(false);
            source.PlayOneShot(bSound, AudioListener.volume);
        }     
        if (other.CompareTag("Exit"))
        {
            if (keysFound == keysNumber)
            {
                GameManager.Instance.AddPoints(100 * GameManager.Instance.GetLives());
                GameManager.Instance.LevelCompleted();
            }
                //Debug.Log("Level finished");
               
                //Debug.Log("You need to collect all the keys");
        }
        if(other.CompareTag("Enemy") && transform.position.y > other.gameObject.transform.position.y)
        {
            GameManager.Instance.AddPoints(10);
            GameManager.Instance.killOrzel();
        }    
        else if(other.CompareTag("Enemy"))
        {
            Death();
        }
        if(other.CompareTag("RedKey"))
        {
            keysFound++;
            GameManager.Instance.AddKeys(Color.red);
            other.gameObject.SetActive(false);
            source.PlayOneShot(bSound, AudioListener.volume);
        }
        if (other.CompareTag("BlueKey"))
        {
            keysFound++;
            GameManager.Instance.AddKeys(Color.blue);
            other.gameObject.SetActive(false);
            source.PlayOneShot(bSound, AudioListener.volume);
        }
        if (other.CompareTag("YellowKey"))
        {
            keysFound++;
            GameManager.Instance.AddKeys(Color.yellow);
            other.gameObject.SetActive(false);
            source.PlayOneShot(bSound, AudioListener.volume);
        }
        if (other.CompareTag("Heart"))
        {
            GameManager.Instance.AddLive();
            other.gameObject.SetActive(false);
            source.PlayOneShot(bSound, AudioListener.volume);
        }
        if(other.CompareTag("FallLevel"))
        {
            Death();
        }
        if(other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(other.transform);
        }
    }
    void Start()
    {
        
    }

    void Death()
    {
        GameManager.Instance.DecLive();
        transform.position = startPosition;
            
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x = -theScale.x;
        transform.localScale = theScale;
    }

    void Awake()
    {
        startPosition = transform.position;
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, groundLayer.value);
    }

    void Jump()
    {
        if(IsGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //Debug.Log("jumping");
        }
          
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.GS_GAME)
        {
            IsWalking = false;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                IsWalking = true;
                if (isFacingRight == false)
                    Flip();
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                IsWalking = true;
                if (isFacingRight)
                    Flip();
            }
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                Jump();
            animator.SetBool("IsGrounded", IsGrounded());
            animator.SetBool("IsWalking", IsWalking);
        }// Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1, false);
    }
}
