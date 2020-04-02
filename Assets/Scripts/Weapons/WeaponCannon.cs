using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
	public class WeaponCannon : Weapon
	{
		[Header("WeaponCannon")]
		public float FullDamageRange = 0.5f;
		public float SplashDamageRange = 1f;
		public float SplashDamageFactor = 0.5f;

		void Update()
		{
			LaunchProgress += 1/AttackInterval * Time.deltaTime;
			while (LaunchProgress >= 1f)
			{
				if (AcquireTarget() && IsActive)
				{
					ReleaseProjectile();
					LaunchProgress -= 1f;
				}
				else
				{
					LaunchProgress = 0.999f;
				}
			}
		}
	}
}