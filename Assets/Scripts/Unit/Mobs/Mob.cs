using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Mob : Unit
	{
        [Header("Mob")]
        public bool CreatedManually = false;
		public int FoodReward = 0;
		public int ReagentReward = 0;

        [SerializeField] MoveByPath _moveByPath;

		void Start()
		{
			StartCoroutine(CheckCreatedManually());
		}
	
		IEnumerator CheckCreatedManually()
		{
			yield return new WaitForSeconds(2f);
            if (CreatedManually)
            {
                MoveByPath mbp = GetComponent<MoveByPath>();
                mbp.SetPath("Path0");
                // AddOrder(mbp, CurrentOrder == null);
			}
        }
    }
}