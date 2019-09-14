﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IWantToBeAStar.MainGame.GameStage
{
    public class HighSkyStage : Stage
    {
        SpawnTimer timer;

        public HighSkyStage() : base(StageType.HighSky)
        {
        }

        protected override IEnumerator StageMain()
        {
            timer = new SpawnTimer(1f, 0.2f, 10, 5);
            IEnumerator spawnAirplane = SpawningAirplane();
            StartCoroutine(spawnAirplane);
            yield return StartCoroutine(timer.StartReduceSpawnTimer());
            StopCoroutine(spawnAirplane);

            yield return StartCoroutine(Warning());

            timer = new SpawnTimer(0.8f, 0.15f, 5, 6);
            IEnumerator spawnLightning = SpawningLightning();
            StartCoroutine(spawnLightning);
            yield return StartCoroutine(timer.StartReduceSpawnTimer());
            StopCoroutine(spawnLightning);
        }

        private IEnumerator Warning()
        {
            UIManager.GameUI.ReadyText.color = Color.red;
            for (int i = 0; i < 3; i++)
            {
                UIManager.GameUI.ReadyText.text = "WARNING!";
                yield return new WaitForSeconds(1f);
                UIManager.GameUI.ReadyText.text = string.Empty;
            }
            UIManager.GameUI.SetDefaultToReadyText();
        }

        private IEnumerator SpawningAirplane()
        {
            while (true)
            {
                hazardManager.SpawnAirplane();
                yield return new WaitForSeconds(timer.SpawnWait);
            }
        }

        private IEnumerator SpawningLightning()
        {
            while (true)
            {
                int count = UnityEngine.Random.Range(1, 3);
                hazardManager.SpawnLightning(count);
                yield return new WaitForSeconds(timer.SpawnWait);
            }
        }
    }
}
