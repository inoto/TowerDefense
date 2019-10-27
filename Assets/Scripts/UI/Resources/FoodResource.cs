using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class FoodResource : MonoBehaviour
	{
		int _food;
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
		}

		void Start()
		{
			_food = 0;
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
			_food += unit.FoodReward;
			UpdateValue();
		}
		
		void FarmProvided(Farm farm, int amount)
		{
			_food += amount;
			UpdateValue();
		}

		void UpdateValue()
		{
			_textMeshPro.text = _food.ToString();
		}
	}
}