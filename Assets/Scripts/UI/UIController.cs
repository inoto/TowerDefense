﻿using System;
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
		[SerializeField] Color MissColor;
		[SerializeField] Color FoodColor;
		[SerializeField] Sprite FoodIconSprite;
        [SerializeField] Image GameOverOverlay;
        [SerializeField] GameObject GameOverElements;
		
		bool locatorInUse = false;
		
		void OnEnable()
		{
			Projectile.MissedEvent += SpawnMissFloatingText;
			Unit.AnyDiedEvent += SpawnFoodFloatingText;
			PlayerController.FoodAmountChangedEvent += SpawnFoodFloatingText;
//			Enemy.AnyDamagedEvent += ShowDamageFloatingText;
			RangeIndicator.ShowEvent += ShowLocator;
			RangeIndicator.HideEvent += HideLocator;

			GameController.GameOverEvent += OnGameOverEvent;
        }

        void OnGameOverEvent()
        {
            GameOverElements.SetActive(false);

            GameOverOverlay.color.SetAlpha(0f);
			GameOverOverlay.gameObject.SetActive(true);
            
            // LeanTween.alpha(GameOverOverlay.rectTransform, 0f, 3f).setIgnoreTimeScale(true);
			// GameOverOverlay.CrossFadeAlpha(1f, 3f, true);

            LeanTween.delayedCall(1f, OnOverlayAppeared).setIgnoreTimeScale(true);

        }

        void OnOverlayAppeared()
        {
            GameOverElements.SetActive(true);
        }

  //       void OnDisable()
		// {
		// 	Projectile.MissedEvent -= SpawnMissFloatingText;
		// 	Unit.AnyDiedEvent -= SpawnFoodFloatingText;
  //           PlayerController.FoodAmountChangedEvent -= SpawnFoodFloatingText;
		// 	//			Enemy.AnyDamagedEvent -= ShowDamageFloatingText;
		// 	RangeIndicator.ShowEvent -= ShowLocator;
		// 	RangeIndicator.HideEvent -= HideLocator;
		// }

		void Start()
		{
			SimplePool.Preload(FloatingTextPrefab, transform, 10);
		}

		void SpawnMissFloatingText(Projectile projectile)
		{
			FloatingText floatingText = SimplePool.Spawn(FloatingTextPrefab).GetComponent<FloatingText>();
			floatingText.transform.position = projectile.transform.position;
			floatingText.Color(MissColor);
            floatingText.Text("MISS");

			floatingText.StartMoving();
		}

		void SpawnFoodFloatingText(Unit unit)
		{
			Mob mob = unit as Mob;
			if (mob == null)
				return;
			if (mob.FoodReward < 1)
				return;
			
			FloatingText floatingText = SimplePool.Spawn(FloatingTextPrefab).GetComponent<FloatingText>();
			floatingText.transform.position = unit.transform.position;
			floatingText.Color(FoodColor);
			floatingText.Text("+" + mob.FoodReward);

			floatingText.StartMoving();
		}
		
		void SpawnFoodFloatingText(int amount, int food, Transform trans)
        {
            string mark = "";
            if (amount > 0)
                mark = "+";

            FloatingText floatingText = SimplePool.Spawn(FloatingTextPrefab).GetComponent<FloatingText>();
			floatingText.transform.position = trans.position;
			floatingText.Color(FoodColor);
			floatingText.Text(mark + amount);
			floatingText.Icon(FoodIconSprite, FoodColor);
			
			floatingText.StartMoving();
		}

		void ShowDamageFloatingText(Unit unit, Weapon weapon)
		{
			FloatingText floatingText = SimplePool.Spawn(FloatingTextPrefab).GetComponent<FloatingText>();
			floatingText.transform.position = unit.transform.position;
			floatingText.Color(Color.red);
			floatingText.Text("-" + weapon.Damage);
			
			floatingText.StartMovingAttached(-30, unit);
		}

		void ShowLocator(float range, String text, Action<Unit> action)
		{
			if (locatorInUse)
				return;

			locatorInUse = true;
			GameObject go = Instantiate(RangeIndicatorPrefab);
			go.transform.parent = transform;
			go.transform.position = Input.mousePosition;
			go.GetComponent<RangeIndicator>().Init(range, text, action);
		}

		void HideLocator()
		{
			locatorInUse = false;
		}
	}
}