using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class Wave01 : Wave
    {
        protected override void StartWave()
        {
            base.StartWave();

			Spawn01();
			Spawn02();
			// Spawn03();
		}

		void Spawn01()
		{
			StartSpawn(WaveData.Mobs[0].MobPrefab, WaveData.Mobs[0].MobCount, WaveData.Mobs[0].Interval, WaveData.Mobs[0].PathName);
		}

		void Spawn02()
		{
			StartSpawn(WaveData.Mobs[1].MobPrefab, WaveData.Mobs[1].MobCount, WaveData.Mobs[1].Interval, WaveData.Mobs[1].PathName);
		}
		
		void Spawn03()
		{
			StartSpawn(WaveData.Mobs[2].MobPrefab, WaveData.Mobs[2].MobCount, WaveData.Mobs[2].Interval, WaveData.Mobs[2].PathName);
		}
	}
}