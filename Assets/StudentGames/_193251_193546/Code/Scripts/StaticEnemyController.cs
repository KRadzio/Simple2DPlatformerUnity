using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace _193251_193546
{
    public class StaticEnemyController : MonoBehaviour
    {
        private Animator animator;
        public GameObject bulletPrefab;
        public ShootingRangeController rangeBox;
        private float shootForece = 1.5f;
        private float currShootCooldown = 0.0f;
        private float shootCooldown = 2.0f;
        private BoxCollider2D hitbox;

        public bool isFacingLeft;

        private AudioSource source;
        [SerializeField] AudioClip deathSound;
        [SerializeField] AudioClip shootSound;

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
                hitbox.enabled = false;
                animator.SetBool("IsDead", true);
                source.PlayOneShot(deathSound, AudioListener.volume);
                StartCoroutine(KillOnAnimationEnd());
                StartCoroutine(FadeAndDestroy(this.gameObject));
            }
            if (other.CompareTag("Bullet"))
            {
                GameManager.Instance.AddPoints(10);
                GameManager.Instance.killOrzel();
                hitbox.enabled = false;
                animator.SetBool("IsDead", true);
                source.PlayOneShot(deathSound, AudioListener.volume);
                other.gameObject.SetActive(false);
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

        public void Shoot()
        { // is trigger w pocisku sprawia że przenika przez ściany i obiekty
            if (currShootCooldown == 0)
            {
                animator.SetBool("IsShooting", true);
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (!isFacingLeft)
                    bulletRb.AddForce(transform.right * shootForece, ForceMode2D.Impulse);
                else
                    bulletRb.AddForce(-1 * transform.right * shootForece, ForceMode2D.Impulse);
                source.PlayOneShot(shootSound, AudioListener.volume);
                currShootCooldown = shootCooldown;
                Destroy(bullet, 3.0f);
            }
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            hitbox = GetComponent<BoxCollider2D>();
            source = GetComponent<AudioSource>();
            if (!isFacingLeft)
                Flip();
        }
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            // Physics2D.Raycast(this.transform.position, Vector2.left, 5.0f);
            if (currShootCooldown >= 0)
                currShootCooldown -= Time.deltaTime;
            if (currShootCooldown < 0)
                currShootCooldown = 0;

            if (currShootCooldown < 2.85)
                animator.SetBool("IsShooting", false);

            if (rangeBox.isPlayerInSite())
                Shoot();

            //  Debug.DrawRay(transform.position, 5.0f * Vector3.left, Color.red, 0.1f, false);
        }
    }
}