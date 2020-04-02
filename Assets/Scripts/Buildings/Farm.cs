using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class Farm : MonoBehaviour, IAlive, ITargetable
	{
		public static event Action<Farm, int> FoodProvidedEvent;

		public bool IsActive;
		[BoxGroup("Income")]
		[SerializeField] float Interval = 2f;
		[BoxGroup("Income")]
		[SerializeField] int Amount;

		Transform _transform;
		Collider2D _collider;
		Healthy _healthy;

		void Awake()
		{
			_transform = GetComponent<Transform>();
			_collider = GetComponent<Collider2D>();
			_healthy = GetComponent<Healthy>();
		}

		void Start()
		{
			Init();
			GetComponent<Healthy>().Init(this);
		}

		void Init()
		{
			StartCoroutine(ProvideFood());
		}

		IEnumerator ProvideFood()
		{
			IsActive = true;
			while (IsActive)
			{
				yield return new WaitForSeconds(Interval);

				FoodProvidedEvent?.Invoke(this, Amount);
			}
			
			yield return null;
		}

		public void StopProvideFood()
		{
			IsActive = false;
			StopAllCoroutines();
		}
		
		void Corpse()
		{
			LeanTween.alpha(gameObject, 0f, 2f).setOnComplete(() => Destroy(gameObject));
		}
		
#region IAlive

		public void RaiseDamagedEvent(int damage, DamageType type)
		{
			// DamagedEvent?.Invoke(this, damage, type);
		}

		public void RaiseDiedEvent()
		{
			Corpse();
			// DiedEvent?.Invoke(this);
			// DiedInstanceEvent?.Invoke();
		}
		
#endregion
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
		public Vector2 Waypoint => Vector2.zero;
		public int PathIndex => 0;
		public float Health => _healthy.CurrentHealth;
		public int MaxHealth => _healthy.MaxHealth;
		public Collider2D Collider => _collider;
		public Vector2 PointToDamage => Position;

#endregion
	}
}