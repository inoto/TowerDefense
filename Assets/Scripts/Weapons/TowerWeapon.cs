using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
	public class TowerWeapon : Weapon
	{
		public Tower Tower;
		public Priority TargetingTo = Priority.RandomTarget;

		protected override void Awake()
		{
			base.Awake();
			
			Tower = GetComponentInParent<Tower>();
		}

		protected override ITargetable DefineTarget()
		{
			switch (TargetingTo)
			{
				case Priority.None:
				case Priority.RandomTarget:
				case Priority.ClosestToExit:
					return FindClosestToExitTarget();
				case Priority.LowHealth:
					return FindLowestHealthTarget();
				case Priority.MostDangerous:
					return FindMostDangerousTarget();
				case Priority.LargestBunch:
					return FindTargetInLargestBunch();
			}
			
			return null;
		}

		#region Targeting

		ITargetable FindClosestToExitTarget()
		{
			ITargetable bestTarget = null;
			int biggestPathIndex = -1;
			float closestDistToWaypoint = float.MaxValue;
			for (int i = 0; i < targetsCount; i++)
			{
				ITargetable possibleTarget = _targetsBuffer[i].GetComponent<ITargetable>();

				if (possibleTarget != null && !possibleTarget.IsDied)
				{
					MoveByPath mbp = possibleTarget.GameObj.GetComponent<MoveByPath>();
					if (mbp.PathIndex > biggestPathIndex)
					{
						bestTarget = possibleTarget;
						biggestPathIndex = mbp.PathIndex;
						closestDistToWaypoint = (mbp.WaypointPoint - possibleTarget.Position).magnitude;
					}
					else if (mbp.PathIndex == biggestPathIndex)
					{
						float distToWaypoint = (mbp.WaypointPoint - possibleTarget.Position).magnitude;
						if (distToWaypoint < closestDistToWaypoint)
						{
							bestTarget = possibleTarget;
							closestDistToWaypoint = distToWaypoint;
						}
					}
				}
			}

			return bestTarget;
		}
		
		ITargetable FindLowestHealthTarget()
		{
			ITargetable bestTarget = null;
			float lowestValue = float.MaxValue;
			for (int i = 0; i < targetsCount; i++)
			{
				ITargetable possibleTarget = _targetsBuffer[i].GetComponent<ITargetable>();
				if (possibleTarget != null)
				{
					if (possibleTarget.MaxHealth < lowestValue)
					{
						bestTarget = possibleTarget;
						lowestValue = bestTarget.MaxHealth;
					}
				}
			}
			return bestTarget;
		}

		ITargetable FindMostDangerousTarget()
		{
			ITargetable bestTarget = null;
			float highestMaxHealth = 0;
			for (int i = 0; i < targetsCount; i++)
			{
				ITargetable possibleTarget = _targetsBuffer[i].GetComponent<ITargetable>();
				if (possibleTarget != null)
				{
					if (possibleTarget.MaxHealth > highestMaxHealth)
					{
						bestTarget = possibleTarget;
						highestMaxHealth = bestTarget.MaxHealth;
					}
				}
			}
			return bestTarget;
		}

		ITargetable FindTargetInLargestBunch()
		{
			ITargetable bestTarget = null;
			float bunchRange = 1f;
			Dictionary<ITargetable, Collider2D[]> targetsWithBunches = new Dictionary<ITargetable, Collider2D[]>();
			for (int i = 0; i < targetsCount; i++)
			{
				ITargetable t = _targetsBuffer[i].GetComponent<ITargetable>();
				targetsWithBunches.Add(t, _targetsBuffer.Where((e)=> Vector2.Distance(t.Position, e.transform.position) <= bunchRange).ToArray());
			}

			int maxCountInBunch = 0;
			foreach (KeyValuePair<ITargetable,Collider2D[]> kvp in targetsWithBunches)
			{
				if (kvp.Value.Length > maxCountInBunch)
				{
					maxCountInBunch = kvp.Value.Length;
					bestTarget = kvp.Key;
				}
			}
			
			return bestTarget;
		}
		
		#endregion
		
		public void OnTargetDied(ITargetable target)
		{

			Tower.ModifySpecValue(20); // TODO: replace with event
		}
	}
}