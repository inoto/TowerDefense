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
				if (ZoomKeyPressedEvent != null)
					ZoomKeyPressedEvent();
			}
			if (Input.GetKeyUp(KeyCode.Alpha1))
			{
				if (Alpha1KeyPressedEvent != null)
					Alpha1KeyPressedEvent();
			}
		}
	}
}