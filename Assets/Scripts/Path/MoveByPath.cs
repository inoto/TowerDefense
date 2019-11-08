using System;
using UnityEngine;
using NaughtyAttributes;

namespace TowerDefense
{
	public class MoveByPath : MonoBehaviour
	{
		public static event Action<MoveByPath, string> LookingForPathEvent;
		
		[ReadOnly] public string PathName;
		public float Speed = 40f;
		[SerializeField] Transform RotationTransform;
		[SerializeField] [ReadOnly] BezierCurve _path;
		[SerializeField] [ReadOnly] bool _isMoving;

		Transform _transform;
		int segment = 0;
		Vector2 waypoint;
		Vector2 desired;
		Quaternion quat;

		void Awake()
		{
			_transform = transform;
		}

		public void Init(string pathName)
		{
			segment = 0;
			_isMoving = false;
			LookingForPathEvent?.Invoke(this, pathName);
		}

		public void AssignPath(BezierCurve path)
		{
			_path = path;
			LeanTween.delayedCall(0.1f, StartMoving);
		}

		void StartMoving()
		{
			waypoint = _path.Anchors[segment];
			_isMoving = true;
		}

		void Update()
		{
			if (_isMoving)
			{
				desired = waypoint - (Vector2)_transform.position;
				quat = RotationTransform.rotation;
				quat.y = desired.x < 0 ? 180f : 0f;
				RotationTransform.rotation = quat;
				float distance = desired.magnitude;
				desired.Normalize();
				if (distance < 0.1f)
				{
					if (segment >= _path.Anchors.Length - 1)
					{
						ArrivedDestination();
						return;
					}
					segment += 1;
					waypoint = _path.Anchors[segment];// - FootPoint;
				}
				_transform.position += (Vector3)desired * Time.fixedDeltaTime * Speed / 100;
			}
		}

		void ArrivedDestination()
		{
			_isMoving = false;
		}
	}
}