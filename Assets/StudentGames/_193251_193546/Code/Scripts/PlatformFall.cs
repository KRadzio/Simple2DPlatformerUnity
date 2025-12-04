using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace _193251_193546
{
    public class PlatformFall : MonoBehaviour
    {

        private Rigidbody2D rbody;
        private Vector3 startPosition;
        public float shakeTime = 2.0f;
        public float shakeMagnitude = 0.3f;
        // Start is called before the first frame update

        IEnumerator ShakePlatform()
        {
            float elapsedTime = 0.0f;

            while (elapsedTime < shakeTime)
            {
                transform.position = startPosition + new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude), 0f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rbody.bodyType = RigidbodyType2D.Dynamic;
            rbody.AddForce(Vector2.up * 1.5f, ForceMode2D.Impulse);
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && col.transform.position.y > transform.position.y - 0.1f &&
                ((col.transform.position.x - transform.position.x < 1.0f && col.transform.position.x - transform.position.x > 0.0f)
                || (transform.position.x - col.transform.position.x < 1.0f && transform.position.x - col.transform.position.x > 0.0f)))
            {
                StartCoroutine(ShakePlatform());
                Destroy(gameObject, 3.0f);
            }
        }

        void Start()
        {
            rbody = GetComponent<Rigidbody2D>();
            startPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}