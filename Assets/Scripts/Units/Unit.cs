using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TowerDefense
{
	public class Unit : MonoBehaviour, IAlive, ITargetable, ICanAttack
	{
		public static event Action<Unit> DiedEvent;
		public event Action DiedInstanceEvent;
		public static event Action<Unit, int, DamageType> DamagedEvent;
		public static event Action<Unit> ArrivedDestinationEvent;
		public event Action ArrivedDestinationInstanceEvent;

		public event Action OrderEndedEvent;
		public event Action OrderChangedEvent;

		Transform _transform;
		Collider2D _collider;
		Healthy _healthy;
		MoveByPath _moveByPath;
		MoveByTransform _moveByTransform;
		MeleeAttack _meleeAttack;
		
		[Header("Unit")]
		public bool IsActive = false;
		[Space]
		public Transform RotationTransform;
		[Space]
		public LinkedList<IUnitOrder> Orders = new LinkedList<IUnitOrder>();
		public IUnitOrder CurrentOrder;
		Stack<IUnitOrder> startedOrders = new Stack<IUnitOrder>();

		protected virtual void Awake()
		{
			_transform = GetComponent<Transform>();
			_collider = GetComponent<Collider2D>();
			_healthy = GetComponent<Healthy>();
			_moveByPath = GetComponent<MoveByPath>();
			_moveByTransform = GetComponent<MoveByTransform>();
			_meleeAttack = GetComponentInChildren<MeleeAttack>();
		}

		public virtual void Init(string pathName = "")
		{
			IsActive = true;
			
			if (_healthy != null)
				_healthy.Init(this);
			
			ResetSprite();
		}

		public virtual void DeInit()
		{
			IsActive = false;
			StopAllCoroutines();
		}

		void LogOrders()
		{
			string s = "";
			foreach (IUnitOrder order in Orders)
			{
				if (CurrentOrder == order)
					s += ">>";
				s += order.OrderName();
				s += "\n";
			}
			
			Debug.Log($"{gameObject.name} orders: \n{s}");
		}

		public void AddOrder(IUnitOrder order, bool startImmidiate = true)
		{
			Debug.Log($"{gameObject.name} AddOrder [{order.OrderName()}]");
			
			if (!Orders.Contains(order))
				Orders.AddLast(order);

			if (startImmidiate)
			{
				if (CurrentOrder != null)
				{
					startedOrders.Push(CurrentOrder);
					CurrentOrder.Pause();
				}
				ChangeOrder(order);
			}

			LogOrders();
		}

		public void ChangeOrder(IUnitOrder order)
		{
			Debug.Log($"{gameObject.name} ChangeOrder [{order.OrderName()}]");
			CurrentOrder = order;
			order.Start();
			LogOrders();
		}

		public void OrderEnded(IUnitOrder order) // TODO: add events
		{
			if (!Orders.Contains(order))
				return;
			
			order.Pause();
			
			Debug.Log($"{gameObject.name} OrderEnded [{order.OrderName()}]");
			
			Orders.Remove(order);

			if (startedOrders.Count > 0)
			{
				IUnitOrder previousOrder = startedOrders.Pop();
				if (previousOrder is MoveByPath mbp)
				{
					mbp.AssignToClosestWaypoint();
				}
				ChangeOrder(previousOrder);
			}
			else
			{
				if (Orders.Count > 0)
					ChangeOrder(Orders.First.Value); // is it good to use first?
				else
					return;
			}

			LogOrders();
		}

		public virtual void ArrivedDestination()
		{
			ArrivedDestinationEvent?.Invoke(this);
			ArrivedDestinationInstanceEvent?.Invoke();
			StopMoving();
		}

		protected void StopMoving()
		{
			StopAllCoroutines();
//			animator.Play("Idle");
//			animator.speed = 1f;
		}
		
		void ResetSprite()
		{
//			animator.enabled = true;
//			animator.Play("Idle");
			LeanTween.alpha(gameObject, 1f, 0f);
		}

		public void RaiseDamagedEvent(int damage, DamageType type)
		{
			DamagedEvent?.Invoke(this, damage, type);
		}

		public void RaiseDiedEvent()
		{
			Corpse();
			DiedEvent?.Invoke(this);
			DiedInstanceEvent?.Invoke();
		}

		// used by Animator
		public void Corpse()
		{
			// animator.enabled = false;
			LeanTween.alpha(gameObject, 0f, 2f).setOnComplete(() => SimplePool.Despawn(gameObject));
		}

#region ITargetable

		public void Damage(Weapon weapon)
		{
			_healthy.Damage(weapon);
		}

		public void Damage(int damage, DamageType type = DamageType.Physical)
		{
			_healthy.Damage(damage, type);
		}

		public GameObject GameObj => gameObject;
		public bool IsDied => _healthy.IsDied;
		public Vector2 Position => _transform.position;
		public Vector2 Waypoint => _moveByPath.WaypointPoint;
		public int PathIndex => _moveByPath.PathIndex;
		public float Health => _healthy.CurrentHealth;
		public int MaxHealth => _healthy.MaxHealth;
		public Collider2D Collider => _collider;
		public Vector2 PointToDamage => Position;

#endregion
	}
}