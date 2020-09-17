using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class ReagentResource : MonoBehaviour
	{
		int reagentsCount;
		
		TextMeshProUGUI _textMeshPro;

		void Awake()
		{
			_textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
		}
        
		void Start()
		{
			reagentsCount = 0;//Int32.Parse(Value.text);
			UpdateValue();
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
//			reagentValue += enemy.FoodReward;
			UpdateValue();
		}

		void UpdateValue()
		{
			_textMeshPro.text = reagentsCount.ToString();
		}
	}
}