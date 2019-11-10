using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TowerDefense
{
	public class Unit : MonoBehaviour, ITargetable
	{
		public static event Action<Unit> DiedEvent;
		public static event Action<Unit, Weapon> DamagedEvent;
		public static event Action<Unit> ArrivedDestinationEvent;
		public event Action ArrivedDestinationInstanceEvent;

		Transform trans;
		Collider2D coll;
		bool initialized = false;
	
		[Header("Unit")]
		public bool IsActive = false;
		public bool IsDead = false;
		[SerializeField] int maxHealth = 10;
		public int CurrentHealth = 10;
		[Range(0, 1f)] public float HealthPercent = 1f;
		public Armor ArmorType = Armor.None;
		public bool Damaged = false;
		public int FoodReward = 0;

		protected virtual void Awake()
		{
			trans = GetComponent<Transform>();
			coll = GetComponent<Collider2D>();
		}

		public virtual void Init(string pathName, bool isNew = true)
		{
			IsActive = true;
			IsDead = false;

			if (isNew && CurrentHealth < MaxHealth)
				CurrentHealth = MaxHealth;
			HealthPercent = (float) CurrentHealth / MaxHealth;
//			UpdateHealthBar();
			
			ResetSprite();

			initialized = true;
		}

		public virtual void DeInit()
		{
			IsActive = false;
			initialized = false;
			StopAllCoroutines();
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
			// LeanTween.alpha(rotationTrans.gameObject, 1f, 0f);
		}
		
		void Die(Weapon weapon)
		{
			DeInit();
			Corpse();
			IsDead = true;
			// Debug.Log(string.Format("# Mob [{0}] died", gameObject.name));

			// animator.Play("Dying");
			// animator.SetTrigger("Die");
			GetComponent<MoveByPath>().StopMoving();
			
			if (DiedEvent != null)
				DiedEvent(this);
		}

		// calls in Animator
		public void Corpse()
		{
			// animator.enabled = false;
			// LeanTween.alpha(rotationTrans.gameObject, 0f, 2f).setOnComplete(() => SimplePool.Despawn(gameObject));
		}
		
		public virtual void Damage(Weapon weapon)
		{
			Damage(weapon.Damage, weapon.DamageType);
			if (DamagedEvent != null)
				DamagedEvent(this, weapon); //TODO: move event to catch not only weapon damage
		}

		public virtual void Damage(float damage, DamageType type = DamageType.Physical)
		{
			if (IsDead)
				return;

			if (type == DamageType.Magical && ArmorType == Armor.Spiritual
			    || type == DamageType.Physical && ArmorType == Armor.Fortified)
				damage *= 0.25f;
			
			CurrentHealth = (int)(CurrentHealth - damage);
			HealthPercent = (float) CurrentHealth / MaxHealth;
//			UpdateHealthBar();
			if (!Damaged && CurrentHealth < MaxHealth)
			{
				Damaged = true;
//				healthBar.gameObject.SetActive(true);
			}
			if (CurrentHealth <= 0)
			{
				Die(null);
			}
		}

#region ITargetable
		public GameObject GameObj
		{
			get { return gameObject; }
		}

		public bool IsDied
		{
			get { return IsDead; }
		}

		public Vector2 WaypointPoint
		{
			get { return Vector2.zero; }
		}

		public int PathIndex
		{
			get { return 0; }
		}

		public float Health
		{
			get { return HealthPercent; }
		}

		public int MaxHealth
		{
			get { return maxHealth; }
		}

		public Collider2D Collider
		{
			get { return coll; }
		}

		public Vector2 Point
		{
			get { return trans.position; }
		}

		public Vector2 PointToDamage
		{
			get { return trans.position; }
		}
#endregion
	}
}