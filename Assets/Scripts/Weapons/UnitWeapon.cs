using UnityEngine;

namespace TowerDefense
{
	public class UnitWeapon : Weapon
	{
		protected Order followingOrder;
		protected bool moveToTarget = false;

		public override void SetTarget(ITargetable target, bool moveToTarget = true)
		{
			base.SetTarget(target, moveToTarget);

			this.moveToTarget = moveToTarget;
		}

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

			switch (CurrentAttackState)
			{
				case AttackState.NonAttacking: // started attacking
					if (Physics2D.IsTouching(_collider, Target.Collider))
						return true;
					else
					{
						if (moveToTarget)
						{
							moveByTransform.AssignTransform(Target.GameObj.transform);
							followingOrder = moveByTransform;
						}
						RaiseTargetOutOfRangeEvent();
						return false;
					}
				case AttackState.TargetInRange:
					if (!Physics2D.IsTouching(_collider, Target.Collider))
					{
						if (moveToTarget)
						{
							moveByTransform.AssignTransform(Target.GameObj.transform);
							followingOrder = moveByTransform;
						}

						RaiseTargetOutOfRangeEvent();
						return false;
					}
					else
						return true;
				case AttackState.TargetOutOfRange:
					if (Physics2D.IsTouching(_collider, Target.Collider))
					{
						RaiseTargetInRangeEvent();
						followingOrder = null;
						return true;
					}
					break;
			}
			return false;
		}

		protected override bool AcquireTarget()
		{
			if (Target != null)
				return false;

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
			// if (Target != null) Target = null;
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