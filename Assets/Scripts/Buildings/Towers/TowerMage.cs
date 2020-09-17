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
				return new []{weapon.DamageMin, weapon.DamageMax};
			}
		}
		
		public override float AttackSpeed => weapon.AttackInterval;

		protected override void Awake()
		{
			base.Awake();

			damageOriginal = Damage;
		}

		public override void LoadSoldier(int index)
		{
			if (!weapon.gameObject.activeSelf && Soldiers.Count > 0)
				weapon.gameObject.SetActive(true);
			else
			{
				weapon.DamageMin += (int)(damageOriginal[0] * damageMultiplierFromSoldiersCount);
				weapon.DamageMax += (int)(damageOriginal[1] * damageMultiplierFromSoldiersCount);
			}

			base.LoadSoldier(index);
		}

		public override Soldier UnloadSoldier(int index)
		{
			if (Soldiers.Count <= 0)
			{
				weapon.gameObject.SetActive(false);
			}
			else
			{
				if (Soldiers.Count <= 0)
				{
					weapon.gameObject.SetActive(false);
				}
				else
				{
					weapon.DamageMin -= (int)(damageOriginal[0] * damageMultiplierFromSoldiersCount);
					weapon.DamageMax -= (int)(damageOriginal[1] * damageMultiplierFromSoldiersCount);
				}
			}

			return base.UnloadSoldier(index);
		}
	}
}
