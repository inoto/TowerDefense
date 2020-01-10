using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace TowerDefense
{
	public class SoldiersDispenser : MonoBehaviour
	{
		public enum Priority
		{
			Low = 0,
			Normal,
			High
		}

		[SerializeField] bool debug = false;
		public bool Balance = true;

		Dictionary<int, List<Tower>> priorities;
		List<Camp> camps;

		void Start()
		{
			Init();
		}

		void Init()
		{
			priorities = new Dictionary<int, List<Tower>>();
			foreach (var value in Enum.GetValues(typeof(Priority)))
			{
				priorities.Add((int) value, new List<Tower>());
			}

			camps = FindObjectsOfType<Camp>().ToList();
		}

		void OnEnable()
		{
			Building.BuiltEvent += TowerBuilt;
			Tower.PriorityChangedEvent += PriorityChanged;
			Tower.DesiredChangedEvent += DesiredChanged;
			Soldier.FreeSoldierEvent += FreeSoldier;
		}

		void OnDisable()
		{
			Building.BuiltEvent -= TowerBuilt;
			Tower.PriorityChangedEvent -= PriorityChanged;
			Tower.DesiredChangedEvent -= DesiredChanged;
			Soldier.FreeSoldierEvent -= FreeSoldier;
		}

		void TowerBuilt(Building building)
		{
			Tower tower = building.GetComponent<Tower>();
			if (tower != null)
			{
				priorities[(int)tower.PriorityForDesired].Add(tower);
				if (debug) Debug.Log($"tower built {tower}");
			}
			else
			{
				camps.Add(building as Camp);
				if (debug) Debug.Log($"camp built {building}");
			}
		}

		void PriorityChanged(Tower tower, Priority oldPriority)
		{
			// update priority data structure
			for (int i = 0; i < priorities[(int)oldPriority].Count; i++)
			{
				if (priorities[(int)oldPriority][i] == tower)
				{
					Tower t = priorities[(int)oldPriority][i];
					priorities[(int)oldPriority].RemoveAt(i);
					priorities[(int)tower.PriorityForDesired].Add(t);
				}
			}

			if (oldPriority < tower.PriorityForDesired) // priority +
			{
				if (tower.Soldiers.Count >= tower.DesiredCount) // no need more soldiers, return
					return;
				if (debug) Debug.Log($"PriorityChanged+ for tower {tower.gameObject}");

				NeedMoreSoldiers(tower, tower.DesiredCount - tower.Soldiers.Count);
			}
			else // priority -
			{
				if (tower.Soldiers.Count <= 0) // no soldiers to give, return
					return;
				if (debug) Debug.Log($"PriorityChanged- for tower {tower.gameObject}");
				// find towers where soldiers are needed and give it
				List<Soldier> copiedSoldiers = new List<Soldier>(tower.Soldiers);
				foreach (var soldier in copiedSoldiers)
				{
					Tower t = FindTowerWithHigherPriority(tower);
					if (t != null)
						tower.RemoveSoldier().AssignToBuilding(t);
				}
			}
		}

		void DesiredChanged(Tower tower)
		{
			if (debug) Debug.Log($"DesiredChanged for tower {tower.gameObject}");
			if (tower.DesiredCount > tower.Soldiers.Count)
			{
				NeedMoreSoldiers(tower, tower.DesiredCount-tower.Soldiers.Count);
			}
			else if (tower.DesiredCount < tower.Soldiers.Count)
			{
				tower.RemoveSoldier().NowFree();
			}
		}

		void NeedMoreSoldiers(Tower tower, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Soldier soldier = FindSoldier(tower);
				if (soldier != null)
					soldier.AssignToBuilding(tower);
			}
		}

		Soldier FindSoldier(Tower tower)
		{
			if (debug) Debug.Log($"FindSoldier for tower: {tower.gameObject.name}");
			Camp camp = FindClosestCamp(tower.transform, true);
			if (camp != null)
				return camp.RemoveSoldier();
			
			List<Tower> towers = new List<Tower>();
			
			foreach (int priority in priorities.Keys)
			{
				if (priority >= (int)tower.PriorityForDesired)
					break;
				// if (debug) Debug.Log($"towers count in priority {(Priority)priority}: {_priorities[priority].Count}");
				foreach (Tower t in priorities[priority])
				{
					if (debug) Debug.Log("tower candidate: " + t.gameObject.name);
					if (t.SoldiersCount > 0)
					{
						towers.Add(t);
					}
				}
			}
			if (towers.Count > 0)
				return WeighLowPriority(tower.transform, towers).RemoveSoldier();

			return null;
		}
		
		void FreeSoldier(Soldier soldier)
		{
			if (debug) Debug.Log($"FreeSoldier: {soldier.gameObject}");
			Tower tower = FindTowerForFreeSoldier(soldier);
			if (tower != null)
			{
				if (debug) Debug.Log($"FreeSoldier tower found: {tower.gameObject}");
				soldier.AssignToBuilding(tower);
			}
			else
			{
				Camp camp = FindClosestCamp(soldier.transform);
				if (camp != null)
				{
					soldier.AssignToBuilding(camp);
				}
			}
		}

		Tower FindTowerForFreeSoldier(Soldier soldier)
		{
			if (debug) Debug.Log($"FindTowerForFreeSoldier: {soldier.gameObject}");
			List<Tower> bestTowers = new List<Tower>();
			foreach (int priority in priorities.Keys.Reverse())
			{
				foreach (Tower t in priorities[priority])
				{
					if (t.DesiredCount > 0 && t.Soldiers.Count < t.DesiredCount)
					{
						// if (debug) Debug.Log("tower candidate: " + t.gameObject);
						bestTowers.Add(t);
					}
				}
			}

			if (bestTowers.Count > 0)
				return Weigh(soldier.transform, bestTowers);

			return null;
		}

		Camp FindClosestCamp(Transform trans, bool withSoldiers = false)
		{
			float lowestDistance = float.MaxValue;
			Camp campWithLowestDistance = null;
			foreach (Camp c in camps)
			{
				if (withSoldiers && c.SoldiersCount <= 0)
					continue;
				
				if (debug) Debug.Log($"camp candidate: {c.gameObject}");
				float distance = Vector2.Distance(trans.position, c.transform.position);
				if (distance < lowestDistance)
				{
					lowestDistance = distance;
					campWithLowestDistance = c;
				}
			}
			return campWithLowestDistance;
		}

		Tower FindTowerWithHigherPriority(Tower tower)
		{
			if (debug) Debug.Log($"FindTowerWithHigherPriority: {tower.gameObject}");
			List<Tower> bestTowers = new List<Tower>();
			foreach (int priority in priorities.Keys.Reverse())
			{
				// if (debug) Debug.Log($"towers count in priority {(Priority)priority}: {_priorities[priority].Count}");
				foreach (Tower t in priorities[priority])
				{
					if (t == tower)
						continue;
						
					if (t.PriorityForDesired > tower.PriorityForDesired
					    && t.DesiredCount > 0 && t.Soldiers.Count < t.DesiredCount)
					{
						// if (debug) Debug.Log("tower candidate: " + t.gameObject);
						bestTowers.Add(t);
					}
				}
			}

			if (bestTowers.Count > 0)
				return Weigh(tower.transform, bestTowers);

			return null;
		}

		Tower Weigh(Transform trans, List<Tower> towers)
		{
			Dictionary<Tower, float> weights = new Dictionary<Tower, float>();
			for (int i = 0; i < towers.Count; i++)
			{
				float distance = Vector2.Distance(trans.position, towers[i].transform.position);
				weights.Add(towers[i], 1/distance);
				if (Balance)
				{
					int diff = towers[i].DesiredCount - towers[i].SoldiersCount;
					weights[towers[i]] += diff * 2;
					if (diff > 0 && towers[i].SoldiersCount == 0)
						weights[towers[i]] += diff;
				}
				int priority = (int)towers[i].PriorityForDesired;
				weights[towers[i]] += priority*3;
				if (debug) Debug.Log($"tower {towers[i].gameObject} weight: {weights[towers[i]]}");
			}
			
			float max = weights.Values.Max();
			Tower bestTower = weights.FirstOrDefault(x => x.Value == max).Key;
			Debug.Log($"Weigh best tower: {bestTower.gameObject}");
			return bestTower;
		}

		Tower WeighLowPriority(Transform trans, List<Tower> towers)
		{
			Dictionary<Tower, float> weights = new Dictionary<Tower, float>();
			for (int i = 0; i < towers.Count; i++)
			{
				float distance = Vector2.Distance(trans.position, towers[i].transform.position);
				weights.Add(towers[i], 1/distance/10);
				if (Balance)
				{
					int diff = towers[i].DesiredCount - towers[i].SoldiersCount;
					weights[towers[i]] -= diff;
				}
				int priority = (int)towers[i].PriorityForDesired;
				weights[towers[i]] -= priority;
				if (debug) Debug.Log($"tower {towers[i].gameObject} weight: {weights[towers[i]]}");
			}
			
			float max = weights.Values.Max();
			Tower bestTower = weights.FirstOrDefault(x => x.Value == max).Key;
			Debug.Log($"Weigh best tower: {bestTower.gameObject}");
			return bestTower;
		}
	}
}