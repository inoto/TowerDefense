using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace TowerDefense
{
	public class Unit : MonoBehaviour, IAlive, ITargetable
	{
		public static event Action<Unit> DiedEvent;
		public event Action DiedInstanceEvent;
		public static event Action<Unit, int, DamageType> DamagedEvent;
		public static event Action<Unit> ArrivedDestinationEvent;
		public event Action ArrivedDestinationInstanceEvent;
		public event Action OrderEndedEvent;
		public event Action OrderChangedEvent;
		
		[Header("Unit")]
		public bool IsActive = false;
		[Space]
		public Transform RotationTransform;
		[Space]
		public Queue<Order> NotStartedOrders = new Queue<Order>();
		public Order CurrentOrder;
		
		Stack<Order> startedOrders = new Stack<Order>();
		
		Transform _transform;
		Collider2D _collider;

		[Header("Orders")]
		[SerializeField] Healthy _healthy;
        [SerializeField] MoveByTransform _moveByTransform;

        protected virtual void Awake()
		{
			_transform = GetComponent<Transform>();
			_collider = GetComponent<Collider2D>();
        }

   //      void Start()
   //      {
			// Reset();
   //      }

		void OnEnable()
		{
            Reset();
        }

		void OnDisable()
		{
			IsActive = false;
			StopAllCoroutines();
		}

        void Reset()
        {
            IsActive = true;

			if (_healthy != null)
                _healthy.Init(this);

            ResetSprite();
		}

		void LogOrders()
		{
			StringBuilder sb = new StringBuilder($"{gameObject.name} orders: \n");
			foreach (Order order in NotStartedOrders)
			{
				if (CurrentOrder == order)
					sb.Append(">>");
				sb.Append(order.GetType().Name);
				sb.Append("\n");
			}
			
			if (NotStartedOrders.Count > 0)
			    Debug.Log(sb.ToString());
		}

		public virtual void AddOrder(Order order, bool startImmediate = true)
		{
			Debug.Log($"{gameObject.name} AddOrder [{order.GetType().Name}]");
            
			if (startImmediate)
			{
				if (CurrentOrder != null)
				{
					startedOrders.Push(CurrentOrder);
					CurrentOrder.Pause();
				}
				ActivateOrder(order);
			}
            else
            {
                if (!NotStartedOrders.Contains(order))
                    NotStartedOrders.Enqueue(order);
				LogOrders();
            }
        }

		void ActivateOrder(Order order)
		{
			Debug.Log($"{gameObject.name} ActivateOrder [{order.GetType().Name}]");
			CurrentOrder = order;
			order.Activate();
			// LogOrders();
		}

		public virtual void OrderEnded(Order order) // TODO: add events
		{
			// if (!NotStartedOrders.Contains(order))
			// 	return;
			
			order.Pause();
            CurrentOrder = null;
			
			Debug.Log($"{gameObject.name} OrderEnded [{order.GetType().Name}]");

			if (startedOrders.Count > 0)
			{
                Order previousOrder = startedOrders.Pop();
				if (previousOrder is MoveByPath mbp)
					mbp.AssignToClosestWaypoint();

				ActivateOrder(previousOrder);
			}
			else
			{
                if (NotStartedOrders.Count > 0)
                {
                    Order nextOrder = NotStartedOrders.Dequeue();
                    if (nextOrder is MoveByPath mbp)
                        mbp.AssignToClosestWaypoint();

					ActivateOrder(nextOrder); // is it good to use first?
                }
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
		void Corpse()
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
		// public Vector2 Waypoint => _moveByPath.WaypointPoint;
		// public int PathIndex => _moveByPath.PathIndex;
		public float Health => _healthy.CurrentHealth;
		public int MaxHealth => _healthy.MaxHealth;
		public Collider2D Collider => _collider;
		public Vector2 PointToDamage => Position;

#endregion
	}
}