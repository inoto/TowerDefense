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

		[ProgressBar("LaunchProgress", 1f)]
		[SerializeField]
		float LaunchProgress;
		void Update()
		{
			LaunchProgress += 1/AttackSpeed * Time.deltaTime;
			while (LaunchProgress >= 1f)
			{
				if (AcquireTarget())
				{
					ReleaseMissile();
					LaunchProgress -= 1f;
				}
				else
				{
					LaunchProgress = 0.999f;
				}
			}
		}
		
		protected override void ReleaseMissile()
		{
			// TODO: Object Pool for projectiles
			Projectile proj = Instantiate(ProjectilePrefab,
			                              (Vector2)transform.position+ProjectileStartPointOffset,
			                              _transform.rotation).GetComponent<Projectile>();
			proj.Init(this);
		}
	}
}