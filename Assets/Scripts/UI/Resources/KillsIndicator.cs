using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class KillsIndicator : MonoBehaviour
	{
		int _kills;
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
		}
        
		void Start()
		{
			_kills = 0;
		}

		void OnEnable()
		{
			Unit.DiedEvent += ChangeValue;
		}

		void OnDisable()
		{
			Unit.DiedEvent -= ChangeValue;
		}

		void ChangeValue(Unit unit)
		{
			_kills += 1;
			_textMeshPro.text = _kills.ToString();
		}
	}
}