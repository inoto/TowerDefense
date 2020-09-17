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
				return new []{weapon.DamageMin, weapon.DamageMax};
			}
		}
		
		public override float AttackSpeed
		{
			get
			{
				if (weapon.AttackInterval <= 0 )
					throw new Exception($"{weapon} AttackSpeed could not be 0!");
				return weapon.AttackInterval;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			attackSpeedOriginal = AttackSpeed;
		}

		public override void LoadSoldier(int index)
		{
			base.LoadSoldier(index);

			if (!weapon.gameObject.activeSelf && Soldiers.Count > 0)
				weapon.gameObject.SetActive(true);
			else
			{
				weapon.AttackInterval -= attackSpeedOriginal * attackMultiplierFromSoldiersCount;
			}
		}

		public override Soldier UnloadSoldier(int index)
		{
			if (Soldiers.Count <= 0)
			{
				weapon.gameObject.SetActive(false);
			}
			else
			{
				weapon.AttackInterval += attackSpeedOriginal * attackMultiplierFromSoldiersCount;
			}

			return base.UnloadSoldier(index);
		}
	}
}
