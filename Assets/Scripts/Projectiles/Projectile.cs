using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Projectile : MonoBehaviour
	{
		public static event Action<Projectile> MissedEvent;
		
		protected Transform _transform;
		protected Weapon _weapon;
		protected ITargetable target;
		
		[Header("Projectile")]
		public float Speed = 1f;

		protected virtual void Awake()
		{
			_transform = GetComponent<Transform>();
		}

		public virtual void Init(Weapon weapon)
		{
			this._weapon = weapon;
			target = weapon.Target;
		}

		protected virtual void CheckHit()
		{
			if (target.Collider.bounds.Contains(_transform.position))
			{
				target.Damage(_weapon);
				Destroy(gameObject);
			}
			else
				LeanTween.delayedCall(2f, () => Destroy(gameObject));
		}

		protected void RaiseMissedEvent()
		{
			if (MissedEvent != null)
				MissedEvent(this);
		}
	}
}