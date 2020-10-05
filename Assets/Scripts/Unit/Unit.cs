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
		public event Action<Unit> DiedEvent;
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
        public float InitialYRotation;

        Transform _transform;
		CircleCollider2D _collider;
		Weapon _weapon;
		public Weapon Weapon => _weapon;

		[Header("Components")]
		[SerializeField] protected Healthy _healthy;
        [SerializeField] protected MoveByTransform _moveByTransform;

        protected virtual void Awake()
		{
			_transform = GetComponent<Transform>();
			_collider = GetComponent<CircleCollider2D>();
			_weapon = GetComponentInChildren<Weapon>();

			InitialYRotation = RotationTransform.rotation.eulerAngles.y;
		}

		void OnEnable()
		{
			Reset();
			// Instances.Add(this);
        }

		void OnDisable()
		{
			IsActive = false;
			StopAllCoroutines();
			// Instances.Remove(this);
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
			// TODO: stop all orders
			this.Weapon.IsActive = false;
            Corpse();
			AnyDiedEvent?.Invoke(this);
			DiedEvent?.Invoke(this);
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
		public CircleCollider2D Collider => _collider;
		public Vector2 PointToDamage => Position;

#endregion
	}
}