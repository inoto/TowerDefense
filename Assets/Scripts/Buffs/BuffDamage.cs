using System;
using UnityEngine;

namespace TowerDefense
{
	public class BuffDamage : MonoBehaviour
	{
		public int DamagePerSecond = 5;
		public float SecondsDuration = 2f;

		float timer = 0f, counter = 0f;

		Unit _unit;

		void Awake()
		{
			_unit = GetComponent<Unit>();
		}

		void Update()
		{
			timer += Time.deltaTime;
			
			if (timer >= 1f)
			{
				timer = 0f;
				_unit.Damage(DamagePerSecond);
				
				if ((counter + timer) >= SecondsDuration)
					Destroy(this);
				else
					counter += timer;
			}
			
			
		}
	}
}