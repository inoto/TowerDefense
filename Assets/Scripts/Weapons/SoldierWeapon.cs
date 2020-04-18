using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class SoldierWeapon : Weapon
	{
		// [SerializeField][ReadOnly] bool followingTarget = false;
		Order followingOrder;

        protected override bool TrackTarget()
		{
			if (Target == null)
				return false;

			if (Target.IsDied || !Target.GameObj.activeSelf)
			{
				// if (followingTarget)
				// {
				// 	followingTarget = false;
					// _unit.OrderEnded(followingOrder);
				// }

                RaiseTargetDiedEvent();
				Target = null;
                // _unit.OrderEnded(this);
				return false;
			}

			if (!Physics2D.IsTouching(_collider, Target.Collider))
			{
				// followingTarget = true;

                moveByTransform.AssignTransform(Target.GameObj.transform);
                RaiseTargetOutOfRangeEvent();
				// _unit.AddOrder(mbt);
				// followingOrder = mbt;
			}
			else if (Physics2D.IsTouching(_collider, Target.Collider))
			{
				// followingTarget = false;

				// _unit.OrderEnded(followingOrder);
                RaiseTargetInRangeEvent();
				followingOrder = null;
			}

			return true;
		}
		
		protected override bool AcquireTarget()
		{
			_targetsBuffer = new Collider2D[GameController.MAX_TARGETS_BUFFER];
			targetsCount = Physics2D.OverlapCollider(_collider, filter, _targetsBuffer);
			if (targetsCount > 0)
			{
				Target = DefineTarget();
                if (Target == null)
					return false;
				
				// _unit.AddOrder(this);
                RaiseTargetAcquiredEvent();
				return true;
			}
			Target = null;
			return false;
		}
		
		protected override ITargetable DefineTarget()
		{
			ITargetable bestTarget = null;
			float minDistance = float.MaxValue;
			for (int i = 0; i < targetsCount; i++)
			{
				ITargetable t = _targetsBuffer[i].GetComponent<ITargetable>();
				if (t == null || t.IsDied)
					continue;
				
				float distance = Vector2.Distance(t.Position, _transform.position);
				if (distance < minDistance)
				{
					bestTarget = t;
					minDistance = distance;
				}
			}
			if (bestTarget != null)
				Debug.Log($"{gameObject.name} DefineTarget [{bestTarget}]");
			return bestTarget;
		}
    }
}