using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193251_193546
{
    public class GeneratedPlatforms : MonoBehaviour
    {
        [SerializeField] GameObject platformPrefab;
        private const int PLATFORMS_NUM = 6;
        GameObject[] platforms;
        Vector3[] positions;
        Vector3[] DstPositions;
        private float speed = 4.0f;
        private float radious = 7.0f;
        private int offset = 1;
        void Start()
        {

        }

        void Awake()
        {
            platforms = new GameObject[PLATFORMS_NUM];
            positions = new Vector3[PLATFORMS_NUM];
            DstPositions = new Vector3[PLATFORMS_NUM];

            for (int i = 0; i < PLATFORMS_NUM; i++)
            {
                float angle = 360 / PLATFORMS_NUM * i;
                angle = angle * (3.14f / 180);
                positions[i] = new Vector3(transform.position.x + Mathf.Cos(angle) * radious, transform.position.y + Mathf.Sin(angle) * radious, 0);
                platforms[i] = Instantiate(platformPrefab, positions[i], Quaternion.identity);
            }
            for (int i = 0; i < PLATFORMS_NUM; i++)
            {
                if (i == PLATFORMS_NUM - 1)
                    DstPositions[i] = platforms[0].transform.position;
                else
                    DstPositions[i] = platforms[i + 1].transform.position;
            }
        }

        private void setNewDest()
        {
            for (int i = 0; i < PLATFORMS_NUM; i++)
            {
                int pos = offset + i;
                if (pos > PLATFORMS_NUM - 1)
                    pos -= 6;
                DstPositions[i] = positions[pos];
            }
        }

        public void ChangeRotationDirection()
        {
            speed = -speed;
        }
        // Update is called once per frame
        void Update()
        {

            for (int i = 0; i < PLATFORMS_NUM; i++)
            {
                platforms[i].transform.position = Vector3.MoveTowards(platforms[i].transform.position, DstPositions[i], speed * Time.deltaTime);
            }
            if (platforms[0].transform.position == DstPositions[0])
            {
                offset++;
                if (offset == PLATFORMS_NUM + 1)
                    offset = 1;
                setNewDest();
            }
        }
    }
}