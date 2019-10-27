using UnityEngine;

namespace TowerDefense
{
	public class Mob : Unit
	{
		[Header("Mob")]
		[SerializeField] HealthBar HealthBar;

		public override void Init(string pathName, bool isNew = true)
		{
			base.Init(pathName, isNew);
			
			UpdateHealthBar();
		}

		public override void Damage(float damage, DamageType type = DamageType.Physical)
		{
			base.Damage(damage, type);
			
			UpdateHealthBar();
		}

		public override void Damage(Weapon weapon)
		{
			base.Damage(weapon);
			
			UpdateHealthBar();
		}
		
		void UpdateHealthBar()
		{
			if (HealthBar != null)
			{
				HealthBar.SetPercent(HealthPercent);
				if (HealthPercent >= 1f || HealthPercent <= 0f)
				{
					HealthBar.gameObject.SetActive(false);
				}
				else
				{
					HealthBar.gameObject.SetActive(true);
				}
			}
		}
	}
}