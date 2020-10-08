using System;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class UITrapChoiceClouds : UILevelControl
	{
		public event Action<UITrapChoiceClouds, GameObject> TrapChosenEvent;

		[System.Serializable]
		public class TrapsDict : SerializableDictionaryBase<UIButton, string> {};

		[SerializeField] float showAnimationDuration = 0.1f;
		[SerializeField] LeanTweenType showAnimationTween = LeanTweenType.notUsed;
		[SerializeField] List<UIButton> buttons = new List<UIButton>();
		[SerializeField] List<GameObject> trapPrefabs = new List<GameObject>();
		
		bool isShown = false;
		TrapPlace trapPlace;
		Vector2 point;
		
		Transform _transform;

		void Awake()
		{
			_transform = GetComponent<Transform>();
		}

		void Start()
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				var i1 = i;
				buttons[i].OnClickedEvent += () => TrapButtonClicked(i1);
			}
		}

		public void Show(Vector2 point, TrapPlace trapPlace)
		{
			// if (isShown)
			// 	return;
			//
			// isShown = true;
			this.point = point;
			this.trapPlace = trapPlace;

			Vector3 position = Camera.main.WorldToScreenPoint(trapPlace.transform.position);
			_transform.position = UICamera.ScreenToWorldPoint(position);
			
			_transform.localScale = Vector3.zero;
			LeanTween.scale(gameObject, Vector3.one, showAnimationDuration).setOnComplete(ActivateSlots);

			// LeanTween.delayedCall(scaleTime, ActivateSlots);

			Show();
		}
		
		void ActivateSlots()
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				buttons[i].Interactable = true;
			}
		}

		public override void Hide()
		{
			// isShown = false;
			
			for (int i = 0; i < buttons.Count; i++)
			{
				buttons[i].Interactable = false;
			}

			base.Hide();
		}

		public void TrapButtonClicked(int index)
		{
			Debug.Log($"TrapButtonClicked {index}");

			// var trap = Instantiate(trapPrefabs[index], trapPlace.Tower.transform).GetComponent<Trap>();
			// trap.transform.position = trapPlace.transform.position;
			// trap.TrapPlace = trapPlace;
			// trapPlace.Tower.AddTrap(trap);
			// trapPlace.Take();

			TrapChosenEvent?.Invoke(this, trapPrefabs[index]);

			Hide();
		}
	}
}