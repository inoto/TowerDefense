using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TowerDefense
{
	public class ProjectileArrow : Projectile
	{
		[Header("ProjectileArrow")]
		[SerializeField] float SpeedMultiplierMin = 0.9f;
		[SerializeField] float SpeedMultiplierMax = 1.1f;
		[SerializeField] float HeightMultiplierMin = 0.9f;
		[SerializeField] float HeightMultiplierMax = 1.1f;
		[SerializeField] AnimationCurve Curve;
		
		Vector3 newPoint, startPoint, endPoint;
		float time = 0f, duration = 1f;
		float height, multipliedSpeed, heightMultiplier;
		float angleRad, angleDeg;
		Vector2 direction;

		public override void Init(Weapon weapon)
		{
			base.Init(weapon);

			if (target == null || target.IsDied)
			{
				SimplePool.Despawn(gameObject);
				return;
			}
			
			startPoint = _transform.position;
			endPoint = target.Position;
			time = 0f;

			StartCoroutine(MoveByArc());
		}

		IEnumerator MoveByArc()
		{
			multipliedSpeed = TravelTime * Random.Range(SpeedMultiplierMin, SpeedMultiplierMax);
			heightMultiplier = Random.Range(HeightMultiplierMin, HeightMultiplierMax);
			duration = Vector2.Distance(startPoint, endPoint); // duration is a distance for now
			if (duration < 1f) duration = 1f; // clamp

			while (time < duration)
			{
				if (!target.IsDied)
				{
					endPoint = target.Position;
					direction = _transform.position - startPoint;
				}
				
				time += Time.fixedDeltaTime * 1/multipliedSpeed;
				height = Curve.Evaluate(time/duration);

				newPoint = Vector2.Lerp(startPoint, endPoint, time/duration) + new Vector2(0f, height);

				if (time < duration * 0.99)
				{
					direction = newPoint - _transform.position;
					angleRad = Mathf.Atan2(direction.y, direction.x);
					angleDeg = (180 / Mathf.PI) * angleRad;
				
					_transform.rotation = Quaternion.AngleAxis(angleDeg, Vector3.forward);
				}

				_transform.position = newPoint;
 
				yield return new WaitForSeconds(0.01f);
			}

			CheckHit();
		}

		protected override void CheckHit()
		{
			if (target.Collider.bounds.Contains(_transform.position))
			{
				target.Damage(weapon);
				SimplePool.Despawn(gameObject);
			}
			else
			{
				RaiseMissedEvent();
				
				LeanTween.alpha(gameObject, 0f, 1f).setOnComplete(() => SimplePool.Despawn(gameObject));
			}
		}
	}
}