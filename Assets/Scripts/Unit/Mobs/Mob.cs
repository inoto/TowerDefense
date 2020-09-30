using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class Mob : Unit
	{
        public bool CreatedManually = false;
		public int FoodReward = 0;

		Tower DefendingTower;
        Queue<Soldier> soldiersQueue = new Queue<Soldier>();

        [ShowNativeProperty] public int NumberOfSoldiersInQueue => soldiersQueue.Count;

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
            SpawnFood();
        }

        void SpawnFood()
        {
	        var food = UnitFactory.Instance.SpawnObject<Food>(UnitFactory.Type.Food);
	        food.transform.position = transform.position;
	        food.Amount = FoodReward;
        }

        public void DefendTower(Tower tower, Soldier soldier)
        {
	        DefendingTower = tower;
            if (soldiersQueue.Count == 0)
				this.Weapon.SetTarget(soldier);
            soldiersQueue.Enqueue(soldier);
            soldier.DiedEvent += OnSoldierDied;
        }

        public void AddToQueue(Soldier soldier)
        {
	        soldiersQueue.Enqueue(soldier);
        }

        void OnSoldierDied(Unit soldier)
        {
	        soldier.DiedEvent -= OnSoldierDied;

	        soldiersQueue.Dequeue();
	        if (soldiersQueue.Count > 0)
	        {
		        var nextSoldier = soldiersQueue.Peek();
		        this.Weapon.SetTarget(nextSoldier);
		        nextSoldier.DiedEvent += OnSoldierDied;
	        }
            else
	        {
                if (DefendingTower.OccupiedByEnemy)
                {
	                _moveByTransform.AssignTransform(DefendingTower.transform);
	                ArrivedDestinationEvent += EnterTower;
                }
                else
                    _moveByPath.AssignToClosestWaypoint();
	        }
        }

        void EnterTower()
        {
	        ArrivedDestinationEvent -= EnterTower;

	        if (DefendingTower.OccupiedByEnemy)
				DefendingTower.GetComponent<OccupiedByEnemy>().ReturnMob(this);
            else
		        _moveByPath.AssignToClosestWaypoint();
        }
    }
}