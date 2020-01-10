using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class AvailableSoldiersResource : MonoBehaviour
	{
		int soldiersCount;
		
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
		}
        
		void Start()
		{
			soldiersCount = 0;//Int32.Parse(Value.text);
		}

		void OnEnable()
		{
			Camp.SoldierAssignedEvent += SoldierAssignedToCamp;
			Camp.SoldierUnassignedEvent += SoldierUnassignedToCamp;
		}

		void OnDisable()
		{
			Camp.SoldierAssignedEvent -= SoldierAssignedToCamp;
			Camp.SoldierUnassignedEvent -= SoldierUnassignedToCamp;
		}

		void SoldierAssignedToCamp(Soldier soldier)
		{
			soldiersCount += 1;
			UpdateValue();
		}
		
		void SoldierUnassignedToCamp(Soldier obj)
		{
			soldiersCount -= 1;
			UpdateValue();
		}

		void UpdateValue()
		{
			_textMeshPro.text = soldiersCount.ToString();
		}
	}
}