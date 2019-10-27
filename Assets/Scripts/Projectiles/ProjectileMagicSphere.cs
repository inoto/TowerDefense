using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class ProjectileMagicSphere : Projectile
	{
		Vector2 desired;
		Vector2 diff;
		float velocityMultiplier;
		
		public override void Init(Weapon weapon)
		{
			base.Init(weapon);

			desired = Vector2.zero;
			diff = Vector2.zero;
			velocityMultiplier = 1f;
		}

		void Update() //TODO: replace with coroutine
		{
			velocityMultiplier += 0.01f;
			diff = target.Point - (Vector2)trans.position;
			trans.position += (Vector3)diff.normalized * velocityMultiplier * Time.fixedDeltaTime * Speed / 100;
			if (diff.magnitude < 0.1f)
				CheckHit();
		}
	}
}