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
		
		Vector3 _newPoint, _startPoint, _endPoint;
		float _time = 0f, _duration = 1f;
		float _height, _multipliedSpeed, _heightMultiplier;
		float _angleRad, _angleDeg;
		Vector2 _direction;

		public override void Init(Weapon weapon)
		{
			base.Init(weapon);

			if (_target == null || _target.IsDead)
			{
				SimplePool.Despawn(gameObject);
				return;
			}
			
			_startPoint = _transform.position;
			_endPoint = _target.Position;
			_time = 0f;

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
				if (!_target.IsDead)
				{
					_endPoint = _target.Position;
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

				_transform.position = _newPoint;
 
				yield return new WaitForSeconds(0.01f);
			}

			CheckHit();
		}

		protected override void CheckHit()
		{
			if (_target.Collider.bounds.Contains(_transform.position))
			{
				_target.Damage(_weapon);
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