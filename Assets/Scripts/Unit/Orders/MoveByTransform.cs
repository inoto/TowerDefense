using System;
using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class MoveByTransform : Order
	{
		const float CommonDistanceToDestination = 0.1f;

		public float Speed = 40f;
		[SerializeField] [ReadOnly] bool isMoving;

		Transform destination;
		Vector2 desired, offset, footPoint;
		Quaternion quat;
		float distanceToDestination;

		AttachmentPoints _attachments;
        Weapon weapon;
        MoveByPath moveByPath;
		
		protected override void Awake()
		{
			base.Awake();

			_attachments = GetComponent<AttachmentPoints>();
			_attachments.Points.TryGetValue("Foot", out footPoint);
            weapon = GetComponentInChildren<Weapon>();
            moveByPath = GetComponent<MoveByPath>();
        }
		
		public void AssignTransform(Transform trans, float distance = CommonDistanceToDestination)
		{
			destination = trans;
            isMoving = true;
            distanceToDestination = distance;

			weapon.TargetOutOfRangeEvent += StartMoving;
        }
		
		void StartMoving()
		{
			weapon.TargetOutOfRangeEvent -= StartMoving;

			isMoving = true;

            weapon.TargetNowInRangeEvent += StopMoving;
		}
		
		public void StopMoving()
		{
            weapon.TargetNowInRangeEvent -= StopMoving;

			isMoving = false;

            weapon.TargetOutOfRangeEvent += StartMoving;
        }

		void Update()
		{
			if (IsActive && isMoving)
			{
				desired = destination.position - _transform.position - (Vector3)footPoint;
				
				quat = _unit.RotationTransform.rotation;
				quat.y = desired.x < 0 ? 180f : 0f;
				_unit.RotationTransform.rotation = quat;
				
				float distance = desired.magnitude;
				desired.Normalize();
				if (distance < distanceToDestination)
				{
					ArrivedDestination();
					return;
				}
				_transform.position += (Vector3)desired * Time.fixedDeltaTime * Speed / 100;
			}
		}

		void ArrivedDestination()
		{
			_unit.ArrivedDestination();
		}

		void OnDrawGizmos()
		{
			if (IsActive && isMoving && destination != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, destination.position);
				Gizmos.color = Color.blue;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, (Vector2)_transform.position+desired);
			}
		}
    }
}