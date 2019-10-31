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
		public static event Action<Unit, string> LookingForPathEvent;
		public static event Action<Unit> ArrivedDestinationEvent;
		public event Action ArrivedDestinationInstanceEvent;

		Transform trans;
		Collider2D coll;
		bool initialized = false;
		[HideInInspector] public Vector2[] Path = null;
		int pathIndex;
		Vector2 waypoint;
		Vector2 waypointOffset;
		Vector2 desired;

		[Header("Unit")]
		[SerializeField] Transform rotationTrans;
		public bool IsActive = false;
		public bool IsMoving = false;
		public bool IsDead = false;
		[SerializeField] int maxHealth = 10;
		public int CurrentHealth = 10;
		[Range(0, 1f)] public float HealthPercent = 1f;
		public Armor ArmorType = Armor.None;
		public bool Damaged = false;
		public float MoveSpeed = 30f;
		public int FoodReward = 0;
		public Vector2 FootPoint;

		protected virtual void Awake()
		{
			trans = GetComponent<Transform>();
			coll = GetComponent<Collider2D>();
		}

		void Start()
		{
			StartCoroutine(CheckCreatedManually());
		}
	
		IEnumerator CheckCreatedManually()
		{
			yield return new WaitForSeconds(2f);
				if (!IsActive)
			Init("Path0", false);
		}

		public virtual void Init(string pathName, bool isNew = true)
		{
			IsActive = true;
			IsDead = false;
			IsMoving = false;

			if (isNew && CurrentHealth < MaxHealth)
				CurrentHealth = MaxHealth;
			HealthPercent = (float) CurrentHealth / MaxHealth;
//			UpdateHealthBar();
			
			ResetSprite();

			pathIndex = 0;
			Path = null;

			initialized = true;
			
			if (LookingForPathEvent != null)
				LookingForPathEvent(this, pathName);
		}

		public void DeInit()
		{
			StopAllCoroutines();
			IsActive = false;
			IsMoving = false;
			initialized = false;
		}
		
		public void AssignPath(Vector2[] path)
		{
			if (path != null && path.Length > 0)
			{
				Path = path;
				StartCoroutine(MoveByPath());
			}
		}

		IEnumerator MoveByPath()
		{
			IsMoving = true;
//			animator.Play("Walking");
//			animator.speed = MoveSpeed / 100 * 3;
			waypoint = Path[pathIndex] - FootPoint;
			Quaternion quat;
			while (IsMoving)
			{
				desired = waypoint - (Vector2)trans.position;
				quat = rotationTrans.rotation;
				quat.y = desired.x < 0 ? 180f : 0f;
				rotationTrans.rotation = quat;
				float distance = desired.magnitude;
				desired.Normalize();
				if (distance < 0.1f)
				{
					if (pathIndex >= Path.Length - 1)
					{
						if (ArrivedDestinationEvent != null)
							ArrivedDestinationEvent(this);
						StopMoving();
						yield break;
					}
					pathIndex += 1;
					waypoint = Path[pathIndex] - FootPoint;
				}
				trans.position += (Vector3)desired * Time.fixedDeltaTime * MoveSpeed / 100;
				yield return null;
			}
		}

		protected IEnumerator MoveByPoint(Vector2 point)
		{
			IsMoving = true;
//			animator.Play("Walking");
//			animator.speed = MoveSpeed / 100 * 3;
			Quaternion quat;
			while (IsMoving)
			{
				desired = point - (Vector2) trans.position;
				quat = rotationTrans.rotation;
				quat.y = desired.x < 0 ? 180f : 0f;
				rotationTrans.rotation = quat;
				float distance = desired.magnitude;
				desired.Normalize();
				if (distance < 0.1f)
				{
					if (ArrivedDestinationEvent != null)
						ArrivedDestinationEvent(this);
					if (ArrivedDestinationInstanceEvent != null)
						ArrivedDestinationInstanceEvent();
					StopMoving();
					yield break;
				}

				trans.position += (Vector3) desired * Time.fixedDeltaTime * MoveSpeed / 100;
				yield return null;
			}
		}

		protected void StopMoving()
		{
			IsMoving = false;
			StopAllCoroutines();
//			animator.Play("Idle");
//			animator.speed = 1f;
		}
		
		void ResetSprite()
		{
//			animator.enabled = true;
//			animator.Play("Idle");
			LeanTween.alpha(rotationTrans.gameObject, 1f, 0f);
		}
		
		void Die(Weapon weapon)
		{
			DeInit();
			Corpse();
			IsDead = true;
			// Debug.Log(string.Format("# Mob [{0}] died", gameObject.name));

			// animator.Play("Dying");
			// animator.SetTrigger("Die");
			
			if (DiedEvent != null)
				DiedEvent(this);
		}

		// calls in Animator
		public void Corpse()
		{
			// animator.enabled = false;
			LeanTween.alpha(rotationTrans.gameObject, 0f, 2f).setDelay(1f).setOnComplete(() => SimplePool.Despawn(gameObject));
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

		void OnDrawGizmos()
		{
			Gizmos.DrawSphere(transform.position + (Vector3)FootPoint, 0.1f);
			if (IsMoving)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(trans.position, (Vector2)trans.position+desired);
			}

			if (Path != null && Path.Length > 0)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(transform.position, Path[pathIndex]);
				for (int i = pathIndex; i < Path.Length-1; i++)
				{
					Gizmos.DrawLine(Path[i], Path[i+1]);
				}
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
			get { return waypoint; }
		}

		public int PathIndex
		{
			get { return pathIndex; }
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