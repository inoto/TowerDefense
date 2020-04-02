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
			if (CurrentWaveNumber >= transform.childCount)
				return;

			CurrentWaveNumber += 1;

			Wave wave = transform.GetChild(CurrentWaveNumber - 1).GetComponent<Wave>();
			if (wave.gameObject.activeSelf && wave.Active)
			{
				CurrentWave = wave;
				CurrentWave.InitWave(CurrentWaveNumber);
			}
		}
	}
}