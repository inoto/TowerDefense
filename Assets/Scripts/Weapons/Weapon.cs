using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

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

		protected const int MOB_LAYER_MASK = 1 << 10;
		
		protected Transform _transform;
		protected Collider2D _collider;
		protected static Collider2D[] _targetsBuffer = new Collider2D[50];
		protected int _targetsCount;
		
		[SerializeField] bool debug = false;
		[SerializeField] Transform DebugITargetableTransform;
		
		[Header("Weapon")]
		public CanAttackTarget CanAttackTarget = CanAttackTarget.GroundAndAir;
		public DamageType DamageType = DamageType.Physical;
		public Priority TargetingTo = Priority.RandomTarget;

		public int Range = 200;
		public float AttackSpeed = 2f;
		public int DamageMin = 2;
		public int DamageMax = 4;
		public int Damage => UnityEngine.Random.Range(DamageMin, DamageMax);
		[ShowNativeProperty] float DamagePerSecond => (float)(DamageMin + DamageMax) / 2 / AttackSpeed;

		public ITargetable Target = null;

		[SerializeField] protected GameObject ProjectilePrefab;
		public Vector2 ProjectileStartPointOffset;
		public Tower Tower;

		protected virtual void Awake()
		{
			Tower = GetComponentInParent<Tower>();
			_transform = GetComponent<Transform>();
			_collider = GetComponent<Collider2D>();
		}

		void OnDisable()
		{
			Target = null;
		}

		protected bool AcquireTarget()
		{
			ContactFilter2D filter = new ContactFilter2D();
			filter.layerMask = MOB_LAYER_MASK;
			filter.useLayerMask = true;
			_targetsBuffer = new Collider2D[25];
			_targetsCount = Physics2D.OverlapCollider(_collider, filter, _targetsBuffer);
			if (_targetsCount > 0)
			{
				Target = DefineTarget();
				return true;
			}
			Target = null;
			return false;
		}
		
		protected bool TrackTarget()
		{
			if (Target == null)
				return false;

			if (Target.IsDied || !Physics2D.IsTouching(_collider, Target.Collider))
			{
				Target = null;
				return false;
			}
			
			return true;
		}

		protected ITargetable DefineTarget()
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
			for (int i = 0; i < _targetsCount; i++)
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
			for (int i = 0; i < _targetsCount; i++)
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
			for (int i = 0; i < _targetsCount; i++)
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
			for (int i = 0; i < _targetsCount; i++)
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

		protected virtual void ReleaseMissile()
		{
		}

		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawSphere((Vector2)transform.position+ProjectileStartPointOffset, 0.02f);
			Gizmos.color = Color.red;
			if (Target != null)
			{
				Gizmos.DrawLine(_transform.position, Target.Position);
			}
		}
	}
}