using UnityEngine;

namespace TowerDefense
{
	public class TrapBarricade : MonoBehaviour, IAlive, ITargetable
	{
		Tower tower;
		Healthy healthy;
		CircleCollider2D collider;

		void Awake()
		{
			healthy = GetComponent<Healthy>();
			collider = GetComponent<CircleCollider2D>();
		}

		void OnEnable()
		{
			// healthy.DiedInstanceEvent += OnDied;
		}

		void OnDisable()
		{
			// healthy.DiedInstanceEvent -= OnDied;
		}

		protected void Corpse()
		{
			LeanTween.alpha(gameObject, 0f, 2f).setOnComplete(() => SimplePool.Despawn(gameObject));
		}

		#region IAlive
		public void RaiseDamagedEvent(int damage, DamageType type)
		{
			// AnyDamagedEvent?.Invoke(this, damage, type);
			// DamagedEvent?.Invoke(damage, type);
		}

		public void RaiseDiedEvent()
		{
			Corpse();
			// AnyDiedEvent?.Invoke(this);
			// DiedEvent?.Invoke(this);
		}
		#endregion

		#region ITargetable
		public void Damage(Weapon weapon)
		{
			healthy.Damage(weapon);
		}

		public void Damage(int damage, DamageType type = DamageType.Physical)
		{
			healthy.Damage(damage, type);
		}

		public GameObject GameObj => gameObject;
		public bool IsDied => healthy.IsDied;
		public Vector2 Position => transform.position;
		public float Health => healthy.CurrentHealth;
		public int MaxHealth => healthy.MaxHealth;
		public CircleCollider2D Collider => collider;
		public Vector2 PointToDamage => Position;

		#endregion
	}
}