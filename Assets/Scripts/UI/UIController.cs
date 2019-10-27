using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace TowerDefense
{
	public class UIController : MonoBehaviour
	{
		[SerializeField] GameObject FloatingTextPrefab;
		[SerializeField] GameObject RangeIndicatorPrefab;
		bool _locatorInUse = false;
		[SerializeField] Color MissColor;
		[SerializeField] Color FoodColor;
		
		void OnEnable()
		{
			Projectile.MissedEvent += SpawnMissFloatingText;
			Unit.DiedEvent += SpawnFoodFloatingText;
			Farm.FoodProvidedEvent += SpawnFarmFoodFloatingText;
//			Enemy.DamagedEvent += ShowDamageFloatingText;
			RangeIndicator.ShowEvent += ShowLocator;
			RangeIndicator.HideEvent += HideLocator;
		}

		void OnDisable()
		{
			Projectile.MissedEvent -= SpawnMissFloatingText;
			Unit.DiedEvent -= SpawnFoodFloatingText;
			Farm.FoodProvidedEvent -= SpawnFarmFoodFloatingText;
//			Enemy.DamagedEvent -= ShowDamageFloatingText;
			RangeIndicator.ShowEvent -= ShowLocator;
			RangeIndicator.HideEvent -= HideLocator;
		}

		void SpawnMissFloatingText(Projectile projectile)
		{
			FloatingText floatingText = Instantiate(FloatingTextPrefab, transform).GetComponent<FloatingText>();
			floatingText.transform.position = projectile.transform.position;
			floatingText.SetColor(MissColor);
			
			floatingText.StartMoving();
		}

		void SpawnFoodFloatingText(Unit unit)
		{
			if (unit.FoodReward < 1)
				return;
			
			FloatingText floatingText = Instantiate(FloatingTextPrefab, transform).GetComponent<FloatingText>();
			floatingText.transform.position = unit.Point;
			floatingText.SetColor(FoodColor);
			floatingText.SetText("+" + unit.FoodReward);
			
			floatingText.StartMoving();
		}
		
		void SpawnFarmFoodFloatingText(Farm farm, int amount)
		{
			if (amount < 1)
				return;
			
			FloatingText floatingText = Instantiate(FloatingTextPrefab, transform).GetComponent<FloatingText>();
			floatingText.transform.position = farm.transform.position;
			floatingText.SetColor(FoodColor);
			floatingText.SetText("+" + amount);
			
			floatingText.StartMoving();
		}

		void ShowDamageFloatingText(Unit unit, Weapon weapon)
		{
			FloatingText floatingText = Instantiate(FloatingTextPrefab, transform).GetComponent<FloatingText>();
			floatingText.transform.position = unit.Point;
			floatingText.SetColor(Color.red);
			floatingText.SetText("-" + weapon.Damage);
			
			floatingText.StartMovingAttached(-30, unit);
		}

		void ShowLocator(float range, String text, Action<Unit> action)
		{
			if (_locatorInUse)
				return;

			_locatorInUse = true;
			GameObject go = Instantiate(RangeIndicatorPrefab);
			go.transform.parent = transform;
			go.transform.position = Input.mousePosition;
			go.GetComponent<RangeIndicator>().Init(range, text, action);
		}

		void HideLocator()
		{
			_locatorInUse = false;
		}
	}
}