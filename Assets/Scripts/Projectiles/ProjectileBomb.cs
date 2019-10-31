using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class ProjectileBomb : Projectile
	{
		[Header("ProjectileBomb")]
		[SerializeField] float SpeedMultiplierMin = 0.9f;
		[SerializeField] float SpeedMultiplierMax = 1.1f;
		[SerializeField] GameObject ExplosionPrefab;
		[SerializeField] float DamageDelay = 0.1f;
		[SerializeField] AnimationCurve Curve;

		Vector3 _newPoint, _startPoint, _endPoint;
		float _time = 0f, _duration = 1f;
		float _height, _multipliedSpeed;

		public override void Init(Weapon weapon)
		{
			base.Init(weapon);
			
			_startPoint = _transform.position;
			_endPoint = target.Point;

			StartCoroutine(MoveByArc());
		}
		
		IEnumerator MoveByArc()
		{
			// float angleRad, angleDeg;
			// Vector2 direction;

			_multipliedSpeed = TravelTime * Random.Range(SpeedMultiplierMin, SpeedMultiplierMax);
			_duration = Vector2.Distance(_startPoint, _endPoint); // duration is a distance for now
			if (_duration < 1f) _duration = 1f; // clamp

			while (_time < _duration)
			{
				_time += Time.fixedDeltaTime * 1/_multipliedSpeed;
				_height = Curve.Evaluate(_time/_duration);

				_newPoint = Vector2.Lerp(_startPoint, _endPoint, _time/_duration) + new Vector2(0f, _height);

				// if (time < duration * 0.99)
				// {
				// 	direction = _transform.position - startPoint;
				// 	angleRad = Mathf.Atan2(direction.y, direction.x);
				// 	angleDeg = (180 / Mathf.PI) * angleRad;
				//
				// 	_transform.rotation = Quaternion.AngleAxis(angleDeg, Vector3.forward);
				// }

				transform.position = _newPoint;
 
				yield return new WaitForSeconds(0.01f);
			}

			MakeExplosion();
		}

		void MakeExplosion()
		{
			GameObject go = Instantiate(ExplosionPrefab, _transform.position, Quaternion.identity);

			WeaponCannon wc = _weapon as WeaponCannon;

			Collider2D[] colliders = new Collider2D[25];
			// TODO: could be replaced with ellipse collider
			// damage in center
			int count = Physics2D.OverlapCircleNonAlloc(_transform.position, wc.FullDamageRange, colliders);
			for (int i = 0; i < count; i++)
			{
				ITargetable t = colliders[i].GetComponent<ITargetable>();
				if (t != null)
					LeanTween.delayedCall(DamageDelay, () => t.Damage(wc.Damage));
				colliders[i] = null;
			}
			// damage in full range
			count = Physics2D.OverlapCircleNonAlloc(_transform.position, wc.SplashDamageRange, colliders);
			for (int i = 0; i < count; i++)
			{
				ITargetable t = colliders[i].GetComponent<ITargetable>();
				if (t != null)
					LeanTween.delayedCall(DamageDelay, () => t.Damage(wc.Damage * wc.SplashDamageFactor));
			}
			
			Destroy(gameObject);
		}
		
		// void OnDrawGizmos()
		// {
		// 	if (target != null && !target.IsDied)
		// 	{
		// 		Gizmos.color = Color.red;
		// 		float dist = Vector2.Distance(_transform.position, endPoint);
		// 		Vector2 prev = startPoint, next;
		// 		for (float i = 0f; i < dist; i += dist/10)
		// 		{
		// 			float height = curve.Evaluate(i);
		// 			next = Vector2.Lerp(startPoint, endPoint, i) + new Vector2(0f, height);
		// 			Gizmos.DrawLine(prev, next);
		// 			prev = next;
		// 		}
		// 	}
		// }
	}
}