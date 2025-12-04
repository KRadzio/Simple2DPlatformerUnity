using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace _193251_193546
{
    public class VerticalEnemyController : MonoBehaviour
    {
        private bool isMovingUp = false;
        [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 4.0f;
        private Animator animator;
        private float startPositionY;
        public float moveRange = 2.0f;
        public bool isFacingRight = true;
        private bool isDead = false;
        private AudioSource source;
        [SerializeField] AudioClip deathSound;


        IEnumerator KillOnAnimationEnd()
        {
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActive(false);
        }

        IEnumerator FadeAndDestroy(GameObject obj)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            float elapsedTime = 0.0f;
            float fadeDuration = 0.5f;
            Color startColor = renderer.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

            while (elapsedTime < fadeDuration)
            {
                renderer.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            obj.SetActive(false);
            Destroy(obj);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && transform.position.y < other.gameObject.transform.position.y + 0.5f)
            {
                GameManager.Instance.AddPoints(10);
                GameManager.Instance.killOrzel();
                isDead = true;
                this.GetComponent<PolygonCollider2D>().enabled = false;
                animator.SetBool("IsDead", true);
                source.PlayOneShot(deathSound, AudioListener.volume);
                StartCoroutine(KillOnAnimationEnd());
                StartCoroutine(FadeAndDestroy(this.gameObject));
            }
            if (other.CompareTag("Bullet"))
            {
                GameManager.Instance.AddPoints(10);
                GameManager.Instance.killOrzel();
                isDead = true;
                this.GetComponent<PolygonCollider2D>().enabled = false;
                other.gameObject.SetActive(false);
                animator.SetBool("IsDead", true);
                source.PlayOneShot(deathSound, AudioListener.volume);
                StartCoroutine(KillOnAnimationEnd());
                StartCoroutine(FadeAndDestroy(this.gameObject));
            }
        }

        void Flip()
        {
            Vector3 theScale = transform.localScale;
            theScale.x = -theScale.x;
            transform.localScale = theScale;
        }

        void MoveUpwards()
        {
            transform.Translate(0.0f, moveSpeed * Time.deltaTime, 0.0f, Space.World);
        }

        void MoveDownwards()
        {
            transform.Translate(0.0f, -moveSpeed * Time.deltaTime, 0.0f, Space.World);
        }
        void Awake()
        {
            startPositionY = this.transform.position.y;
            animator = GetComponent<Animator>();
            source = GetComponent<AudioSource>();
            if (!isFacingRight)
                Flip();
        }
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!isDead)
            {
                if (isMovingUp)
                {
                    if (this.transform.position.y <= startPositionY + moveRange)
                        MoveUpwards();
                    else
                    {
                        isMovingUp = false;
                        MoveDownwards();
                    }
                }
                else
                {
                    if (this.transform.position.y >= startPositionY - moveRange)
                    {
                        MoveDownwards();
                    }
                    else
                    {
                        isMovingUp = true;
                        MoveUpwards();
                    }
                }
            }

        }
    }
}