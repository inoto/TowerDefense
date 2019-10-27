using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class WaveResource : MonoBehaviour
	{
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
		}

		void Start()
		{
			_textMeshPro.text = 0.ToString();
		}

		void OnEnable()
		{
			Wave.StartedEvent += ChangeValue;
		}

		void OnDisable()
		{
			Wave.StartedEvent -= ChangeValue;
		}

		void ChangeValue(int waveNumber)
		{
			_textMeshPro.text = waveNumber.ToString();
		}
	}
}