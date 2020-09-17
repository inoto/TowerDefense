using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
	public class SoldierWeapon : Weapon
	{
		Order followingOrder;

        protected override bool TrackTarget()
		{
			if (Target == null)
				return false;

			if (Target.IsDied || !Target.GameObj.activeSelf)
			{
				RaiseTargetDiedEvent();
				Target = null;

				return false;
			}

			if (!Physics2D.IsTouching(_collider, Target.Collider))
			{
				moveByTransform.AssignTransform(Target.GameObj.transform);
				followingOrder = moveByTransform;
                RaiseTargetOutOfRangeEvent();
                return false;
			}
			if (followingOrder != null && Physics2D.IsTouching(_collider, Target.Collider))
			{
				RaiseTargetInRangeEvent();
				followingOrder = null;
			}

			return true;
		}
		
		protected override bool AcquireTarget()
		{
			targetsCount = Physics2D.OverlapCollider(_collider, filter, _targetsBuffer);
			if (targetsCount > 0)
			{
				Target = DefineTarget();
                if (Target == null)
					return false;
                
                RaiseTargetAcquiredEvent();
                moveByTransform.StopMoving();
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