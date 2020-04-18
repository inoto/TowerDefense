using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public enum SpawnType
	{
		Simple,
		Group,
		TightGroup
	}

	public enum WaveFormation
	{
		None,
		LeaderFirst
	}
	
	public class WaveController : MonoBehaviour
	{
		public int CurrentWaveNumber = 0;
		public Wave CurrentWave;

        int currentChild = 0;
		
		void OnEnable()
		{
			Wave.EndedEvent += StartNextWave;
		}

		void OnDisable()
		{
			Wave.EndedEvent -= StartNextWave;
		}

		void Start()
		{
			StartNextWave(1);
		}

		void StartNextWave(int waveNumber)
		{
            CurrentWaveNumber += 1;

            for (int i = currentChild; i < transform.childCount; i++)
            {
				Wave wave = transform.GetChild(i).GetComponent<Wave>();
                if (wave.gameObject.activeSelf && wave.Active)
                {
                    currentChild = i+1;
					CurrentWave = wave;
                    CurrentWave.InitWave(CurrentWaveNumber);
                    return;
                }
            }
        }
	}
}