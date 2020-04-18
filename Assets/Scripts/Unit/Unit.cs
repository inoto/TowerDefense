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
        public event Action<int, DamageType> DamagedInstanceEvent;
		public static event Action<Unit> ArrivedDestinationEvent;
		public event Action ArrivedDestinationInstanceEvent;
		public event Action OrderEndedEvent;
		public event Action OrderChangedEvent;
		
		[Header("Unit")]
		public bool IsActive = false;

        [Space]
        public Transform RotationTransform;

        Transform _transform;
		Collider2D _collider;

		[Header("Orders")]
		[SerializeField] protected Healthy _healthy;
        [SerializeField] protected MoveByTransform _moveByTransform;

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

            ResetSprite();
		}

        public virtual void ArrivedDestination()
		{
			ArrivedDestinationEvent?.Invoke(this);
			ArrivedDestinationInstanceEvent?.Invoke();
			StopMoving();
		}

		protected void StopMoving()
		{
			_moveByTransform.StopMoving();
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
			DamagedInstanceEvent?.Invoke(damage, type);
		}

		public void RaiseDiedEvent()
        {
            Corpse();
			DiedEvent?.Invoke(this);
			DiedInstanceEvent?.Invoke();
		}

		// used by Animator
		protected virtual void Corpse()
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