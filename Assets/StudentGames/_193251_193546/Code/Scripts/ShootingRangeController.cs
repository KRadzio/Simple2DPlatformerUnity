using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace _193251_193546
{
    public class ShootingRangeController : MonoBehaviour
    {
        // Start is called before the first frame update

        private bool playerInSite = false;



        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                playerInSite = true;
        }

        /* private void OnTriggerStay2D(Collider2D other)
         {
             if (other.CompareTag("Player"))
             {
                 playerInSite = true;
             }
         }*/

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                playerInSite = false;
        }

        public bool isPlayerInSite() { return playerInSite; }


        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}