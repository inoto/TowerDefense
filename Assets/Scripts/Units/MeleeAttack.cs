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
		[ReadOnly] public Unit Target;
		[ProgressBar("LaunchProgress", 1f)]
		[SerializeField] float LaunchProgress;

		Transform _transform;
		Collider2D _collider;
		MoveByPath _moveByPath;
		Unit _unit;
		static Collider2D[] _targetsBuffer;
		int _targetsCount;

		void Awake()
		{
			_transform = GetComponent<Transform>();
			_collider = GetComponent<Collider2D>();
			_moveByPath = GetComponentInParent<MoveByPath>();
			_unit = GetComponentInParent<Unit>();
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

			if (!followingTarget && !Physics2D.IsTouching(_collider, Target.Collider))
			{
				followingTarget = true;

				MoveByTransform mbp = GetComponentInParent<MoveByTransform>();
				mbp.Init(Target.transform);
				_unit.AddOrder(mbp);
			}
			else if (followingTarget)
			{
				if (!Target.gameObject.activeSelf)
				{
					followingTarget = false;
					Target = null;
					_unit.OrderEnded(GetComponentInParent<MoveByTransform>());
					_unit.OrderEnded(this);
				}
				else if (Physics2D.IsTouching(_collider, Target.Collider))
				{
					followingTarget = false;
					_unit.AddOrder(this);
				}
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
				IsActive = true;
				_unit.AddOrder(this);
				return true;
			}
			Target = null;
			return false;
		}

		Unit DefineTarget()
		{
			Unit bestTarget = null;
			float minDistance = float.MaxValue;
			for (int i = 0; i < _targetsCount; i++)
			{
				float distance = Vector2.Distance(_targetsBuffer[i].transform.position, _transform.position);
				if (distance < minDistance)
				{
					bestTarget = _targetsBuffer[i].GetComponent<Unit>();
					minDistance = distance;
				}
			}
			Debug.Log($"DefineTarget [{bestTarget}]");
			return bestTarget;
		}

		void FollowTarget()
		{
			
		}

		void Attack()
		{
			Target.DiedInstanceEvent += TargetDied;
			Target.Damage(AttackDamage, AttackType);
		}

		void TargetDied()
		{
			Target.DiedInstanceEvent -= TargetDied;
			_unit.OrderEnded(this);
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