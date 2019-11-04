using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace TowerDefense
{
	public class TowerCannon : Tower
	{
		[Header("TowerCannon")]
		[SerializeField] float attackMultiplierFromSoldiersCount = 0.25f;
		float attackSpeedOriginal;
		
		public override int[] Damage
		{
			get
			{
				return new []{Weapon.DamageMin, Weapon.DamageMax};
			}
		}
		
		public override float AttackSpeed
		{
			get
			{
				if (Weapon.AttackSpeed <= 0 ) throw new Exception($"{Weapon} AttackSpeed could not be 0!");
				return Weapon.AttackSpeed;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			attackSpeedOriginal = AttackSpeed;
		}

		public override void ActivateSoldier()
		{
			if (!Weapon.gameObject.activeSelf && Soldiers.Count > 0)
				Weapon.gameObject.SetActive(true);
			else
			{
				Weapon.AttackSpeed -= attackSpeedOriginal * attackMultiplierFromSoldiersCount;
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
					Weapon.AttackSpeed += attackSpeedOriginal * attackMultiplierFromSoldiersCount;
				}
			}

			return soldier;
		}
	}
}
