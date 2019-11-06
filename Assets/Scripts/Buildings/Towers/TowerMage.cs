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
		[SerializeField] float DamageMultiplierFromSoldiersCount = 0.25f;

		int[] _damageOriginal;
		
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

			_damageOriginal = Damage;
		}

		public override void ActivateSoldier()
		{
			if (!Weapon.gameObject.activeSelf && Soldiers.Count > 0)
				Weapon.gameObject.SetActive(true);
			else
			{
				Weapon.DamageMin += (int)(_damageOriginal[0] * DamageMultiplierFromSoldiersCount);
				Weapon.DamageMax += (int)(_damageOriginal[1] * DamageMultiplierFromSoldiersCount);
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
					Weapon.DamageMin -= (int)(_damageOriginal[0] * DamageMultiplierFromSoldiersCount);
					Weapon.DamageMax -= (int)(_damageOriginal[1] * DamageMultiplierFromSoldiersCount);
				}
			}

			return soldier;
		}
	}
}
