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
		[SerializeField] float damageMultiplierFromSoldiersCount = 0.25f;

		int[] damageOriginal;
		
		public override int[] Damage
		{
			get
			{
				return new []{Weapon.DamageMin, Weapon.DamageMax};
			}
		}
		
		public override float AttackSpeed => Weapon.AttackSpeed;

		protected override void Awake()
		{
			base.Awake();

			damageOriginal = Damage;
		}

		public override void ActivateSoldier()
		{
			if (!Weapon.gameObject.activeSelf && Soldiers.Count > 0)
				Weapon.gameObject.SetActive(true);
			else
			{
				Weapon.DamageMin += (int)(damageOriginal[0] * damageMultiplierFromSoldiersCount);
				Weapon.DamageMax += (int)(damageOriginal[1] * damageMultiplierFromSoldiersCount);
			}

			base.ActivateSoldier();
		}

		public override Soldier RemoveSoldier()
		{
			Soldier soldier = base.RemoveSoldier();

			if (soldier.InBuilding)
			{
				if (Soldiers.Count <= 0)
					Weapon.gameObject.SetActive(false);
				else
				{
					Weapon.DamageMin -= (int)(damageOriginal[0] * damageMultiplierFromSoldiersCount);
					Weapon.DamageMax -= (int)(damageOriginal[1] * damageMultiplierFromSoldiersCount);
				}
			}

			return soldier;
		}
	}
}
