using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class MoveByPoint : MonoBehaviour
	{
		public float Speed = 40f;
		[SerializeField] Transform RotationTransform;
		[SerializeField] [ReadOnly] bool _isMoving;
		
		Transform _transform;
		AttachmentPoints _attachments;
		Unit _unit;

		Vector2 destination;
		Vector2 desired;
		Quaternion quat;
		Vector2 offset;
		Vector2 footPoint;
		
		void Awake()
		{
			_transform = transform;
			_attachments = GetComponent<AttachmentPoints>();
			_attachments.Points.TryGetValue("Foot", out footPoint);
			_unit = GetComponent<Unit>();
		}
		
		public void Init(Vector2 point)
		{
			_isMoving = false;
			destination = point;
			StartMoving();
		}
		
		void StartMoving()
		{
			_isMoving = true;
		}
		
		public void StopMoving()
		{
			_isMoving = false;
		}

		void Update()
		{
			if (_isMoving)
			{
				desired = destination - (Vector2)_transform.position - footPoint;
				
				quat = RotationTransform.rotation;
				quat.y = desired.x < 0 ? 180f : 0f;
				RotationTransform.rotation = quat;
				
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
			_isMoving = false;
			_unit.ArrivedDestination();
		}

		void OnDrawGizmos()
		{
			if (_isMoving)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, destination);
				Gizmos.color = Color.blue;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, (Vector2)_transform.position+desired);
			}
		}
	}
}