using TMPro;
using UnityEngine;

namespace TowerDefense
{
	public class CampCanvas : MonoBehaviour
	{
		[SerializeField] TextMeshPro CounterText;
		
		public void UpdateCounterText(int soldierCount)
		{
			CounterText.text = $"{soldierCount}";
		}
	}
}