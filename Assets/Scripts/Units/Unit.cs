using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TowerDefense
{
	public class Unit : MonoBehaviour
	{
		public static event Action<Unit> DiedEvent;
		public event Action DiedInstanceEvent;
		public static event Action<Unit, int, DamageType> DamagedEvent;
		public static event Action<Unit> ArrivedDestinationEvent;
		public event Action ArrivedDestinationInstanceEvent;

		Transform _transform;
		public Vector2 Position => _transform.position;
		Collider2D _collider;
		public Collider2D Collider => _collider;
		
		[Header("Unit")]
		public bool IsActive = false;
		[Space]
		public Transform RotationTransform;
		[Space]
		[SerializeField] HealthBar HealthBar;
		public bool Damageable = true;
		public int MaxHealth = 10;
		public int CurrentHealth = 10;
		[Range(0, 1f)] public float HealthPercent = 1f;
		public Armor ArmorType = Armor.None;
		public bool Damaged = false;
		public bool IsDead = false;

		public LinkedList<IUnitOrder> Orders = new LinkedList<IUnitOrder>();
		public IUnitOrder CurrentOrder;

		protected virtual void Awake()
		{
			_transform = GetComponent<Transform>();
			_collider = GetComponent<Collider2D>();
		}

		public virtual void Init(string pathName, bool isNew = true)
		{
			IsActive = true;
			IsDead = false;

			ResetHealth();
			ResetSprite();
		}

		public virtual void DeInit()
		{
			IsActive = false;
			StopAllCoroutines();
		}

		public void AddOrder(IUnitOrder order, bool runImmediate = true)
		{
			Debug.Log($"AddOrder [{order}], {runImmediate}");
			if (!Orders.Contains(order))
				Orders.AddLast(order);
			if (runImmediate)
			{
				if (CurrentOrder != null)
				{
					CurrentOrder.PauseOrder();
				}
				ChangeOrder(order);
			}
		}

		public void ChangeOrder(IUnitOrder order)
		{
			Debug.Log($"ChangeOrder [{order}]");
			CurrentOrder = order;
			order.StartOrder();
		}

		public void OrderEnded(IUnitOrder order) // TODO: rewrite to events
		{
			if (!Orders.Contains(order))
				return;
			
			order.PauseOrder();
			
			Debug.Log($"OrderEnded [{order}]");
			Orders.Remove(order);
			if (Orders.Count > 0)
				ChangeOrder(Orders.First.Value);
		}

		void ResetHealth()
		{
			if (CurrentHealth < MaxHealth)
				CurrentHealth = MaxHealth;
			HealthPercent = (float) CurrentHealth / MaxHealth;
			UpdateHealthBar();
			
			Damaged = false;
		}
		
		void UpdateHealthBar()
		{
			if (HealthBar != null)
			{
				HealthBar.SetPercent(HealthPercent);
				if (HealthPercent >= 1f || HealthPercent <= 0f)
					HealthBar.gameObject.SetActive(false);
				else
					HealthBar.gameObject.SetActive(true);
			}
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
		
		void Die(Weapon weapon)
		{
			DeInit();
			Corpse();
			IsDead = true;
			// Debug.Log(string.Format("# Mob [{0}] died", gameObject.name));

			// animator.Play("Dying");
			// animator.SetTrigger("Die");
			GetComponent<MoveByPath>()?.StopMoving();

			DiedEvent?.Invoke(this);
			DiedInstanceEvent?.Invoke();
		}

		// used by Animator
		public void Corpse()
		{
			// animator.enabled = false;
			LeanTween.alpha(gameObject, 0f, 2f).setOnComplete(() => SimplePool.Despawn(gameObject));
		}
		
		public virtual void Damage(Weapon weapon)
		{
			if (!Damageable)
				return;
			
			Damage(weapon.Damage, weapon.DamageType);
		}

		public virtual void Damage(int damage, DamageType type = DamageType.Physical)
		{
			if (IsDead || !Damageable)
				return;

			if (type == DamageType.Magical && ArmorType == Armor.Spiritual
			    || type == DamageType.Physical && ArmorType == Armor.Fortified)
				damage = Mathf.RoundToInt(damage * 0.25f);
			
			CurrentHealth = (int)(CurrentHealth - damage);
			HealthPercent = (float) CurrentHealth / MaxHealth;
			UpdateHealthBar();
			if (!Damaged && CurrentHealth < MaxHealth)
			{
				Damaged = true;
			}
			if (CurrentHealth <= 0)
			{
				Die(null);
			}
			
			DamagedEvent?.Invoke(this, damage, type);
		}
	}
}