using System;
using UnityEngine;

namespace TowerDefense
{
	public class UIButton : MonoBehaviour
	{
		public event Action OnClickedEvent;

		public bool Interactable = true;

		public void OnTap(Vector2 point)
		{
			if (!Interactable)
				return;

			OnClickedEvent?.Invoke();
		}
	}
}