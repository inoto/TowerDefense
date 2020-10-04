using System;
using System.IO;
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
		Vector2 offset, footPoint;
		Quaternion quat;
		float distanceToDestination;

		AttachmentPoints _attachments;
        Weapon weapon;
        MoveByPath moveByPath;
        Soldier soldier;
		
		protected override void Awake()
		{
			base.Awake();

			_attachments = GetComponent<AttachmentPoints>();
			_attachments.Points.TryGetValue("Foot", out footPoint);
            weapon = GetComponentInChildren<Weapon>();
            moveByPath = GetComponent<MoveByPath>();
			if (_unit is Soldier unit)
				soldier = unit;
		}
		
		public void AssignTransform(Transform trans, float distanceToDestination = CommonDistanceToDestination)
		{
			destination = trans;
            isMoving = true;
            this.distanceToDestination = distanceToDestination;

			weapon.TargetOutOfRangeEvent += StartMoving;
        }

		public void StopMoving()
		{
			isMoving = false;
		}
		
		void StartMoving(Weapon weapon1)
		{
			weapon.TargetOutOfRangeEvent -= StartMoving;

			isMoving = true;

            weapon.TargetNowInRangeEvent += StopMoving;
		}
		
		void StopMoving(Weapon weapon1)
		{
            weapon.TargetNowInRangeEvent -= StopMoving;

			isMoving = false;

            weapon.TargetOutOfRangeEvent += StartMoving;
        }

		void Update()
		{
			if (IsActive && isMoving)
			{
				_unit.Velocity = destination.position - _transform.position - (Vector3)footPoint;

				if (soldier != null)
				{
					Flock();
				}

				quat = _unit.RotationTransform.rotation;
				quat.y = _unit.Velocity.x < 0 ? 180f : 0f;
				_unit.RotationTransform.rotation = quat;
				
				if (_unit.Collider.bounds.Contains(destination.position))
				{
					ArrivedDestination();
					Debug.Log($"{gameObject.name} ArrivedDestination by collider");
					return;
				}
				float distance = _unit.Velocity.magnitude;
				if (distance < distanceToDestination)
				{
					ArrivedDestination();
					Debug.Log($"{gameObject.name} ArrivedDestination by distance");
					return;
				}
				_transform.position += (Vector3)_unit.Velocity * Time.fixedDeltaTime * Speed / 100;
			}
		}

		void ArrivedDestination()
		{
			_unit.ArrivedDestination();
		}

		Vector2 Seek(Vector2 destination)
		{
			Vector2 desired = destination - (Vector2)soldier.transform.position - footPoint;
			desired.Normalize();
			return desired - _unit.Velocity;
		}

		void Flock()
		{
			Vector2 sep = Separate();
			Vector2 ali = Align();
			Vector2 coh = Cohesion();

			sep *= 1f;
			ali *= 1f;
			coh *= 1f;

			_unit.Velocity += sep + ali + coh;
			_unit.Velocity.Normalize();
			// _unit.Velocity = Vector2.ClampMagnitude(_unit.Velocity, 1f);
		}

		Vector2 Separate()
		{
			Vector2 sum = Vector2.zero;
			int count = 0;
			foreach (var other in Soldier.Instances)
			{
				float dist = Vector2.Distance(soldier.transform.position, other.transform.position);
				if (dist > 0 && dist < soldier.CircleCollider.radius)
				{
					Vector2 diff = soldier.transform.position - other.transform.position;
					diff.Normalize();
					diff /= dist;
					sum += diff;
					count++;
				}
			}
			if (count > 0)
			{
				sum /= count;
				sum.Normalize();
				return sum - _unit.Velocity;
			}
			else
				return Vector2.zero;
		}

		Vector2 Align()
		{
			float neighborDist = 10f;
			Vector2 sum = Vector2.zero;
			int count = 0;
			foreach (var other in Soldier.Instances)
			{
				float dist = Vector2.Distance(soldier.transform.position, other.transform.position);
				if (dist > 0 && dist < neighborDist)
				{
					sum += other.Velocity;
					count++;
				}
			}
			if (count > 0)
			{
				sum /= count;
				sum.Normalize();
				return sum - _unit.Velocity;
			}
			else
				return Vector2.zero;
		}

		Vector2 Cohesion()
		{
			float neighborDist = 10f;
			Vector2 sum = Vector2.zero;
			int count = 0;
			foreach (var other in Soldier.Instances)
			{
				float dist = Vector2.Distance(soldier.transform.position, other.transform.position);
				if (dist > 0 && dist < neighborDist)
				{
					sum += other.Velocity;
					count++;
				}
			}
			if (count > 0)
			{
				sum /= count;
				return Seek(sum);
			}
			else
				return Vector2.zero;
		}

		void OnDrawGizmos()
		{
			if (IsActive && isMoving && destination != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, destination.position);
				Gizmos.color = Color.blue;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, (Vector2)_transform.position + _unit.Velocity);
			}
		}
    }
}