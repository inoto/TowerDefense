using System;
using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class WeaponBow : Weapon
	{
		[ProgressBar("LaunchProgress")][SerializeField]
		float LaunchProgress;
		void Update()
		{
			LaunchProgress += 1/AttackSpeed * Time.deltaTime;
			while (LaunchProgress >= 1f)
			{
				if (TrackTarget() || AcquireTarget())
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