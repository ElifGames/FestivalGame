﻿using System;
using System.Collections;
using UnityEngine;

namespace IWantToBeAStar
{
    public class GameController : MonoBehaviour
    {
        public GameObject Hazard;
        public Vector2 SpawnValues;

        /// <summary>
        /// 웨이브 당 장애물 증가 폭
        /// </summary>
        public int HazardIncrease;

        /// <summary>
        /// 맨처음 장애물 수
        /// </summary>
        public int HazardCount;

        /// <summary>
        /// 스폰 시간 간격
        /// </summary>
        public float SpawnWait;

        /// <summary>
        /// 맨처음 시작 대기 시간
        /// </summary>
        public float StartWait;

        /// <summary>
        /// 웨이브 대기 시간
        /// </summary>
        public float WaveWait;

        /// <summary>
        /// 웨이브가 시작됨을 알리는 이벤트
        /// </summary>
        public event EventHandler WaveStarted;

        // Use this for initialization
        private void Start()
        {
            GameData.Wave = 1;
            StartCoroutine("SpawnWaves");
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private IEnumerator SpawnWaves()
        {
            // 게임 시작

            // 맨 처음 시작 대기
            yield return new WaitForSeconds(StartWait);

            while (true)
            {
                // 한 웨이브 진행
                Debug.Log(GameData.Wave + "번째 웨이브 시작");

                WaveStarted(this, new WaveStartedEventArgs(GameData.Wave));

                for (int i = 0; i < HazardCount; i++)
                {
                    Vector2 spawnPosition = new Vector2
                        (UnityEngine.Random.Range(-SpawnValues.x, SpawnValues.x), SpawnValues.y);
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate(Hazard, spawnPosition, spawnRotation);
                    yield return new WaitForSeconds(SpawnWait);
                }
                // 한 웨이브 끝
                // 다음 웨이브 준비
                GameData.Wave++;
                HazardCount += HazardIncrease;
                if (SpawnWait > 0.05f)
                {
                    SpawnWait -= 0.01f;
                }
                yield return new WaitForSeconds(WaveWait);
            }
        }
    }
}