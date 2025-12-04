using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193251_193546
{
    public class ChangeGenPlatformRotationSignController : MonoBehaviour
    {
        public GeneratedPlatforms generatedPlatformsController;
        // Start is called before the first frame update

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("RotationChangeCrate"))
                generatedPlatformsController.ChangeRotationDirection();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("RotationChangeCrate"))
                generatedPlatformsController.ChangeRotationDirection();
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}