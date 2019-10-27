using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class FPSCounter : MonoBehaviour
	{
		TextMeshProUGUI _textMeshPro;
		float _deltaTime = 0f;

		void Awake()
		{
			_textMeshPro = GetComponent<TextMeshProUGUI>();
		}

		IEnumerator Start()
		{
			_deltaTime = Time.unscaledDeltaTime;
			while (true)
			{
				if (Time.timeScale > 0)
				{
					_deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
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