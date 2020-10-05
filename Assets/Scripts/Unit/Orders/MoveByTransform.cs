using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class MoveByTransform : Order
	{
		[System.Serializable]
		public class SteeringSettings
		{
			public bool Seek = true;
			public float SeekWeight = 1f;

			public bool Separate = true;
			public float SeparateWeight = 1f;

			public bool Align = true;
			public float AlignWeight = 1f;

			public bool Cohesion = true;
			public float CohesionWeight = 1f;
		}

		const float CommonDistanceToDestination = 0.1f;

		public static List<MoveByTransform> Instances = new List<MoveByTransform>(30);

		public float Speed = 40f;
		[SerializeField] SteeringSettings Steering = null;
		[SerializeField] [ReadOnly] bool isMoving = false;

		Vector2 velocity = Vector2.zero;
		public Vector2 Velocity => velocity;
		Transform destination;
		Vector2 offset, footPoint;
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

		void OnEnable()
		{
			Instances.Add(this);
		}

		void OnDisable()
		{
			Instances.Remove(this);
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
				// _unit.Velocity = destination.position - _transform.position - (Vector3)footPoint;

				UpdateSteering();

				quat = _unit.RotationTransform.rotation;
				quat.y = velocity.x < 0 ? 180f : 0f;
				_unit.RotationTransform.rotation = quat;
				
				if (_unit.Collider.bounds.Contains(destination.position))
				{
					ArrivedDestination();
					Debug.Log($"{gameObject.name} ArrivedDestination by collider");
					return;
				}
				float distance = Vector2.Distance(_transform.position, destination.position);
				if (distance < distanceToDestination)
				{
					ArrivedDestination();
					Debug.Log($"{gameObject.name} ArrivedDestination by distance");
					return;
				}
				_transform.position += (Vector3)velocity * Time.fixedDeltaTime * Speed / 100;
			}
		}

		void ArrivedDestination()
		{
			_unit.ArrivedDestination();
		}

		void UpdateSteering()
		{
			Vector2 seek = Seek(destination.position);
			Vector2 sep = Separate();
			Vector2 ali = Align();
			Vector2 coh = Cohesion();

			seek *= Steering.Seek ? Steering.SeekWeight : 0f;
			sep *= Steering.Separate ? Steering.SeparateWeight : 0f;
			ali *= Steering.Align ? Steering.AlignWeight : 0f;
			coh *= Steering.Cohesion ? Steering.CohesionWeight : 0f;

			velocity += seek + sep + ali + coh;
			velocity.Normalize();
			// _unit.Velocity = Vector2.ClampMagnitude(_unit.Velocity, 1f);
		}

		Vector2 Seek(Vector2 dest)
		{
			Vector2 desired = dest - (Vector2)_transform.position - footPoint;
			desired.Normalize();
			return desired - velocity;
		}

		Vector2 Separate()
		{
			Vector2 sum = Vector2.zero;
			int count = 0;
			foreach (var other in Instances)
			{
				float dist = Vector2.Distance(_transform.position, other.transform.position);
				if (dist > 0 && dist < _unit.Collider.radius)
				{
					Vector2 diff = _transform.position - other.transform.position;
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
				return sum - velocity;
			}
			else
				return Vector2.zero;
		}

		Vector2 Align()
		{
			float neighborDist = 3f;
			Vector2 sum = Vector2.zero;
			int count = 0;
			foreach (var other in Instances)
			{
				float dist = Vector2.Distance(_transform.position, other.transform.position);
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
				return sum - velocity;
			}
			else
				return Vector2.zero;
		}

		Vector2 Cohesion()
		{
			float neighborDist = 3f;
			Vector2 sum = Vector2.zero;
			int count = 0;
			foreach (var other in Instances)
			{
				float dist = Vector2.Distance(_transform.position, other.transform.position);
				if (dist > 0 && dist < neighborDist)
				{
					sum += velocity;
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
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, (Vector2)_transform.position + velocity);
			}
		}
    }
}