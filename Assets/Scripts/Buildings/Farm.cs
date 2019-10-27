using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class Farm : MonoBehaviour
	{
		public static event Action<Farm, int> FoodProvidedEvent;

		public bool IsActive;
		[BoxGroup("Income")]
		[SerializeField] float Interval = 2f;
		[BoxGroup("Income")]
		[SerializeField] int Amount;

		void Start()
		{
			Init();
		}

		void Init()
		{
			StartCoroutine(ProvideFood());
		}

		IEnumerator ProvideFood()
		{
			IsActive = true;
			while (IsActive)
			{
				yield return new WaitForSeconds(Interval);

				if (FoodProvidedEvent != null)
					FoodProvidedEvent(this, Amount);
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