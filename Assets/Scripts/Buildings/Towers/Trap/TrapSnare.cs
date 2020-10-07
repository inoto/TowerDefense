using UnityEngine;

namespace TowerDefense
{
	public class TrapSnare : Trap
	{
		[SerializeField] int damage = 5;
		[SerializeField] int charges = 5;

		void OnTriggerEnter2D(Collider2D other)
		{
			if (charges <= 0)
				return;

			var mob = other.GetComponent<Mob>();
			if (mob == null)
				return;

			mob.Damage(damage, DamageType.Physical);
			charges -= 1;
			if (charges <= 0)
			{
				RaiseFullyUsedEvent();
				Corpse();
			}
		}

		protected void Corpse()
		{
			LeanTween.alpha(gameObject, 0f, 2f).setOnComplete(() => SimplePool.Despawn(gameObject));
		}
	}
}