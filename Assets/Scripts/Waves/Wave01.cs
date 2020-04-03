﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class Wave01 : Wave
	{
		public float StartDelay = 5f;
		public List<WaveData> WaveData = new List<WaveData>();

		protected override IEnumerator StartWave()
		{
			yield return new WaitForSeconds(StartDelay);
			
			yield return StartCoroutine(base.StartWave());

			Spawn01();
			Spawn02();
			// Spawn03();

			float largestDuration = 0f;
			for (int i = 0; i < WaveData.Count; i++)
			{
				float spawnDuration = WaveData[1].MobCount * WaveData[1].Interval;
				if (spawnDuration > largestDuration)
				{
					largestDuration = spawnDuration;
				}
			}

			yield return null;
		}

		void Spawn01()
		{
			StartSpawn(WaveData[0].MobPrefab, WaveData[0].MobCount, WaveData[0].Interval, WaveData[0].PathName);
		}

		void Spawn02()
		{
			StartSpawn(WaveData[1].MobPrefab, WaveData[1].MobCount, WaveData[1].Interval, WaveData[1].PathName);
		}
		
		void Spawn03()
		{
			StartSpawn(WaveData[2].MobPrefab, WaveData[2].MobCount, WaveData[2].Interval, WaveData[2].PathName);
		}
	}
}