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

		Dictionary<int, List<Tower>> _priorities;
		List<Camp> _camps;

		void Start()
		{
			Init();
		}

		void Init()
		{
			_priorities = new Dictionary<int, List<Tower>>();
			foreach (var value in Enum.GetValues(typeof(Priority)))
			{
				_priorities.Add((int) value, new List<Tower>());
			}

			_camps = FindObjectsOfType<Camp>().ToList();
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
				_priorities[(int)tower.PriorityForDesired].Add(tower);
				if (debug) Debug.Log("tower built " + tower);
			}
			else
			{
				_camps.Add(building as Camp);
				if (debug) Debug.Log("camp built " + building);
			}
		}

		void PriorityChanged(Tower tower, Priority oldPriority)
		{
			// update priority data structure
			for (int i = 0; i < _priorities[(int)oldPriority].Count; i++)
			{
				if (_priorities[(int)oldPriority][i] == tower)
				{
					Tower t = _priorities[(int)oldPriority][i];
					_priorities[(int)oldPriority].RemoveAt(i);
					_priorities[(int)tower.PriorityForDesired].Add(t);
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
			Tower bestTower = null;
			if (debug) Debug.Log($"FindSoldier for tower: {tower.gameObject.name}");
			Camp camp = FindClosestCamp(tower.transform, true);
			if (camp != null)
				return camp.RemoveSoldier();
			
			foreach (int priority in _priorities.Keys)
			{
				if (priority >= (int)tower.PriorityForDesired)
					break;
				// if (debug) Debug.Log($"towers count in priority {(Priority)priority}: {_priorities[priority].Count}");
				foreach (Tower t in _priorities[priority])
				{
					if (debug) Debug.Log("tower candidate: " + t.gameObject.name);
					if (t.SoldiersCount > 0)
					{
						if (bestTower == null)
							bestTower = t;
						else if (t.SoldiersCount > bestTower.SoldiersCount)
							bestTower = t;
					}
				}
			}
			if (bestTower != null)
				return bestTower.RemoveSoldier();

			return null;
		}
		
		void FreeSoldier(Soldier soldier)
		{
			if (debug) Debug.Log($"FreeSoldier: {soldier.gameObject.name}");
			Tower tower = FindTowerForFreeSoldier(soldier);
			if (tower != null)
			{
				if (debug) Debug.Log($"FreeSoldier tower found: {tower.gameObject.name}");
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
			if (debug) Debug.Log($"FindTowerForFreeSoldier: {soldier.gameObject.name}");
			List<Tower> bestTowers = new List<Tower>();
			float lowestDistance = float.MaxValue;
			Tower towerWithLowestDistance = null;
			foreach (int priority in _priorities.Keys.Reverse())
			{
				foreach (Tower t in _priorities[priority])
				{
					if (t.DesiredCount > 0 && t.Soldiers.Count < t.DesiredCount)
					{
						if (debug) Debug.Log("tower candidate: " + t.gameObject);
						bestTowers.Add(t);
						float distance = Vector2.Distance(soldier.transform.position, t.transform.position);
						if (distance < lowestDistance)
						{
							lowestDistance = distance;
							towerWithLowestDistance = t;
						}
					}
				}
			}

			if (Balance && bestTowers.Count > 0)
				return Rebalance(soldier.transform, bestTowers);
			
			if (towerWithLowestDistance != null)
				return towerWithLowestDistance;

			return null;
		}

		Camp FindClosestCamp(Transform trans, bool withSoldiers = false)
		{
			float lowestDistance = float.MaxValue;
			Camp campWithLowestDistance = null;
			foreach (Camp c in _camps)
			{
				if (withSoldiers && c.SoldiersCount <= 0)
					continue;
				
				if (debug) Debug.Log("camp candidate: " + c.gameObject);
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
			Tower bestTower = null;
			float lowestDistance = float.MaxValue;
			List<Tower> bestTowers = new List<Tower>();
			foreach (int priority in _priorities.Keys.Reverse())
			{
				if (debug) Debug.Log($"towers count in priority {(Priority)priority}: {_priorities[priority].Count}");
				foreach (Tower t in _priorities[priority])
				{
					if (t == tower)
						continue;
						
					if (t.PriorityForDesired > tower.PriorityForDesired
					    && t.DesiredCount > 0 && t.Soldiers.Count < t.DesiredCount)
					{
						if (debug) Debug.Log("tower candidate: " + t.gameObject);
						bestTowers.Add(t);
						float distance = Vector2.Distance(tower.transform.position, t.transform.position);
						if (distance < lowestDistance)
						{
							lowestDistance = distance;
							bestTower = t;
						}
					}
				}
			}

			if (Balance && bestTowers.Count > 0)
				return Rebalance(tower.transform, bestTowers);

			if (bestTower != null)
				return bestTower;

			return null;
		}

		Tower Rebalance(Transform trans, List<Tower> towers)
		{
			Tower bestTower = null;
			int lowestCount = int.MaxValue;
			float lowestDistance = float.MaxValue;
			for (int i = 0; i < towers.Count; i++)
			{
				float distance = Vector2.Distance(trans.position, towers[i].transform.position);
				if (towers[i].SoldiersCount < lowestCount && distance < lowestDistance)
				{
					lowestCount = towers[i].SoldiersCount;
					lowestDistance = distance;
					bestTower = towers[i];
				}
			}
			if (bestTower != null)
				return bestTower;

			return null;
		}
	}
}