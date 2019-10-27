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
				return new []{weapon.DamageMin, weapon.DamageMax};
			}
		}
		
		public override int AttackSpeed
		{
			get
			{
				return weapon.AttackSpeed;
			}
		}

		public override void ActivateSoldier()
		{
			//			GameObject go = Instantiate(WeaponPrefab, transform, true);
//			go.transform.position = spriteTransform.position;
			if (!weapon.gameObject.activeSelf && Soldiers.Count > 0)
				weapon.gameObject.SetActive(true);
			else
			{
				weapon.DamageMin = (int)(weapon.DamageMin * damageMultiplierFromSoldiersCount);
				weapon.DamageMax = (int)(weapon.DamageMax * damageMultiplierFromSoldiersCount);
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
					weapon.gameObject.SetActive(false);
				else
				{
					weapon.DamageMin = (int)(weapon.DamageMin / damageMultiplierFromSoldiersCount);
					weapon.DamageMax = (int)(weapon.DamageMax / damageMultiplierFromSoldiersCount);
				}
			}

			return soldier;
		}
	}
}
