using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class MoveByTransform : MonoBehaviour, IUnitOrder
	{
		public bool IsActive;
		[Space]
		public float Speed = 40f;
		[SerializeField] [ReadOnly] bool _isMoving;
		
		Transform _transform;
		AttachmentPoints _attachments;
		Unit _unit;

		Transform destination;
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
		
		public void Init(Transform transform)
		{
			_isMoving = false;
			destination = transform;
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
			if (IsActive && _isMoving)
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
			_isMoving = false;
			_unit.ArrivedDestination();
		}

		void OnDrawGizmos()
		{
			if (IsActive && _isMoving)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, destination.position);
				Gizmos.color = Color.blue;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, (Vector2)_transform.position+desired);
			}
		}
		
#region IUnitOrder

		public void StartOrder()
		{
			IsActive = true;
			Debug.Log($"{gameObject} order started");
		}

		public void PauseOrder()
		{
			IsActive = false;
			Debug.Log($"{gameObject} order paused");
		}
		
#endregion
	}
}