using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace TowerDefense
{
	public class Weapon : Order
	{
		public enum Priority
		{
			RandomTarget,
			ClosestToExit,
			LowHealth,
			MostDangerous,
			LargestBunch
		}

        public event Action TargetAcquiredEvent;
        public event Action TargetDiedEvent;
        public event Action TargetOutOfRangeEvent;
        public event Action TargetNowInRangeEvent;


		public CanAttackTarget CanAttackTarget = CanAttackTarget.GroundAndAir;
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

		[SerializeField] protected GameObject ProjectilePrefab;
		public Vector2 ProjectileStartPointOffset;

		protected static Collider2D[] _targetsBuffer = new Collider2D[GameController.MAX_TARGETS_BUFFER];
		protected int targetsCount;
		protected ContactFilter2D filter;
		
		protected Collider2D _collider;

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
			_targetsBuffer = new Collider2D[GameController.MAX_TARGETS_BUFFER];
			targetsCount = Physics2D.OverlapCollider(_collider, filter, _targetsBuffer);
			if (targetsCount > 0)
			{
				Target = DefineTarget();
                return true;
			}
			Target = null;
			return false;
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
			TargetAcquiredEvent?.Invoke();
        }

        protected void RaiseTargetDiedEvent()
        {
			TargetDiedEvent?.Invoke();
        }

        protected void RaiseTargetOutOfRangeEvent()
        {
			TargetOutOfRangeEvent?.Invoke();
        }

        protected void RaiseTargetInRangeEvent()
        {
            TargetNowInRangeEvent?.Invoke();

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