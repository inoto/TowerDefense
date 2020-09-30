using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class OccupiedByEnemy : MonoBehaviour
	{
		[SerializeField] Weapon weapon = null;
		[SerializeField] List<Mob> mobPrefabs = new List<Mob>();

		public int NumberOfAliveMobs => mobs.Count;

		[SerializeField] List<Mob> mobs = new List<Mob>();

		Tower _tower;

		void Awake()
		{
			_tower = GetComponent<Tower>();
		}

		void Start()
		{
			for (int i = 0; i < mobPrefabs.Count; i++)
			{
				var go = Instantiate(mobPrefabs[i].gameObject, transform);
				go.transform.position = transform.position;
				go.SetActive(false);
				go.name = $"{go.name} {i}";
				go.GetComponent<Mob>().Weapon.gameObject.name = $"Weapon {i}";
				mobs.Add(go.GetComponent<Mob>());
				mobs[mobs.Count - 1].DiedEvent += OnMobDied;
			}
			_tower.SoldiersCountCloud.TowerOccupied(this);
		}

		void OnMobDied(Unit mob)
		{
			mob.DiedEvent -= OnMobDied;

			mobs.Remove(mob as Mob);
			_tower.SoldiersCountCloud.TowerOccupied(this);
		}

		public Mob CallMob(Soldier soldier)
		{
			Mob mob = null;
			int lowestNumberOfSoldiers = int.MaxValue;
			Mob suitableMob = null;
			for (int i = 0; i < mobs.Count; i++)
			{
				if (mobs[i].NumberOfSoldiersInQueue < lowestNumberOfSoldiers)
				{
					lowestNumberOfSoldiers = mobs[i].NumberOfSoldiersInQueue;
					suitableMob = mobs[i];
				}
			}
			mob = suitableMob;
			if (mob.NumberOfSoldiersInQueue == 0)
			{
				mob.gameObject.SetActive(true);
				mob.DefendTower(_tower, soldier);

			}
			else
				mob.AddToQueue(soldier);

			return mob;
		}

		public void ReturnMob(Mob mob)
		{
			mob.gameObject.SetActive(false);
		}

		
	}
}