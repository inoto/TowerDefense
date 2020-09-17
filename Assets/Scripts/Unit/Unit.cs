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
		public static event Action<Unit> AnyDiedEvent;
		public event Action DiedEvent;
		public static event Action<Unit, int, DamageType> AnyDamagedEvent;
        public event Action<int, DamageType> DamagedEvent;
		public static event Action<Unit> AnyArrivedDestinationEvent;
		public event Action ArrivedDestinationEvent;
		public event Action OrderEndedEvent;
		public event Action OrderChangedEvent;
		
		[Header("Unit")]
		public bool IsActive = false;

        [Space]
        public Transform RotationTransform;
        public float SpriteYSpeed = 1f;
        public float SpriteMaxY = 0.03f;

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

		void Start()
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
			StopMoving();
			AnyArrivedDestinationEvent?.Invoke(this);
			ArrivedDestinationEvent?.Invoke();
		}

		protected void StopMoving()
		{
			_moveByTransform.StopMoving();
        }
		
		void ResetSprite()
		{
            LeanTween.alpha(gameObject, 1f, 0f);
		}

		public void RaiseDamagedEvent(int damage, DamageType type)
		{
			AnyDamagedEvent?.Invoke(this, damage, type);
			DamagedEvent?.Invoke(damage, type);
		}

		public void RaiseDiedEvent()
        {
            Corpse();
			AnyDiedEvent?.Invoke(this);
			DiedEvent?.Invoke();
		}

		// using by Animator
		protected virtual void Corpse()
		{
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