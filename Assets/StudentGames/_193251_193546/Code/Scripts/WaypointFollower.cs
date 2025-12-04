using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193251_193546
{
    public class WaypointFollower : MonoBehaviour
    {

        [SerializeField] private GameObject[] waypoints;
        private int currentWaypoint = 0;
        [SerializeField] private float speed = 1.0f;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float dist = Vector2.Distance(this.transform.position, waypoints[currentWaypoint].transform.position);
            if (dist < 0.1f)
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;

            this.transform.position = Vector2.MoveTowards(this.transform.position, waypoints[currentWaypoint].transform.position, speed * Time.deltaTime);

        }
    }
}