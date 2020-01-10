using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class FPSCounter : MonoBehaviour
	{
		float deltaTime = 0f;
		
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponent<TextMeshProUGUI>();
		}

		IEnumerator Start()
		{
			deltaTime = Time.unscaledDeltaTime;
			while (true)
			{
				if (Time.timeScale > 0)
				{
					deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
					_textMeshPro.text = (1.0f / Time.unscaledDeltaTime).ToString("F1");
				}
				else
				{
					_textMeshPro.text = "paused";
				}
				yield return new WaitForSeconds (0.5f);
			}
		}
	}
}