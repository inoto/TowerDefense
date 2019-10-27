using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class LifesResource : MonoBehaviour
	{
		[SerializeField] int StartLifes = 20;
		int _lifesValue;
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
		}

		void Start()
		{
			_lifesValue = StartLifes;
		}

		void OnEnable()
		{
			Unit.ArrivedDestinationEvent += ChangeValue;
		}

		void OnDisable()
		{
			Unit.ArrivedDestinationEvent -= ChangeValue;
		}

		void ChangeValue(Unit unit)
		{
			_lifesValue -= 1;
			_textMeshPro.text = _lifesValue.ToString();
		}
	}
}