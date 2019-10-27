using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Projectile : MonoBehaviour
	{
		public static event Action<Projectile> MissedEvent;
		
		protected Transform trans;
		protected Rigidbody2D rigid;
		protected Weapon weapon;
		protected ITargetable target;
		protected bool isActive;
		
		[Header("Projectile")]
		public float Speed = 1;

		protected virtual void Awake()
		{
			trans = GetComponent<Transform>();
			rigid = GetComponent<Rigidbody2D>();
		}

		public virtual void Init(Weapon weapon)
		{
			this.weapon = weapon;
			target = weapon.Target;
		}

		protected virtual void CheckHit()
		{
			if (target.Collider.bounds.Contains(trans.position))
			{
				target.Damage(weapon);
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