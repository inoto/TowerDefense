using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class TowerMage : Tower
	{
		[Header("TowerMage")]
		[SerializeField] float damageMultiplierFromSoldiersCount = 2f;
//		[SerializeField] GameObject WeaponPrefab;
//		[SerializeField] Weapon weapon;
		
		public override int[] Damage
		{
			get
			{
				return new []{Weapon.DamageMin, Weapon.DamageMax};
			}
		}
		
		public override float AttackSpeed => Weapon.AttackSpeed;

		public override void ActivateSoldier()
		{
			//			GameObject go = Instantiate(WeaponPrefab, transform, true);
//			go.transform.position = spriteTransform.position;
			if (!Weapon.gameObject.activeSelf && Soldiers.Count > 0)
				Weapon.gameObject.SetActive(true);
			else
			{
				Weapon.DamageMin = (int)(Weapon.DamageMin * damageMultiplierFromSoldiersCount);
				Weapon.DamageMax = (int)(Weapon.DamageMax * damageMultiplierFromSoldiersCount);
			}
//			Weapons.Add(go.GetComponent<Weapon>());

base.ActivateSoldier();
		}

		public override Soldier RemoveSoldier()
		{
			Soldier soldier = base.RemoveSoldier();

			if (soldier.InBuilding)
			{
//				Weapon last = Weapons[Weapons.Count - 1];
//				Weapons.Remove(last);
//				Destroy(last.gameObject);
				if (Soldiers.Count <= 0)
					Weapon.gameObject.SetActive(false);
				else
				{
					Weapon.DamageMin = (int)(Weapon.DamageMin / damageMultiplierFromSoldiersCount);
					Weapon.DamageMax = (int)(Weapon.DamageMax / damageMultiplierFromSoldiersCount);
				}
			}

			return soldier;
		}
	}
}
