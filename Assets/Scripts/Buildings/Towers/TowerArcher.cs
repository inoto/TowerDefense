using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TowerDefense
{
	public class TowerArcher : Tower
	{
		[Header("TowerArcher")]
		[SerializeField] GameObject weaponPrefab;
		public List<Weapon> Weapons;
		
		public override int[] Damage
		{
			get
			{
				int damageMin = 0, damageMax = 0;
				for (int i = 0; i < Weapons.Count; i++)
				{
					damageMin += Weapons[i].DamageMin;
					damageMax += Weapons[i].DamageMax;
				}
				return new []{damageMin, damageMax};
			}
		}
		
		public override float AttackSpeed => Weapons.Count > 0 ? Weapons[0].AttackSpeed : 0;

		public override void ActivateSoldier()
		{
			GameObject go = Instantiate(weaponPrefab, transform, true);
			go.transform.position = spriteTransform.position;
			go.SetActive(true);
			Weapons.Add(go.GetComponent<Weapon>());
			
			base.ActivateSoldier();
		}

		public override Soldier RemoveSoldier()
		{
			Soldier soldier = base.RemoveSoldier();

			if (soldier.InBuilding)
			{
				Weapon last = Weapons[Weapons.Count - 1];
				Weapons.Remove(last);
				Destroy(last.gameObject);
			}

			return soldier;
		}
	}
}
