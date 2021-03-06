﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class LifesResource : MonoBehaviour
	{
		[SerializeField] int StartLifes = 20;
		
		int lifesCount;
		
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
		}

		void Start()
		{
			lifesCount = StartLifes;
		}

		void OnEnable()
		{
			Unit.AnyArrivedDestinationEvent += ChangeValue;
		}

		void OnDisable()
		{
			Unit.AnyArrivedDestinationEvent -= ChangeValue;
		}

		void ChangeValue(Unit unit)
		{
			lifesCount -= 1;
			_textMeshPro.text = lifesCount.ToString();
		}
	}
}