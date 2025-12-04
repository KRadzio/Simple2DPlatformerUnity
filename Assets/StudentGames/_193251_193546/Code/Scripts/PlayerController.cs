
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace _193251_193546
{

    public class PlayerController : MonoBehaviour
    {

        [Header("Movement parameters")]
        [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 6.0f; // moving speed of the player
        [Space(10)]
        [Range(0.01f, 20.0f)][SerializeField] private float jumpForce = 6.0f;
        [SerializeField] AudioClip bSound;
        [SerializeField] AudioClip deathSound;
        [SerializeField] AudioClip keySound;
        [SerializeField] AudioClip secretDoorSound;
        [SerializeField] AudioClip ammoCrateSound;
        [SerializeField] AudioClip shootSound;

        private AudioSource source;
        private int keysFound = 0;
        private const int keysNumber = 3;
        private Animator animator;
        private bool IsWalking = false;
        private bool isFacingRight = true;
        private bool isRising = false;
        private bool isFalling = false;
        private Vector2 startPosition;
        private Rigidbody2D rigidBody;
        public LayerMask groundLayer;
        const float rayLength = 1.5f;
        Vector3 offset = new Vector3(0.4f, 0.0f, 0.0f);
        //  public GameObject secretDoor;
        private bool leverPressed = false;
        private Renderer playerRenderer;
        private bool secretDoorIsMoving = false;

        public GameObject bulletPrefab;

        bool isLadder = false;
        bool isClimbing = false;
        private float vertical;

        private PlayerInput playerInput;

        IEnumerator FadeAfterCollision()
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            float elapsedTime = 0.0f;
            float fadeDuration = 0.5f;
            Color startColor = renderer.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

            for (int i = 0; i < 2; i++)
            {
                while (elapsedTime < fadeDuration)
                {
                    renderer.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                elapsedTime = 0;
                renderer.color = endColor;
                while (elapsedTime < fadeDuration)
                {
                    renderer.color = Color.Lerp(endColor, startColor, elapsedTime / fadeDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                renderer.color = startColor;
                elapsedTime = 0;
            }

        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("MovingPlatform"))
            {
                if (GameManager.Instance.getCurrGameState() == GameState.GS_GAME)
                {
                    transform.SetParent(null);
                }

            }

            if (other.CompareTag("Ladder"))
            {
                isLadder = false;
                isClimbing = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bonus"))
            {
                GameManager.Instance.AddPoints(5);
                other.gameObject.SetActive(false);
                source.PlayOneShot(bSound, AudioListener.volume);
            }
            if (other.CompareTag("SuperBonus"))
            {
                GameManager.Instance.AddPoints(100);
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
                else
                {
                    StartCoroutine(GameManager.Instance.showExitMessage());
                }

            }
            if (other.CompareTag("Enemy") && transform.position.y > other.gameObject.transform.position.y + 0.5f)
            {
                rigidBody.AddForce(Vector2.up * -rigidBody.velocity.y, ForceMode2D.Impulse);
                // Debug.Log(rigidBody.velocity);
                rigidBody.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
            }
            else if (other.CompareTag("Enemy") || other.CompareTag("SlimeEnemy"))
            {
                if (GameManager.Instance.GetArmor() > 0)
                {
                    if (GameManager.Instance.GetArmorCooldown() <= 0)
                    {
                        GameManager.Instance.SubtractArmor();
                        StartCoroutine(FadeAfterCollision());
                    }
                }
                else if (GameManager.Instance.GetArmorCooldown() <= 0)
                    Death();
            }
            /*
            else if(other.CompareTag("Enemy"))
            {
                if (GameManager.Instance.GetArmor() > 0)
                {
                    if (GameManager.Instance.GetArmorCooldown() <= 0)
                    {
                        GameManager.Instance.SubtractArmor();
                        StartCoroutine(FadeAfterCollision());
                    }
                }
                else if (GameManager.Instance.GetArmorCooldown() <= 0)
                    Death();
            }*/


            if (other.CompareTag("RedKey"))
            {
                keysFound++;
                GameManager.Instance.AddKeys(Color.red);
                other.gameObject.SetActive(false);
                source.PlayOneShot(keySound, AudioListener.volume);
            }
            if (other.CompareTag("BlueKey"))
            {
                keysFound++;
                GameManager.Instance.AddKeys(Color.blue);
                other.gameObject.SetActive(false);
                source.PlayOneShot(keySound, AudioListener.volume);
            }
            if (other.CompareTag("YellowKey"))
            {
                keysFound++;
                GameManager.Instance.AddKeys(Color.yellow);
                other.gameObject.SetActive(false);
                source.PlayOneShot(keySound, AudioListener.volume);
            }
            if (other.CompareTag("Heart"))
            {
                GameManager.Instance.AddLive();
                other.gameObject.SetActive(false);
                source.PlayOneShot(bSound, AudioListener.volume);
            }
            if (other.CompareTag("FallLevel"))
                Death();
            if (other.CompareTag("MovingPlatform"))
            {
                if (other != null)
                    transform.SetParent(other.transform);
            }

            if (other.CompareTag("CutSceneLever") && leverPressed == false)
                OpenDoor();

            if (other.CompareTag("Ladder"))
                isLadder = true;

            if (other.CompareTag("SuperJump"))
            {
                rigidBody.AddForce(Vector2.up * -rigidBody.velocity.y, ForceMode2D.Impulse);
                rigidBody.AddForce(Vector2.up * 30, ForceMode2D.Impulse);
            }


            if (other.CompareTag("AmmoChest"))
            {
                int nowAmmo = GameManager.Instance.GetAmmo();
                if (nowAmmo < 20)
                {
                    StartCoroutine(GameManager.Instance.showAmmoMessage());
                    source.PlayOneShot(ammoCrateSound, AudioListener.volume);
                    nowAmmo = nowAmmo + 10 <= 20 ? nowAmmo + 10 : 20;
                    GameManager.Instance.SetAmmo(nowAmmo);
                    other.gameObject.SetActive(false);
                }
            }

            if (other.CompareTag("Armor"))
            {
                if (GameManager.Instance.GetArmor() < 2)
                {
                    source.PlayOneShot(ammoCrateSound, AudioListener.volume);
                    GameManager.Instance.TakeArmor();
                    other.gameObject.SetActive(false);
                }
            }

            if (other.CompareTag("Secret"))
            {
                other.gameObject.SetActive(false);
                source.PlayOneShot(secretDoorSound, AudioListener.volume);
                GameManager.Instance.incSecretCount();
            }

            if (other.CompareTag("EnemyBullet"))
            {
                other.gameObject.SetActive(false);
                if (GameManager.Instance.GetArmor() > 0)
                {
                    if (GameManager.Instance.GetArmorCooldown() <= 0)
                    {
                        GameManager.Instance.SubtractArmor();
                        StartCoroutine(FadeAfterCollision());
                    }
                }
                else if (GameManager.Instance.GetArmorCooldown() <= 0)
                    Death();
            }
            if (other.CompareTag("FallingBlocker"))
            {
                if (other.GetComponent<Rigidbody2D>().velocity.y < -1.0f)
                    Death();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if ((other.CompareTag("Enemy") || other.CompareTag("SlimeEnemy")) && transform.position.y <= other.gameObject.transform.position.y + 0.5f)
            {
                if (GameManager.Instance.GetArmor() > 0)
                {
                    if (GameManager.Instance.GetArmorCooldown() <= 0)
                    {
                        GameManager.Instance.SubtractArmor();
                        StartCoroutine(FadeAfterCollision());
                    }
                }
                else if (GameManager.Instance.GetArmorCooldown() <= 0)
                    Death();
            }
            /*
            if (other.CompareTag("Enemy") && transform.position.y <= other.gameObject.transform.position.y + 0.5f)
            {
                if (GameManager.Instance.GetArmor() > 0)
                {
                    if (GameManager.Instance.GetArmorCooldown() <= 0)
                    {
                        GameManager.Instance.SubtractArmor();
                        StartCoroutine(FadeAfterCollision());
                    }
                }
                else if (GameManager.Instance.GetArmorCooldown() <= 0)
                    Death();
            }*/

            if (other.CompareTag("AmmoChest"))
            {
                int nowAmmo = GameManager.Instance.GetAmmo();
                if (nowAmmo < 20)
                {
                    StartCoroutine(GameManager.Instance.showAmmoMessage());
                    source.PlayOneShot(ammoCrateSound, AudioListener.volume);
                    nowAmmo = nowAmmo + 10 <= 20 ? nowAmmo + 10 : 20;
                    GameManager.Instance.SetAmmo(nowAmmo);
                    other.gameObject.SetActive(false);
                }
            }

            if (other.CompareTag("Armor"))
            {
                if (GameManager.Instance.GetArmor() < 2)
                {
                    source.PlayOneShot(ammoCrateSound, AudioListener.volume);
                    GameManager.Instance.TakeArmor();
                    other.gameObject.SetActive(false);
                }
            }
        }

        void Start()
        {

        }

        void Shoot()
        { // is trigger w pocisku sprawia ¿e przenika przez œciany i obiekty
            if (!isClimbing)
            {
                if (GameManager.Instance.GetAmmo() != 0)
                {
                    GameManager.Instance.SubtractAmmo();
                    float offset;
                    if (isFacingRight)
                        offset = 0.25f;
                    else
                        offset = -0.25f;
                    GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    bullet.transform.Translate(offset, 0.0f, 0.0f, Space.World);
                    Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                    if (isFacingRight)
                        bulletRb.AddForce(transform.right * 20.0f, ForceMode2D.Impulse);
                    else
                        bulletRb.AddForce(-1 * transform.right * 20.0f, ForceMode2D.Impulse);
                    source.PlayOneShot(shootSound, AudioListener.volume);
                    Destroy(bullet, 5.0f);
                }
            }
        }

        IEnumerator MoveCameraToDoorPosition() // tutaj statyczne 
        {
            var position = this.transform.position;

            yield return new WaitForSeconds(1.0f);

            secretDoorIsMoving = true;

            playerRenderer.enabled = false;

            //  transform.position = secretDoor.transform.position;

            rigidBody.bodyType = RigidbodyType2D.Static;

            this.GetComponent<BoxCollider2D>().enabled = false;

            yield return new WaitForSeconds(1.0f);

            source.PlayOneShot(secretDoorSound, AudioListener.volume);

            yield return new WaitForSeconds(0.5f);

            //    secretDoor.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;


            yield return new WaitForSeconds(3.0f);

            secretDoorIsMoving = false;

            this.GetComponent<BoxCollider2D>().enabled = true;

            rigidBody.bodyType = RigidbodyType2D.Dynamic;

            playerRenderer.enabled = true;

            this.transform.position = position;

            //    secretDoor.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        }

        void OpenDoor()
        {
            StartCoroutine(MoveCameraToDoorPosition());
            leverPressed = true;
        }

        void Death()
        {
            GameManager.Instance.setArmor(0);
            GameManager.Instance.DecLive();
            if (GameManager.Instance.GetLives() > 0)
                transform.position = startPosition;
            else
                this.enabled = false;
            source.PlayOneShot(deathSound, AudioListener.volume);

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
            //   secretDoor = GameObject.FindGameObjectsWithTag("SecretDoor").FirstOrDefault();
            playerRenderer = GetComponent<SpriteRenderer>();
            playerInput = GetComponent<PlayerInput>();
        }

        bool IsGrounded()
        {
            return (Physics2D.Raycast(this.transform.position - offset, Vector2.down * 0.5f, rayLength, groundLayer.value) || Physics2D.Raycast(this.transform.position + offset, Vector2.down * 0.5f, rayLength, groundLayer.value));
        }

        void Jump()
        {
            if (IsGrounded() && rigidBody.velocity.y < 1)
            {
                rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isRising = true;
            }

        }

        private void FixedUpdate()
        {
            if (isClimbing)
            {
                rigidBody.gravityScale = 0;
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, vertical * moveSpeed);
            }
            else
            {
                rigidBody.gravityScale = 1;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance.currentGameState == GameState.GS_GAME)
            {
                Vector2 joystickInput = playerInput.actions["Move"].ReadValue<Vector2>();
                IsWalking = false;
                if (!secretDoorIsMoving)
                {
                    if (joystickInput.x != 0.0f)
                    {
                        transform.Translate(joystickInput.x * moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                        IsWalking = true;
                        if (isFacingRight != (joystickInput.x > 0.0f))
                            Flip();
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || joystickInput.x > 0.0f)
                        {
                            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                            IsWalking = true;
                            if (isFacingRight == false)
                                Flip();
                        }

                        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || joystickInput.x < 0.0f)
                        {
                            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                            IsWalking = true;
                            if (isFacingRight)
                                Flip();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Space) || playerInput.actions["Jump"].WasPressedThisFrame())
                        Jump();
                    if (Input.GetKeyDown(KeyCode.X) || playerInput.actions["Shoot"].WasPressedThisFrame())
                        Shoot();
                }
                vertical = Input.GetAxis("Vertical");

                if (isLadder && vertical != 0)
                {
                    isClimbing = true;
                    isFalling = false;
                    isRising = false;
                }
                if (rigidBody.velocity.y > 0 && !isClimbing)
                {
                    isRising = true;
                }
                if (rigidBody.velocity.y < 0 && !isClimbing)
                {
                    isRising = false;
                    isFalling = true;
                }

                else if (rigidBody.velocity.y == 0)
                {
                    isFalling = false;
                    isRising = false;
                }
                //  Debug.Log(rigidBody.velocity);
                animator.SetBool("IsGrounded", IsGrounded());
                animator.SetBool("IsWalking", IsWalking);
                animator.SetBool("IsFalling", isFalling);
                animator.SetBool("IsRising", isRising);
                animator.SetBool("IsClimbing", isClimbing);
                //  Debug.DrawRay(transform.position - offset, rayLength * Vector3.down, Color.red, 0.1f, false);
                // Debug.DrawRay(transform.position + offset, rayLength * Vector3.down, Color.red, 0.1f, false);
            }

            if (playerInput.actions["Pause"].WasPressedThisFrame())
            {
                if (GameManager.Instance.currentGameState == GameState.GS_PAUSEMENU)
                {
                    GameManager.Instance.InGame();
                }

                else if (GameManager.Instance.currentGameState == GameState.GS_GAME)
                {
                    GameManager.Instance.PauseMenu();
                }
            }
            if (playerInput.actions["Help"].WasPressedThisFrame())
            {
                if (GameManager.Instance.currentGameState == GameState.GS_HELPMENU)
                {
                    GameManager.Instance.InGame();
                }

                else if (GameManager.Instance.currentGameState == GameState.GS_GAME)
                {
                    GameManager.Instance.HelpMenu();
                }
            }
        }
    }

}