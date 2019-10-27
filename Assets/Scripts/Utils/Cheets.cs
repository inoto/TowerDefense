using UnityEngine;

namespace TowerDefense
{
	public class Cheets : MonoBehaviour
	{
		void OnEnable()
		{
			InputKeyboard.Alpha1KeyPressedEvent += UseAlpha1Key;
		}

		void OnDisable()
		{
			InputKeyboard.Alpha1KeyPressedEvent -= UseAlpha1Key;
		}

		void UseAlpha1Key()
		{
			RangeIndicator.Show(200f, "KILL", (enemy) =>
			{
				enemy.Damage(2000);
			});
		}
	}
}