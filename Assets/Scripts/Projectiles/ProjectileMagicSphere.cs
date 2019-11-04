using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class ProjectileMagicSphere : Projectile
	{
		Vector2 _desired, _diff;
		float _velocityMultiplier;
		Vector3 _endPoint;
		
		public override void Init(Weapon weapon)
		{
			base.Init(weapon);

			_endPoint = _target.Point;

			_desired = Vector2.zero;
			_diff = Vector2.zero;
			_velocityMultiplier = 1f;
		}

		void Update() //TODO: replace with coroutine
		{
			if (_target == null)
			{
				SimplePool.Despawn(gameObject);
				return;
			}
			
			_velocityMultiplier += 0.01f;
			_diff = _target.Point - (Vector2)_transform.position;
			_transform.position += (Vector3)_diff.normalized * _velocityMultiplier * Time.fixedDeltaTime * (1/TravelTime);
			if (_diff.magnitude < 0.1f)
				CheckHit();
		}
	}
}