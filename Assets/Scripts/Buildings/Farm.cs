using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class Farm : Building
	{
		public bool IsActive;
		[BoxGroup("Income")]
		[SerializeField] float Interval = 2f;
		[BoxGroup("Income")]
		[SerializeField] int Amount;

		public override void LoadSoldier(int index)
		{
			StartCoroutine(ProvideFood());

			base.LoadSoldier(index);
		}

		public override Soldier UnloadSoldier(int index)
		{
			StopProvideFood();

			return base.UnloadSoldier(index);
		}

		IEnumerator ProvideFood()
		{
			IsActive = true;
			while (IsActive)
			{
				yield return new WaitForSeconds(Interval);

                PlayerController.Instance.AddFood(Amount, transform);
			}
			
			yield return null;
		}

		public void StopProvideFood()
		{
			IsActive = false;
			StopAllCoroutines();
		}
	}
}