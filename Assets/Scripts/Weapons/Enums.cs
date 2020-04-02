using UnityEngine;

namespace TowerDefense
{
	public enum DamageType
	{
		Physical,
		Magical
	}
	
	public enum CanAttackTarget // TODO: rework with byte construction
	{
		Ground,
		Air,
		GroundAndAir
	}
}