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
		[SerializeField] [ReadOnly] BezierCurve path;
		[SerializeField] [ReadOnly] bool isMoving;

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
			isMoving = false;
			offset = new Vector2(Random.Range(-MAX_WAYPOINT_OFFSET, MAX_WAYPOINT_OFFSET),
			                     Random.Range(-MAX_WAYPOINT_OFFSET, MAX_WAYPOINT_OFFSET));
			LookingForPathEvent?.Invoke(this, pathName);
		}

		public void AssignPath(BezierCurve path)
		{
			this.path = path;
			
			LeanTween.delayedCall(0.1f, StartMoving);
		}

		void StartMoving()
		{
			waypoint = path.Anchors[segment] + offset;
			isMoving = true;
			
			_unit.AddOrder(this);
		}
		
		public void StopMoving()
		{
			isMoving = false;
		}

		void Update()
		{
			if (!IsActive || !isMoving)
				return;
			
			desired = waypoint - (Vector2)_transform.position - footPoint;
			
			quat = _unit.RotationTransform.rotation;
			quat.y = desired.x < 0 ? 180f : 0f;
			_unit.RotationTransform.rotation = quat;
			
			float distance = desired.magnitude;
			desired.Normalize();
			if (distance < 0.1f)
			{
				if (segment >= path.Anchors.Length - 1)
				{
					ArrivedDestination();
					return;
				}
				segment += 1;
				waypoint = path.Anchors[segment] + offset;
			}
			_transform.position += (Vector3)desired * Time.fixedDeltaTime * Speed / 100;
		}

		void ArrivedDestination()
		{
			isMoving = false;
			_unit.ArrivedDestination();
			_unit.OrderEnded(this);
		}

		public void AssignToClosestWaypoint()
		{
			float dist;
			float smallestDist = Single.MaxValue;
			int closestSegment = 0;
			for (int i = 0; i < path.Anchors.Length; i++)
			{
				dist = Vector2.Distance(_transform.position, path.Anchors[i]);
				if (dist < smallestDist)
				{
					smallestDist = dist;
					closestSegment = i;
				}
			}

			if (closestSegment > 0)
			{
				segment = closestSegment;
				waypoint = path.Anchors[segment] + offset;
			}
		}

		void OnDrawGizmos()
		{
			if (path != null && path.Anchors.Length > 0)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, path.Anchors[segment] + offset);
				for (int i = segment; i < path.Anchors.Length-1; i++)
				{
					Gizmos.DrawLine(path.Anchors[i] + offset, path.Anchors[i+1] + offset);
				}
			}
			
			if (IsActive && isMoving)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine((Vector2)_transform.position + footPoint, (Vector2)_transform.position+desired);
			}
		}

#region IUnitOrder

		public void Start()
		{
			IsActive = true;
			Debug.Log($"{name} order started");
		}

		public void Pause()
		{
			IsActive = false;
			Debug.Log($"{name} order paused");
		}
		
		public string OrderName()
		{
			return name;
		}
		
#endregion
	}
}