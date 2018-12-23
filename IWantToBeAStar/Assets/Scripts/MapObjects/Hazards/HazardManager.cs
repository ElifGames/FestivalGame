﻿using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

namespace IWantToBeAStar.MapObjects.Hazards
{
    public class HazardManager : MonoBehaviour
    {
        public Vector2 UpPosition;
        public Vector2 LeftPosition;
        public Vector2 RightPosition;
        public GameObject Bird;
        public GameObject Airplane;
        public GameObject Lightning;
        public GameObject Dangerous;

        public void Start()
        {
            GameController controller = FindObjectOfType<GameController>();
            controller.OnBackgroundChange += WhenReceivedBgChangeEvent;

            GameData.UpPosition = UpPosition;
            GameData.LeftPosition = LeftPosition;
            GameData.RightPosition = RightPosition;
        }

        private void WhenReceivedBgChangeEvent(BackgroundStatus status)
        {
            var list = GameObject.FindGameObjectsWithTag("Notification");
            var list2 = GameObject.FindGameObjectsWithTag("Hazard");
            if (list.Length != 0)
            {
                foreach (var item in list)
                {
                    DestroyImmediate(item);
                }
            }
            if (list2.Length != 0)
            {
                foreach (var item in list2)
                {
                    if (item.name == "Lightning")
                    {
                        DestroyImmediate(item);
                    }
                }
            }
            StopAllCoroutines();
            switch (status)
            {
                case BackgroundStatus.Ground:
                    break;
                case BackgroundStatus.LowSky:
                    StartCoroutine("StartSpawningLeftRightMove", new object[2] { Bird, 0f });
                    break;
                case BackgroundStatus.HighSky:
                    StartCoroutine("StartSpawningLeftRightMove", new object[2] { Airplane, 0.3f });
                    StartCoroutine("StartSpawningLightning");
                    break;
                case BackgroundStatus.Space:
                    break;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// 매개변수는 반드시 2개의 원소로 이루어진 object배열이 와야함.
        /// </summary>
        /// <param name="parameters">
        /// parameters[0] -> <see cref="GameObject"/>, 
        /// parameters[1] -> float
        /// </param>
        /// <returns></returns>
        private IEnumerator StartSpawningLeftRightMove(object[] parameters)
        {
            GameObject hazard = (GameObject)parameters[0];
            float spawnWaitGain = (float)parameters[1];

            if (!GameData.IsStarted)
            {
                yield return new WaitUntil(() => GameData.IsStarted);
            }

            while (true)
            {
                int i = Random.Range(0, 100);
                if (i <= 50)
                {
                    SpawnHazard(hazard, Direction.Left);
                }
                else
                {
                    SpawnHazard(hazard, Direction.Right);
                }

                yield return new WaitForSeconds(GameData.SpawnWait + spawnWaitGain);
            }
        }

        private IEnumerator StartSpawningLightning()
        {
            while (true)
            {
                StartCoroutine("SpawnLightning");
                yield return new WaitForSeconds(GameData.SpawnWait + 0.1f);
            }
        }

        /// <summary>
        /// 번개 하나를 경고 후 생성
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnLightning()
        {
            float randomX = Random.Range(-UpPosition.x, UpPosition.x);
            var dangerousPosition = new Vector2(randomX, 4.9f);
            var LightningPosition = new Vector2(randomX, 0);
            Quaternion spawnRotation = Quaternion.identity;

            var dangerousInstance = Instantiate(Dangerous, dangerousPosition, spawnRotation);

            bool enable = true;
            for (int i = 0; i < 6; i++)
            {
                enable = enable ? false : true;
                dangerousInstance.GetComponent<Renderer>().enabled = enable;
                yield return new WaitForSeconds(0.3f);
            }
            Destroy(dangerousInstance);

            var lightningInstance = Instantiate(Lightning, LightningPosition, spawnRotation);
            yield return new WaitForSeconds(0.15f);
            Destroy(lightningInstance);
        }

        private void SpawnHazard(GameObject hazard, Direction direction)
        {
            Vector2 spawnPosition = new Vector2();

            switch (direction)
            {
                case Direction.Up:
                    spawnPosition = new Vector2
                        (Random.Range(-UpPosition.x, UpPosition.x), UpPosition.y);
                    break;

                case Direction.Left:
                    spawnPosition = new Vector2
                        (LeftPosition.x, Random.Range(-LeftPosition.y, LeftPosition.y));
                    break;

                case Direction.Right:
                    spawnPosition = new Vector2
                        (RightPosition.x, Random.Range(-RightPosition.y, RightPosition.y));
                    break;
            }

            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard, spawnPosition, spawnRotation);
        }
    }
}
