using System;
using UnityEngine;

namespace TowerDefense
{
	public class UILevelControl : MonoBehaviour
	{
		public event Action<UILevelControl> ShownEvent;
		public event Action<UILevelControl> HiddenEvent;

		public GameObject Owner { get; protected set; }

		public virtual void Show()
		{
			gameObject.SetActive(true);
			ShownEvent?.Invoke(this);
		}

		public virtual void Hide()
		{
			gameObject.SetActive(false);
			HiddenEvent?.Invoke(this);
		}
	}
}