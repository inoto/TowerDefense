using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Mob : Unit
	{
		public static event Action<Unit, string> LookingForPathEvent;
		
		[Header("Mob")]
		[SerializeField] HealthBar HealthBar;
		
		void Start()
		{
			StartCoroutine(CheckCreatedManually());
		}
	
		IEnumerator CheckCreatedManually()
		{
			yield return new WaitForSeconds(2f);
			if (!IsActive)
				Init("Path0", false);
		}

		public override void Init(string pathName, bool isNew = true)
		{
			base.Init(pathName, isNew);
			
			UpdateHealthBar();
			
			GetComponent<MoveByPath>().Init(pathName);

			// if (LookingForPathEvent != null)
			// 	LookingForPathEvent(this, pathName);
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