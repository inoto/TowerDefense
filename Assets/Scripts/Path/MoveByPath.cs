using System;
using UnityEngine;
using NaughtyAttributes;
using Random = UnityEngine.Random;

namespace TowerDefense
{
	public class MoveByPath : MonoBehaviour, IUnitOrder
	{
		public static event Action<MoveByPath, string> LookingForPathEvent;

		const float MAX_WAYPOINT_OFFSET = 0.1f;

		public bool IsActive;
		[Space]
		[ReadOnly] public string PathName;
		public float Speed = 40f;
		[SerializeField] [ReadOnly] BezierCurve _path;
		[SerializeField] [ReadOnly] bool _isMoving;

		Transform _transform;
		AttachmentPoints _attachments;
		Unit _unit;
		
		int segment = 0;
		public int PathIndex => segment;
		Vector2 waypoint;
		public Vector2 WaypointPoint => waypoint;
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

		public void Init(string pathName)
		{
			segment = 0;
			_isMoving = false;
			offset = new Vector2(Random.Range(-MAX_WAYPOINT_OFFSET, MAX_WAYPOINT_OFFSET),
			                     Random.Range(-MAX_WAYPOINT_OFFSET, MAX_WAYPOINT_OFFSET));
			LookingForPathEvent?.Invoke(this, pathName);
		}

		public void AssignPath(BezierCurve path)
		{
			_path = path;
			
			LeanTween.delayedCall(0.1f, StartMoving);
		}

		void StartMoving()
		{
			waypoint = _path.Anchors[segment] + offset;
			_isMoving = true;
			
			_unit.AddOrder(this);
		}
		
		public void StopMoving()
		{
			_isMoving = false;
		}

		void Update()
		{
			if (IsActive && _isMoving)
			{
				desired = waypoint - (Vector2)_transform.position - footPoint;
				
				quat = _unit.RotationTransform.rotation;
				quat.y = desired.x < 0 ? 180f : 0f;
				_unit.RotationTransform.rotation = quat;
				
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
					waypoint = _path.Anchors[segment] + offset;
				}
				_transform.position += (Vector3)desired * Time.fixedDeltaTime * Speed / 100;
			}
		}

		void ArrivedDestination()
		{
			_isMoving = false;
			_unit.ArrivedDestination();
			_unit.OrderEnded(this);
		}

		void OnDrawGizmos()
		{
			if (_path != null && _path.Anchors.Length > 0)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, _path.Anchors[segment] + offset);
				for (int i = segment; i < _path.Anchors.Length-1; i++)
				{
					Gizmos.DrawLine(_path.Anchors[i] + offset, _path.Anchors[i+1] + offset);
				}
			}
			
			if (IsActive && _isMoving)
			{
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