using UnityEngine;

namespace TowerDefense
{
	[CreateAssetMenu(fileName = "GameConfig", menuName = "TowerDefense/GameConfig", order = 0)]
	public class GameConfig : ScriptableObject
	{
		[System.Serializable]
		public class SteeringSettings
		{
			public float MaxForce = 2f;
			public float SeparationForce = 1f;
		}

		public SteeringSettings Steering;
	}
}