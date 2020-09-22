using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	[CreateAssetMenu(fileName = "WaveData", menuName = "TowerDefense/WaveData", order = 0)]
    public class WaveData : ScriptableObject
    {
	    public bool Active;
	    public float BeforeWaveDelay;
	    public float AfterWaveDelay;
        public List<MobData> Mobs;
        public List<WizardData> Wizards;

        [Serializable]
        public class MobData
        {
	        public float Delay;
            public GameObject MobPrefab;
            public int MobCount;
            public float Interval;
            public string PathName;

            public MobData()
            {
	            Delay = 0f;
                MobPrefab = null;
                MobCount = 5;
                Interval = 2;
                PathName = "Path0";
            }
        }

        [Serializable]
        public class WizardData
        {
	        public float Delay;
            public GameObject WizardPrefab;
            public int CaveIndex;
        }
    }
}