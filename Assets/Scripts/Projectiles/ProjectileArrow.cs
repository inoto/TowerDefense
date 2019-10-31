using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
		
		Vector3 _newPoint, _startPoint, _endPoint;
		float _time = 0f, _duration = 1f;
		float _height, _multipliedSpeed, _heightMultiplier;
		float _angleRad, _angleDeg;
		Vector2 _direction;

		public override void Init(Weapon weapon)
		{
			base.Init(weapon);
			
			_startPoint = _transform.position;
			_endPoint = target.Point;

			StartCoroutine(MoveByArc());
		}
		
		IEnumerator MoveByArc()
		{
			_multipliedSpeed = TravelTime * Random.Range(SpeedMultiplierMin, SpeedMultiplierMax);
			_heightMultiplier = Random.Range(HeightMultiplierMin, HeightMultiplierMax);
			_duration = Vector2.Distance(_startPoint, _endPoint); // duration is a distance for now
			if (_duration < 1f) _duration = 1f; // clamp

			while (_time < _duration)
			{
				if (!target.IsDied)
				{
					_endPoint = target.Point;
					_direction = _transform.position - _startPoint;
				}
				
				_time += Time.fixedDeltaTime * 1/_multipliedSpeed;
				_height = Curve.Evaluate(_time/_duration);

				_newPoint = Vector2.Lerp(_startPoint, _endPoint, _time/_duration) + new Vector2(0f, _height);

				if (_time < _duration * 0.99)
				{
					_direction = _newPoint - _transform.position;
					_angleRad = Mathf.Atan2(_direction.y, _direction.x);
					_angleDeg = (180 / Mathf.PI) * _angleRad;
				
					_transform.rotation = Quaternion.AngleAxis(_angleDeg, Vector3.forward);
				}

				transform.position = _newPoint;
 
				yield return new WaitForSeconds(0.01f);
			}

			CheckHit();
		}

		protected override void CheckHit()
		{
			if (target.Collider.bounds.Contains(_transform.position))
			{
				target.Damage(_weapon);
				Destroy(gameObject);
			}
			else
			{
				RaiseMissedEvent();
				
				LeanTween.alpha(gameObject, 0f, 1f).setOnComplete(() => Destroy(gameObject));
			}
		}
	}
}