using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class UISpecChoiceClouds : UILevelControl
	{
		[SerializeField] float ShowAnimationTime = 0.1f;
		[SerializeField] LeanTweenType ShowAnimationTween = LeanTweenType.notUsed;
		[SerializeField] UIButton[] Buttons;
		
		bool isShown = false;
		Tower tower;
		
		Transform _transform;

		void Awake()
		{
			_transform = GetComponent<Transform>();

			for (int i = 0; i < Buttons.Length; i++)
			{
				var i1 = i;
				Buttons[i].OnClickedEvent += () => SpecButtonClicked(i1);
			}
		}

		public void Show(Transform buttonTransform, Tower tower)
		{
			// if (isShown)
			// 	return;
			//
			// isShown = true;
			this.tower = tower;

			Vector3 newPos = (Vector2)buttonTransform.position;
			newPos.z -= 1;
			_transform.position = newPos;
			
			_transform.localScale = Vector3.zero;
			LeanTween.scale(gameObject, Vector3.one, ShowAnimationTime).setOnComplete(ActivateSlots);

			// LeanTween.delayedCall(scaleTime, ActivateSlots);

			Show();
		}
		
		void ActivateSlots()
		{
			for (int i = 0; i < Buttons.Length; i++)
				Buttons[i].Interactable = true;
		}

		public override void Hide()
		{
			// isShown = false;
			
			for (int i = 0; i < Buttons.Length; i++)
				Buttons[i].Interactable = false;

			base.Hide();
		}

		public void SpecButtonClicked(int value)
		{
			Debug.Log($"SpecButtonClicked {value}");
			tower.SetSpec((Specialization.Type)(value + 1));
			Hide();
		}
	}
}