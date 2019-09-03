﻿using IWantToBeAStar.MainGame.MapObjects.Hazards;
using System.Collections;
using UnityEngine;

namespace IWantToBeAStar.MainGame
{
    public class ScoreManager : MonoBehaviour
    {
        /// <summary>
        /// 점수 증가 시간 폭
        /// </summary>
        public float ScoreTimeGain;

        /// <summary>
        /// 해당 값의 점수대마다 스폰 시간 감소
        /// </summary>
        public int ReduceSpawnGainScore;

        public int HighSkyStartScore;
        public int SpaceStartScore;

        public delegate void StageChanged(Stage changedStage);

        public event StageChanged StageChangedEvent;

        private GameManager gameManager;

        private void HandleGameEndedEvent()
        {
            StopCoroutine("Scoring");
        }

        private void Awake()
        {
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

            GameData.Score = 0;
            GameData.HighSkyStartScore = HighSkyStartScore;
            GameData.SpaceStartScore = SpaceStartScore;
            GameData.ScoreTimeGain = ScoreTimeGain;
            GameData.ReduceSpawnGainScore = ReduceSpawnGainScore;

            gameManager.GameEndEvent += HandleGameEndedEvent;
            gameManager.GameStartEvent += HandleGameStartEvent;
        }

        private void Start()
        {
            UIManager.GameUI.ScoreText.text = "0";
        }

        private void HandleGameStartEvent()
        {
            StartCoroutine("Scoring");
        }

        private IEnumerator Scoring()
        {
            while (true)
            {
                yield return new WaitForSeconds(ScoreTimeGain);
                AddScore(1);
                var score = GameData.Score;

                // 일정 점수대마다 스폰 주기 감소
                // 0.2초보다 낮아지거나 같으면 더이상 감소 안함
                if (GameData.SpawnWait > 0.2f)
                {
                    if (score % ReduceSpawnGainScore == 0)
                    {
                        ReduceSpawnWait();
                    }
                }

                if (score == HighSkyStartScore)
                {
                    StageChangedEvent(Stage.HighSky);
                }
                else if (score == SpaceStartScore)
                {
                    StageChangedEvent(Stage.Space);
                    StartCoroutine(ChangeScoreHeaderColorBlackToWhite());
                }

                // SpaceStartScore보다 높은 점수 일때
                // 200점마다 장애물 변경
                if ((score >= SpaceStartScore + 1) && (score % 200 == 0))
                {
                    switch (GameData.SpawnSpaceHazard)
                    {
                        case SpaceHazards.Meteo:
                            GameData.SpawnSpaceHazard = SpaceHazards.UFO;
                            break;

                        case SpaceHazards.UFO:
                            GameData.SpawnSpaceHazard = SpaceHazards.Meteo;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void AddScore(int score)
        {
            GameData.Score += score;
            UIManager.GameUI.ScoreText.text = GameData.Score.ToString();
        }
        private IEnumerator ChangeScoreHeaderColorBlackToWhite()
        {
            while (UIManager.GameUI.ScoreText.color.r <= 255)
            {
                float beforeColor = UIManager.GameUI.ScoreText.color.r + 0.01f;
                UIManager.GameUI.ScoreText.color = new Color(beforeColor, beforeColor, beforeColor);
                yield return new WaitForSeconds(0.05f);
            }
        }

        private void ReduceSpawnWait()
        {
            GameData.SpawnWait -= GameData.SpawnGain;
            Debug.Log($"스폰 시간 감소: {GameData.SpawnWait}");
        }
    }
}