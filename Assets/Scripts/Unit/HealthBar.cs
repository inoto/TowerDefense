using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class HealthBar : MonoBehaviour
	{
		[SerializeField] Image FillImage;

		public void SetPercent(float value)
		{
			FillImage.fillAmount = value;
		}
	}
}