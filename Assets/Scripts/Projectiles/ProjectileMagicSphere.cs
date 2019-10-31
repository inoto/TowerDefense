using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class ProjectileMagicSphere : Projectile
	{
		Vector2 _desired;
		Vector2 _diff;
		float _velocityMultiplier;
		
		public override void Init(Weapon weapon)
		{
			base.Init(weapon);

			_desired = Vector2.zero;
			_diff = Vector2.zero;
			_velocityMultiplier = 1f;
		}

		void Update() //TODO: replace with coroutine
		{
			if (target == null || target.IsDied)
			{
				Destroy(gameObject);
				return;
			}
			
			_velocityMultiplier += 0.01f;
			_diff = target.Point - (Vector2)_transform.position;
			_transform.position += (Vector3)_diff.normalized * _velocityMultiplier * Time.fixedDeltaTime * (1/TravelTime);
			if (_diff.magnitude < 0.1f)
				CheckHit();
		}
	}
}