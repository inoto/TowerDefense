using System;
using UnityEngine;

namespace TowerDefense
{
	public class InputKeyboard : MonoBehaviour
	{
		public static event Action ZoomKeyPressedEvent;
		public static event Action Alpha1KeyPressedEvent;
		
		void Update()
		{
			if (Input.GetKeyUp(KeyCode.Tab))
			{
				ZoomKeyPressedEvent?.Invoke();
			}
			if (Input.GetKeyUp(KeyCode.Alpha1))
			{
				Alpha1KeyPressedEvent?.Invoke();
			}
		}
	}
}