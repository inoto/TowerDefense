using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace TowerDefense
{
	public abstract class Weapon : MonoBehaviour
	{
		public enum Priority
		{
			RandomTarget,
			ClosestToExit,
			LowHealth,
			MostDangerous,
			LargestBunch
		}

		[FormerlySerializedAs("showDebug")] [SerializeField] bool debug = false;
		
		[Header("Weapon")]
		public CanAttackTarget CanAttackTarget = CanAttackTarget.GroundAndAir;
		public DamageType DamageType = DamageType.Physical;
		public Priority TargetingTo = Priority.RandomTarget;

		public int Range = 200;
		public int AttackSpeed = 20;
		public int DamageMin = 2;
		public int DamageMax = 4;
		public int Damage
		{
			get { return Random.Range(DamageMin, DamageMax); }
		}

		public ITargetable Target = null;
		List<ITargetable> targetsInRange = new List<ITargetable>();
		
		public bool IsAttacking = false;
		bool isCoroutineActive = false;
		ITargetable possibleTarget = null;

		[SerializeField] GameObject projectilePrefab;
		public Vector2 ProjectileStartPointOffset;
		public Tower Tower;

		protected virtual void Awake()
		{
			Tower = GetComponentInParent<Tower>();
		}

		void OnDisable()
		{
			StopAttacking();
			StopAllCoroutines();
			isCoroutineActive = false;
			Target = null;
			targetsInRange.Clear();
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			ITargetable t = other.GetComponent<ITargetable>();
			targetsInRange.Add(t);
			t.InRangeByWeapon(this);
			
			if (debug) Debug.Log(String.Format("# {0} OnTriggerEnter2D {1}", gameObject.GetInstanceID(), t.GameObj.GetInstanceID()));
		}

		void OnTriggerExit2D(Collider2D other)
		{
			ITargetable t = other.GetComponent<ITargetable>();
			targetsInRange.Remove(t);
			t.NotInRangeByWeapon(this);
			
			if (t == Target)
			{
				if (debug) Debug.Log(String.Format("# {0} OnTriggerExit2D {1}", gameObject.GetInstanceID(), t.GameObj.GetInstanceID()));
				
				StopAttacking();
			}
		}

		void NewTarget(ITargetable target)
		{
			if (debug) Debug.Log(String.Format("# {0} NewTarget {1}", gameObject.GetInstanceID(), target.GameObj.GetInstanceID()));
			Target = target;

			
			
			StartCoroutine(Attack());
		}
		
		ITargetable DefineTarget()
		{
			switch (TargetingTo)
			{
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
			for (int i = 0; i < targetsInRange.Count; i++)
			{
				ITargetable possibleTarget = targetsInRange[i];

				if (possibleTarget != null && !possibleTarget.IsDied)
				{
					if (possibleTarget.PathIndex > biggestPathIndex)
					{
						bestTarget = possibleTarget;
						biggestPathIndex = possibleTarget.PathIndex;
						closestDistToWaypoint = (possibleTarget.WaypointPoint - possibleTarget.Point).magnitude;
					}
					else if (possibleTarget.PathIndex == biggestPathIndex)
					{
						float distToWaypoint = (possibleTarget.WaypointPoint - possibleTarget.Point).magnitude;
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
			foreach (var t in targetsInRange)
			{
				ITargetable possibleTarget = t;
				if (possibleTarget != null)
				{
					if (possibleTarget.Health < lowestValue)
					{
						bestTarget = possibleTarget;
						lowestValue = bestTarget.Health;
					}
				}
			}
			return bestTarget;
		}

		ITargetable FindMostDangerousTarget()
		{
			ITargetable bestTarget = null;
			float highestMaxHealth = 0;
			foreach (var t in targetsInRange)
			{
				ITargetable possibleTarget = t;
				if (possibleTarget != null)
				{
					if (possibleTarget.MaxHealth > highestMaxHealth)
					{
						bestTarget = possibleTarget;
						highestMaxHealth = bestTarget.Health;
					}
				}
			}
			return bestTarget;
		}

		ITargetable FindTargetInLargestBunch()
		{
			ITargetable bestTarget = null;
			float bunchRange = 1f;
			Dictionary<ITargetable, ITargetable[]> targetsWithBunches = new Dictionary<ITargetable, ITargetable[]>();
			foreach (var t in targetsInRange)
			{
				targetsWithBunches.Add(t, targetsInRange.FindAll((e) => Vector2.Distance(t.Point, e.Point) <= bunchRange).ToArray());
			}

			int maxCountInBunch = 0;
			foreach (KeyValuePair<ITargetable,ITargetable[]> kvp in targetsWithBunches)
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
			if (debug) Debug.Log(String.Format("# {0} OnTargetDied {1}", gameObject.GetInstanceID(), target.GameObj.GetInstanceID()));
			targetsInRange.Remove(target);
			target.NotInRangeByWeapon(this);

			if (Target == target)
				StopAttacking();
			
			Tower.ModifySpecValue(20); // TODO: replace with event
		}

		IEnumerator Attack()
		{
			if (debug) Debug.Log(String.Format("# {0} Attack {1}", gameObject.GetInstanceID(), Target.GameObj.GetInstanceID()));
			IsAttacking = true;
			isCoroutineActive = true;
			while (IsAttacking)
			{
				ReleaseMissile();

				yield return new WaitForSecondsRealtime(1 / (float)AttackSpeed * 40);
			}
			if (debug) Debug.Log(string.Format("# {0} Attack end", gameObject.GetInstanceID()));
			isCoroutineActive = false;

			possibleTarget = DefineTarget();
			if (possibleTarget != null)
				NewTarget(possibleTarget);
		}

		void StopAttacking()
		{
			IsAttacking = false;
//			StopAllCoroutines();
		}

		protected virtual void ReleaseMissile()
		{
			// TODO: Object Pool for projectiles
			Projectile proj = Instantiate(projectilePrefab, (Vector2)transform.position+ProjectileStartPointOffset, transform.rotation).GetComponent<Projectile>();
			proj.Init(this);
		}

		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.DrawSphere((Vector2)transform.position+ProjectileStartPointOffset, 0.05f);
			Gizmos.color = Color.yellow;
			for (int i = 0; i < targetsInRange.Count; i++)
			{
				Gizmos.DrawLine(transform.position, targetsInRange[i].Point);
			}
			Gizmos.color = Color.red;
			if (Target != null)
			{
				Gizmos.DrawLine(transform.position, Target.Point);
			}
		}
	}
}