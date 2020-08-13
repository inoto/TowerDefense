using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	[CreateAssetMenu(fileName = "TowerMagicSpecsData", menuName = "TowerDefense/LevelData", order = 0)]
	public class LevelData : ScriptableObject
	{
		public List<WaveData> Waves;
	}
}