using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

namespace TowerDefense
{
	public class Projectile : MonoBehaviour
	{
		public static event Action<Projectile> MissedEvent;
		
		protected Transform _transform;
		protected Weapon _weapon;
		protected Unit _target;
		
		[Header("Projectile")]
		public float TravelTime = 1f;

		protected virtual void Awake()
		{
			_transform = GetComponent<Transform>();
		}

		public virtual void Init(Weapon weapon)
		{
			_weapon = weapon;
			_target = weapon.Target;
			
			// StopAllCoroutines();
			// LeanTween.cancelAll(gameObject);
		}

		protected virtual void CheckHit()
		{
			if (_target.Collider.bounds.Contains(_transform.position))
			{
				_target.Damage(_weapon);
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