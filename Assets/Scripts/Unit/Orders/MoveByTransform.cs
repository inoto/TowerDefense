using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class MoveByTransform : Order
	{
		public float Speed = 40f;
		[SerializeField] [ReadOnly] bool isMoving;

		Transform destination;
		Vector2 desired, offset, footPoint;
		Quaternion quat;

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
		
		public void AssignTransform(Transform trans)
		{
			isMoving = false;
			destination = trans;
            isMoving = true;

			weapon.TargetOutOfRangeEvent += StopMoving;
        }
		
		void StartMoving()
		{
            weapon.TargetInRangeEvent -= StartMoving;

			isMoving = true;

            weapon.TargetOutOfRangeEvent += StopMoving;
		}
		
		public void StopMoving()
		{
            weapon.TargetOutOfRangeEvent -= StopMoving;

			isMoving = false;

            weapon.TargetInRangeEvent += StartMoving;
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
				if (distance < 0.1f)
				{
					ArrivedDestination();
					return;
				}
				_transform.position += (Vector3)desired * Time.fixedDeltaTime * Speed / 100;
			}
		}

		void ArrivedDestination()
		{
			isMoving = false;
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