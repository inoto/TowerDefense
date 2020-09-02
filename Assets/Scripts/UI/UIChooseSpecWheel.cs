using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class UIChooseSpecWheel : UILevelControl
	{
		[SerializeField] float ShowAnimationTime = 0.1f;
		[SerializeField] LeanTweenType ShowAnimationTween = LeanTweenType.notUsed;
		[SerializeField] Button[] Buttons;
		
		bool isShown = false;
		Tower tower;
		
		Transform _transform;

		void Awake()
		{
			_transform = GetComponent<Transform>();

			Hide();
		}

		public void Show(Transform buttonTransform, Tower tower)
		{
			if (isShown)
				return;
			
			isShown = true;
			this.tower = tower;

			gameObject.SetActive(true);
			Vector3 newPos = (Vector2)buttonTransform.position;
			newPos.z -= 1;
			_transform.position = newPos;
			
			_transform.localScale = Vector3.zero;
			float scaleTime = 0.1f;
			LeanTween.scale(gameObject, Vector3.one, scaleTime);

			LeanTween.delayedCall(scaleTime, ActivateSlots);
		}
		
		void ActivateSlots()
		{
			for (int i = 0; i < Buttons.Length; i++)
				Buttons[i].interactable = true;
		}

		public void Hide()
		{
			isShown = false;
			
			for (int i = 0; i < Buttons.Length; i++)
				Buttons[i].interactable = false;
			
			gameObject.SetActive(false);
		}

		// using by button
		public void SpecSet(int value)
		{
			tower.SetSpec((Specialization.Type)value);
			Hide();
		}
	}
}