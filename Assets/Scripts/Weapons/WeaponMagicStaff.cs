using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
	public class WeaponMagicStaff : Weapon
	{
		[ProgressBar("LaunchProgress", 1f)]
		[SerializeField]
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
			Projectile proj = SimplePool.Spawn(ProjectilePrefab,
			                              (Vector2)transform.position+ProjectileStartPointOffset,
			                              _transform.rotation).GetComponent<Projectile>();
			proj.Init(this);
		}
	}
}