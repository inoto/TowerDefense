using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

namespace TowerDefense
{
	public class Projectile : MonoBehaviour
	{
		public static event Action<Projectile> MissedEvent;

		protected Weapon weapon;
		protected ITargetable target;
		
		protected Transform _transform;
		
		[Header("Projectile")]
		public float TravelTime = 1f;

		protected virtual void Awake()
		{
			_transform = GetComponent<Transform>();
		}

		public virtual void Init(Weapon weapon)
		{
			this.weapon = weapon;
			target = weapon.Target;
			
			StopAllCoroutines();
			LeanTween.cancel(gameObject);
		}

		protected virtual void CheckHit()
		{
			if (target.Collider.bounds.Contains(_transform.position))
			{
				target.Damage(weapon);
				SimplePool.Despawn(gameObject);
			}
			else
				LeanTween.delayedCall(1f, () => SimplePool.Despawn(gameObject));
		}

		protected void RaiseMissedEvent()
		{
			MissedEvent?.Invoke(this);
		}
	}
}