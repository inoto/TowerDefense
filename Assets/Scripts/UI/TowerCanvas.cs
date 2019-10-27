using TMPro;
using UnityEngine;

namespace TowerDefense
{
	public class TowerCanvas : MonoBehaviour
	{
		[SerializeField] TextMeshPro CounterText;
		[SerializeField] GameObject NoSpecIcon;
		
		public void UpdateCounterText(int soldierCount, int desiredCount)
		{
			CounterText.text = $"{soldierCount}/{desiredCount}";
		}

		public void SetNoSpecIcon(bool value)
		{
			NoSpecIcon.SetActive(value);
		}
	}
}