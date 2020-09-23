using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Mob : Unit
	{
        public bool CreatedManually = false;
		public int FoodReward = 0;

        [SerializeField] MobStatsData StatsData;
        [SerializeField] MoveByPath _moveByPath;

		void Start()
        {
            LoadData();
            StartCoroutine(CheckCreatedManually());
		}

        void LoadData()
        {
            _healthy.SetMaxHealth(StatsData.Hp);

            MobWeapon weapon = GetComponentInChildren<MobWeapon>();
            weapon.DamageMin = StatsData.DamageMin;
            weapon.DamageMax = StatsData.DamageMax;
            weapon.AttackInterval = StatsData.AttackRate;

            _healthy.ArmorType = StatsData.Armor;

            _moveByPath.Speed = (float) StatsData.Speed;

            MoveByTransform mbt = GetComponent<MoveByTransform>();
            mbt.Speed = (float) StatsData.Speed;

            FoodReward = StatsData.FoodReward;
        }
	
		IEnumerator CheckCreatedManually()
		{
			yield return new WaitForSeconds(2f);
            if (CreatedManually)
            {
	            _moveByPath.SetPath("Path0");
                // AddOrder(mbp, CurrentOrder == null);
			}
        }

        protected override void Corpse()
        {
            base.Corpse();

            StopMoving();
            _moveByPath.Pause();
            var food = UnitFactory.Instance.SpawnObject<Food>(UnitFactory.Type.Food);
            food.transform.position = transform.position;
            food.Amount = FoodReward;
        }
    }
}