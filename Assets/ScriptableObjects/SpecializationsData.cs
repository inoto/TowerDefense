using UnityEngine;

namespace TowerDefense
{
	[CreateAssetMenu(fileName = "TowerMagicSpecsData", menuName = "TowerDefense/SpecializationsData", order = 0)]
	public class SpecializationsSettings : ScriptableObject
	{
		public Specialization.Data[] Data;
	}
}