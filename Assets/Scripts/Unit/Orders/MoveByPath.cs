using System;
using UnityEngine;
using NaughtyAttributes;
using Random = UnityEngine.Random;

namespace TowerDefense
{
	public class MoveByPath : Order
	{
		public static event Action<MoveByPath, string> LookingForPathEvent;
        public event Action EndOfPathArrivedEvent;

		const float MAX_WAYPOINT_OFFSET = 0.1f;

		[ReadOnly] public string PathName;
		public float Speed = 40f;
		[SerializeField] [ReadOnly] BezierCurve path;
		[SerializeField] [ReadOnly] bool isMoving;

		int segment = 0;
		public int PathIndex => segment;
		Vector2 waypoint;
		public Vector2 WaypointPoint => waypoint;
		Vector2 desired;
		Vector3 rotation;
		Vector2 offset;
		Vector2 footPoint;
        float spriteYoffset = 0.01f;
        float initialYRotation;

		AttachmentPoints _attachments;
        Weapon weapon;
        MoveByTransform moveByTransform;

        protected override void Awake()
		{
			base.Awake();

			_attachments = GetComponent<AttachmentPoints>();
			_attachments.Points.TryGetValue("Foot", out footPoint);
            weapon = GetComponentInChildren<Weapon>();
            moveByTransform = GetComponent<MoveByTransform>();
		}

        public void SetPath(string pathName)
		{
			segment = 0;
			isMoving = false;
			offset = new Vector2(Random.Range(-MAX_WAYPOINT_OFFSET, MAX_WAYPOINT_OFFSET),
			                     Random.Range(-MAX_WAYPOINT_OFFSET, MAX_WAYPOINT_OFFSET));
			LookingForPathEvent?.Invoke(this, pathName);

			// _unit.DiedEvent += Pause;
		}

		public void AssignPath(BezierCurve path)
		{
			this.path = path;

			// _unit.AddOrder(this, _unit.CurrentOrder == null);
            Activate();
        }

		public override void Activate()
		{
			base.Activate();
			waypoint = path.Anchors[segment] + offset;
			isMoving = true;

            weapon.TargetAcquiredEvent += TargetAcquiredEvent;
        }

        void TargetAcquiredEvent(Weapon weapon1)
        {
            weapon.TargetAcquiredEvent -= TargetAcquiredEvent;

			Pause();

            weapon.TargetDiedEvent += ContinueMoving;
            weapon.TargetOutOfRangeEvent += ContinueMoving;
		}

        void ContinueMoving(Weapon weapon1)
        {
            weapon.TargetDiedEvent -= ContinueMoving;
            weapon.TargetOutOfRangeEvent -= ContinueMoving;

			isMoving = true;

            weapon.TargetAcquiredEvent += TargetAcquiredEvent;
		}

        public override void Pause()
		{
            isMoving = false;
		}

		void Update()
		{
			if (!IsActive || !isMoving)
				return;
			
			desired = waypoint - (Vector2)_transform.position - footPoint;
			
			rotation = _unit.RotationTransform.rotation.eulerAngles;
			rotation.y = desired.x < 0 ? _unit.InitialYRotation + 180f : _unit.InitialYRotation;
			_unit.RotationTransform.rotation = Quaternion.Euler(rotation);
			
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

            if (_unit.RotationTransform.localPosition.y > _unit.SpriteMaxY)
                spriteYoffset = -0.005f;
            else if (_unit.RotationTransform.localPosition.y < -_unit.SpriteMaxY)
                spriteYoffset = 0.005f;
            spriteYoffset *= _unit.SpriteYSpeed;
            _unit.RotationTransform.localPosition += new Vector3(0, spriteYoffset, 0);
        }

		void ArrivedDestination()
		{
			isMoving = false;
            spriteYoffset = 0f;
			_unit.ArrivedDestination();
			// _unit.OrderEnded(this);
            EndOfPathArrivedEvent?.Invoke();
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
    }
}