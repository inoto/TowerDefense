using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Mob : Unit
	{
		public static event Action<Unit, string> LookingForPathEvent;
		
		[Header("Mob")]
		public int FoodReward = 0;
		public int ReagentReward = 0;
		
		void Start()
		{
			StartCoroutine(CheckCreatedManually());
		}
	
		IEnumerator CheckCreatedManually()
		{
			yield return new WaitForSeconds(2f);
			if (!IsActive)
				Init("Path0");
		}

		public override void Init(string pathName = "")
		{
			base.Init();

			GetComponent<MoveByPath>().Init(pathName);

			// if (LookingForPathEvent != null)
			// 	LookingForPathEvent(this, pathName);
		}
		
	}
}