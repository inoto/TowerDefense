using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class KillsIndicator : MonoBehaviour
	{
		int killsCount;
		
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
		}
        
		void Start()
		{
			killsCount = 0;
		}

		void OnEnable()
		{
			Unit.AnyDiedEvent += ChangeValue;
		}

		void OnDisable()
		{
			Unit.AnyDiedEvent -= ChangeValue;
		}

		void ChangeValue(Unit unit)
		{
			killsCount += 1;
			_textMeshPro.text = killsCount.ToString();
		}
	}
}