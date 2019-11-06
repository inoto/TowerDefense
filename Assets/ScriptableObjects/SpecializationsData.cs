using UnityEngine;

namespace TowerDefense
{
	[UnityEngine.CreateAssetMenu(fileName = "TowerMagicSpecsData", menuName = "TowerDefense/SpecializationsData", order = 0)]
	public class SpecializationsData : ScriptableObject
	{
		public Specialization.Data[] Data;
	}
}