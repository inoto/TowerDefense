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
			Camp = 0,
			Low,
			Normal,
			High
		}

		[SerializeField] bool Debug = false;

		Dictionary<int, List<Building>> _priorities;
		Camp _camp;

		void Start()
		{
			Init();
		}

		void Init()
		{
			_priorities = new Dictionary<int, List<Building>>();
			foreach (var value in Enum.GetValues(typeof(Priority)))
			{
				_priorities.Add((int) value, new List<Building>());
			}

			_camp = FindObjectOfType<Camp>();
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
				_priorities[(int)tower.PriorityForDesired].Add(building);
//				Debug.Log("tower built " + tower);
			}
			else
			{
				_priorities[0].Add(building);
//				Debug.Log("camp built " + building);
			}

		}

		void PriorityChanged(Tower tower, Priority oldPriority, Priority newPriority)
		{
			for (int i = 0; i < _priorities[(int)oldPriority].Count; i++)
			{
				if (_priorities[(int)oldPriority][i] == tower)
				{
					Building t = _priorities[(int)oldPriority][i];
					_priorities[(int)oldPriority].RemoveAt(i);
					_priorities[(int)newPriority].Add(t);
				}
			}

			if (oldPriority < newPriority) // priority +
			{
				if (tower.Soldiers.Count >= tower.DesiredCount) // no need more soldiers
					return;

				foreach (int section in _priorities.Keys)
				{
					if (section >= (int)tower.PriorityForDesired || tower.Soldiers.Count >= tower.DesiredCount)
						break;

					// find towers with soldiers and take missed soldiers
					FindBestBuildingToTakeSoldier(tower);
				}
			}
			else // priority -
			{
				if (tower.Soldiers.Count <= 0) // no soldiers to give, return
					return;

				// find towers where soldiers are needed and give it
				GiveSoldierToBestBuildingSafe(tower);
			}
		}

		void DesiredChanged(Tower tower)
		{
			if (tower.DesiredCount > tower.Soldiers.Count)
			{
				FindBestBuildingToTakeSoldier(tower);
			}
			else if (tower.DesiredCount < tower.Soldiers.Count)
			{
				// find towers where soldiers are needed and give it
				GiveSoldierToBestBuilding(tower);
			}
		}
		
		void FreeSoldier(Soldier soldier)
		{
			Building bestBuilding = null;
			float lowestDistance = float.MaxValue;
			foreach (int section in _priorities.Keys.Reverse())
			{
				if (section == 0 && bestBuilding != null)
					break;
				if (Debug) UnityEngine.Debug.Log(string.Format("towers count in section {0}: {1}", (Priority) section, _priorities[section].Count));
				foreach (Building b in _priorities[section])
				{
					Tower t = b as Tower;
					if (Debug) UnityEngine.Debug.Log("building candidate: " + b.gameObject.name);
					if (t != null)
					{
						if (t.DesiredCount > 0 && t.Soldiers.Count < t.DesiredCount)
						{
							float distance = Vector2.Distance(soldier.Point, t.transform.position);
							if (distance < lowestDistance || bestBuilding == null)
							{
								lowestDistance = distance;
								bestBuilding = t;
							}
						}
					}
					else if (b is Camp)
					{
						float distance = Vector2.Distance(soldier.Point, b.transform.position);
						if (distance < lowestDistance || bestBuilding == null)
						{
							lowestDistance = distance;
							bestBuilding = b;
						}
					}
				}
			}
			
			if (bestBuilding != null)
				soldier.AssignToBuilding(bestBuilding);
		}

		void FindBestBuildingToTakeSoldier(Tower tower)
		{
			foreach (int section in _priorities.Keys)
			{
				if (section >= (int)tower.PriorityForDesired)
					return;
				if (Debug) UnityEngine.Debug.Log(string.Format("towers count in section {0}: {1}", (Priority) section, _priorities[section].Count));
				foreach (Building b in _priorities[section])
				{
					if (Debug) UnityEngine.Debug.Log("building candidate: " + b.gameObject.name);
					if (b.Soldiers.Count > 0)
					{
						b.RemoveSoldier().AssignToBuilding(tower);
						return;
					}
				}
			}
		}
		
		void GiveSoldierToBestBuilding(Tower tower)
		{
			Building bestBuilding = null;
			float lowestDistance = float.MaxValue;
			foreach (int section in _priorities.Keys.Reverse())
			{
				if (section == 0 && bestBuilding != null)
					break;
				if (Debug) UnityEngine.Debug.Log(string.Format("towers count in section {0}: {1}", (Priority) section, _priorities[section].Count));
				foreach (Building b in _priorities[section])
				{
					Tower t = b as Tower;
					if (Debug) UnityEngine.Debug.Log("building candidate: " + b.gameObject.name);
					if (t != null)
					{
						if (t == tower)
							continue;
						
						if (t.DesiredCount > 0 && t.Soldiers.Count < t.DesiredCount)
						{
							float distance = Vector2.Distance(tower.transform.position, t.transform.position);
							if (distance < lowestDistance || bestBuilding == null)
							{
								lowestDistance = distance;
								bestBuilding = t;
							}
						}
					}
					else if (b is Camp)
					{
						float distance = Vector2.Distance(tower.transform.position, b.transform.position);
						if (distance < lowestDistance || bestBuilding == null)
						{
							lowestDistance = distance;
							bestBuilding = b;
						}
					}
				}
			}
			
			if (bestBuilding != null)
				tower.RemoveSoldier().AssignToBuilding(bestBuilding);
		}
		
		void GiveSoldierToBestBuildingSafe(Tower tower)
		{
			Building bestBuilding = null;
			float lowestDistance = float.MaxValue;
			foreach (int section in _priorities.Keys.Reverse())
			{
				if (section == 0 && bestBuilding != null)
					break;
				
				if (Debug) UnityEngine.Debug.Log(string.Format("towers count in section {0}: {1}", (Priority) section, _priorities[section].Count));
				foreach (Building b in _priorities[section])
				{
					Tower t = b as Tower;
					if (Debug) UnityEngine.Debug.Log("building candidate: " + b.gameObject.name);
					if (t != null)
					{
						if (t == tower)
							continue;
						
						if (t.PriorityForDesired > tower.PriorityForDesired
						    && t.DesiredCount > 0
						    && t.Soldiers.Count < t.DesiredCount)
						{
							float distance = Vector2.Distance(tower.transform.position, t.transform.position);
							if (distance < lowestDistance || bestBuilding == null)
							{
								lowestDistance = distance;
								bestBuilding = t;
							}
						}
					}
					else if (b is Camp && tower.DesiredCount <= 0)
					{
						float distance = Vector2.Distance(tower.transform.position, b.transform.position);
						if (distance < lowestDistance || bestBuilding == null)
						{
							lowestDistance = distance;
							bestBuilding = b;
						}
					}
				}
			}
			if (bestBuilding != null)
				tower.RemoveSoldier().AssignToBuilding(bestBuilding);
		}
	}
}