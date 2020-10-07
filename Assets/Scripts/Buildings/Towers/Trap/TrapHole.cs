using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class TrapHole : MonoBehaviour
	{
		[SerializeField] int maxHp = 30;
		[SerializeField] int charges = 3;
		[SerializeField] List<Mob> mobs = new List<Mob>();

		CircleCollider2D collider;

		void OnTriggerEnter2D(Collider2D other)
		{
			if (charges <= 0)
				return;

			var mob = other.GetComponent<Mob>();
			if (mob == null)
				return;

			if (mob.MaxHealth > maxHp)
				return;

			mob.Damage(1000, DamageType.Physical);
			charges -= 1;
			if (charges <= 0)
			{
				Corpse();
			}
		}

		protected void Corpse()
		{
			LeanTween.alpha(gameObject, 0f, 2f).setOnComplete(() => SimplePool.Despawn(gameObject));
		}
	}
}