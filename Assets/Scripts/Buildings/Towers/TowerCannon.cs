using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class TowerCannon : Tower
	{
		[Header("TowerCannon")]
		[SerializeField] float attackMultiplierFromSoldiersCount = 1.75f;
//		[SerializeField] GameObject WeaponPrefab;
//		[SerializeField] Weapon weapon;
		
		public override int[] Damage
		{
			get
			{
				return new []{weapon.DamageMin, weapon.DamageMax};
			}
		}
		
		public override float AttackSpeed => weapon.AttackSpeed;

		public override void ActivateSoldier()
		{
//			GameObject go = Instantiate(WeaponPrefab, transform, true);
//			go.transform.position = spriteTransform.position;
			if (!weapon.gameObject.activeSelf && Soldiers.Count > 0)
				weapon.gameObject.SetActive(true);
			else
			{
				weapon.AttackSpeed = (int)(weapon.AttackSpeed * attackMultiplierFromSoldiersCount);
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
					weapon.AttackSpeed = (int)(weapon.AttackSpeed / attackMultiplierFromSoldiersCount);
				}
			}

			return soldier;
		}
	}
}
