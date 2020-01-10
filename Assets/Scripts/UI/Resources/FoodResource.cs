using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class FoodResource : MonoBehaviour
	{
		int foodCount;
		
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
		}

		void Start()
		{
			foodCount = 0;
			UpdateValue();
		}

		void OnEnable()
		{
			Unit.DiedEvent += ChangeValue;
			Farm.FoodProvidedEvent += FarmProvided;
		}

		void OnDisable()
		{
			Unit.DiedEvent -= ChangeValue;
			Farm.FoodProvidedEvent += FarmProvided;
		}

		void ChangeValue(Unit unit)
		{
			if (unit is Mob mob)
			{
				foodCount += mob.FoodReward;
				UpdateValue();
			}
		}
		
		void FarmProvided(Farm farm, int amount)
		{
			foodCount += amount;
			UpdateValue();
		}

		void UpdateValue()
		{
			_textMeshPro.text = foodCount.ToString();
		}
	}
}