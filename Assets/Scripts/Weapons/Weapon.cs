using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace TowerDefense
{
	public enum DamageType
	{
		Physical,
		Magical
	}

	public enum ArmorType
	{
		None,
		Fortified,
		Spiritual
	}

	public enum ArmorLevel
	{
		Weak,
		Good,
		Strong
	}

	public class Weapon : Order
	{
		public enum AttackState
		{
			NonAttacking,
			TargetInRange,
			TargetOutOfRange,
			TargetDied
		}

		public enum Priority
		{
			None,
			ClosestToExit,
			LowHealth,
			MostDangerous,
			LargestBunch,
			RandomTarget
		}

        public event Action<Weapon> TargetAcquiredEvent;
        public event Action<Weapon> TargetDiedEvent;
        public event Action<Weapon> TargetOutOfRangeEvent;
        public event Action<Weapon> TargetNowInRangeEvent;

		public DamageType DamageType = DamageType.Physical;
		public LayerMask LayerMask;
		
		public bool Ranged = false;
		[Tooltip("Seconds")]
		public float AttackInterval = 2f;

        [ProgressBar("LaunchProgress", 1f)] [SerializeField]
        protected float LaunchProgress = 1f;
		public int DamageMin = 2;
		public int DamageMax = 4;
		public int Damage => UnityEngine.Random.Range(DamageMin, DamageMax);
		[ShowNativeProperty] float DamagePerSecond => (float)(DamageMin + DamageMax) / 2 / AttackInterval;

		public ITargetable Target = null;
		[ReadOnly] public AttackState CurrentAttackState = AttackState.NonAttacking;

		[SerializeField] protected GameObject ProjectilePrefab;
		public Vector2 ProjectileStartPointOffset;

		protected static Collider2D[] _targetsBuffer = new Collider2D[GameController.MAX_TARGETS_BUFFER];
		protected int targetsCount;
		protected ContactFilter2D filter;
		
		protected Collider2D _collider;

		// orders
        protected MoveByTransform moveByTransform;
        protected MoveByPath moveByPath;

        protected override void Awake()
		{
			base.Awake();

			_collider = GetComponent<Collider2D>();
            moveByTransform = GetComponentInParent<MoveByTransform>();
            moveByPath = GetComponentInParent<MoveByPath>();
        }

		void OnEnable()
		{
			filter = new ContactFilter2D {layerMask = LayerMask, useLayerMask = true};
		}

		void Start()
		{
			if (ProjectilePrefab != null)
			{
				SimplePool.Preload(ProjectilePrefab, 5);
			}
			_targetsBuffer = new Collider2D[GameController.MAX_TARGETS_BUFFER];
		}

		void OnDisable()
		{
			Target = null;
		}
		
		void Update()
		{
			LaunchProgress += 1/AttackInterval * Time.deltaTime;
			while (LaunchProgress >= 1f)
			{
				if ((TrackTarget() || AcquireTarget()) && IsActive)
				{
					Attack();
					LaunchProgress -= 1f;
				}
				else
				{
					LaunchProgress = 0.999f;
				}
			}
		}
		
		protected virtual bool TrackTarget()
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

		protected virtual bool AcquireTarget()
		{
			targetsCount = Physics2D.OverlapCollider(_collider, filter, _targetsBuffer);
			if (targetsCount > 0)
			{
				Target = DefineTarget();
                return true;
			}
			Target = null;
			return false;
		}

		public virtual void SetTarget(ITargetable target, bool moveToTarget = true)
		{
			Target = target;
		}

		protected virtual void Attack()
		{
			if (Target == null)
				return;
			
			if (Ranged)
			{
				ReleaseProjectile();
			}
			else
			{
				Target.Damage(this);
			}
		}

		protected virtual ITargetable DefineTarget()
		{
			return null;
		}

		protected virtual void ReleaseProjectile()
		{
			SimplePool.Spawn(ProjectilePrefab, (Vector2)transform.position+ProjectileStartPointOffset, _transform.rotation)
				.GetComponent<Projectile>()
				.Init(this);
		}

        protected void RaiseTargetAcquiredEvent()
        {
	        CurrentAttackState = AttackState.NonAttacking;
			TargetAcquiredEvent?.Invoke(this);
        }

        protected void RaiseTargetDiedEvent()
        {
	        CurrentAttackState = AttackState.TargetDied;
			TargetDiedEvent?.Invoke(this);
        }

        protected void RaiseTargetOutOfRangeEvent()
        {
	        CurrentAttackState = AttackState.TargetOutOfRange;
			TargetOutOfRangeEvent?.Invoke(this);
        }

        protected void RaiseTargetInRangeEvent()
        {
	        CurrentAttackState = AttackState.TargetInRange;
            TargetNowInRangeEvent?.Invoke(this);

		}

		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawSphere((Vector2)transform.position+ProjectileStartPointOffset, 0.02f);
			Gizmos.color = Color.red;
			if (Target != null && !Target.IsDied)
			{
				Gizmos.DrawLine(_transform.position, Target.Position);
			}
		}
	}
}