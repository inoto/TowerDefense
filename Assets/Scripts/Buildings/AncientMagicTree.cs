using System;
using UnityEngine;

namespace TowerDefense
{
    public class AncientMagicTree : MonoBehaviour, IAlive, ITargetable
    {
        public static event Action DiedEvent;

		Transform _transform;
        Collider2D _collider;
        Healthy _healthy;

        void Awake()
        {
            _transform = GetComponent<Transform>();
            _collider = GetComponent<Collider2D>();
            _healthy = GetComponent<Healthy>();
        }

        void Corpse()
        {
            DiedEvent?.Invoke();
        }

#region IAlive

        public void RaiseDamagedEvent(int damage, DamageType type)
        {
            // AnyDamagedEvent?.Invoke(this, damage, type);
        }

        public void RaiseDiedEvent()
        {
            Corpse();
            // AnyDiedEvent?.Invoke(this);
            // DiedEvent?.Invoke();
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