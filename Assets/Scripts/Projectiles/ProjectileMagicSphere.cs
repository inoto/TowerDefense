using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class ProjectileMagicSphere : Projectile
	{
		Vector2 desired, diff;
		float velocityMultiplier;
		Vector3 endPoint;
		
		public override void Init(Weapon weapon)
		{
			base.Init(weapon);

			if (target == null || target.IsDied)
			{
				SimplePool.Despawn(gameObject);
				return;
			}

			endPoint = target.Position;

			desired = Vector2.zero;
			diff = Vector2.zero;
			velocityMultiplier = 1f;
		}

		void Update()
		{
			if (target == null)
			{
				SimplePool.Despawn(gameObject);
				return;
			}
			
			velocityMultiplier += 0.01f;
			diff = target.Position - (Vector2)_transform.position;
			_transform.position += (Vector3)diff.normalized * velocityMultiplier * Time.fixedDeltaTime * (1/TravelTime);
			if (diff.magnitude < 0.1f)
				CheckHit();
		}
	}
}