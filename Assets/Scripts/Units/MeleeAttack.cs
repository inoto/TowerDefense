using System;
using UnityEngine;
using NaughtyAttributes;

namespace TowerDefense
{
	[RequireComponent(typeof(CircleCollider2D))]
	public class MeleeAttack : MonoBehaviour, IUnitOrder
	{
		public bool IsActive = false;
		[Space]
		[ReadOnly] bool followingTarget = false;
		[Tooltip("Melee <= 0.5f")]
		public float AttackRange = 0.5f;
		public float AttackSpeed = 1.5f;
		public int AttackDamage = 2;
		public DamageType AttackType = DamageType.Physical;
		[SerializeField] LayerMask LayerMask;
		[ReadOnly] public ITargetable Target;
		[ProgressBar("LaunchProgress", 1f)]
		[SerializeField] float LaunchProgress;

		Transform _transform;
		Collider2D _collider;
		MoveByPath _moveByPath;
		ICanAttack _attacking;
		static Collider2D[] _targetsBuffer;
		int _targetsCount;

		void Awake()
		{
			_transform = GetComponent<Transform>();
			_collider = GetComponent<Collider2D>();
			_moveByPath = GetComponentInParent<MoveByPath>();
			_attacking = GetComponentInParent<ICanAttack>();
		}

		void Update()
		{
			LaunchProgress += 1/AttackSpeed * Time.deltaTime;
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
		
		bool TrackTarget()
		{
			if (Target == null)
				return false;

			if (Target.IsDied || !Target.GameObj.activeSelf)
			{
				if (followingTarget)
				{
					followingTarget = false;
					_attacking.OrderEnded(GetComponentInParent<MoveByTransform>());
				}

				Target = null;
				EndAttacking();
			}

			if (!followingTarget && !Physics2D.IsTouching(_collider, Target.Collider))
			{
				followingTarget = true;

				MoveByTransform mbp = GetComponentInParent<MoveByTransform>();
				mbp.Init(Target.GameObj.transform);
				_attacking.AddOrder(mbp);
			}
			else if (followingTarget && Physics2D.IsTouching(_collider, Target.Collider))
			{
				followingTarget = false;
				_attacking.AddOrder(this);
			}

			return true;
		}
		
		bool AcquireTarget()
		{
			ContactFilter2D filter = new ContactFilter2D();
			filter.layerMask = LayerMask;
			filter.useLayerMask = true;
			_targetsBuffer = new Collider2D[25];
			_targetsCount = Physics2D.OverlapCollider(_collider, filter, _targetsBuffer);
			if (_targetsCount > 0)
			{
				Target = DefineTarget();
				if (Target == null)
					return false;
				
				IsActive = true;
				_attacking.AddOrder(this);
				return true;
			}
			Target = null;
			return false;
		}

		ITargetable DefineTarget()
		{
			ITargetable bestTarget = null;
			float minDistance = float.MaxValue;
			for (int i = 0; i < _targetsCount; i++)
			{
				ITargetable t = _targetsBuffer[i].GetComponent<ITargetable>();
				if (t.IsDied)
					continue;
				
				float distance = Vector2.Distance(t.Position, _transform.position);
				if (distance < minDistance)
				{
					bestTarget = t;
					minDistance = distance;
				}
			}
			if (bestTarget != null)
				Debug.Log($"DefineTarget [{bestTarget}]");
			return bestTarget;
		}

		void Attack()
		{
			Target.Damage(AttackDamage, AttackType);
		}

		void EndAttacking()
		{
			_attacking.OrderEnded(this);
		}
		
#region IUnitOrder

		public void StartOrder()
		{
			IsActive = true;
			Debug.Log($"{gameObject} order started");
		}

		public void PauseOrder()
		{
			IsActive = false;
			Debug.Log($"{gameObject} order paused");
		}
		
#endregion
	}
}