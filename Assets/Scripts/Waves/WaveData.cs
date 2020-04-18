using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    [Serializable]
    public class WaveData
    {
        public List<MobData> Mobs;
        public List<WizardData> Wizards;

        [Serializable]
        public class MobData
        {
            public GameObject MobPrefab;
            public int MobCount;
            public float Interval;
            public string PathName;

            public MobData()
            {
                MobPrefab = null;
                MobCount = 5;
                Interval = 2;
                PathName = "Path0";
            }
        }

        [Serializable]
        public class WizardData
        {
            public BigSkyStone Stone;
        }
    }
}