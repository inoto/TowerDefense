using UnityEngine;

namespace TowerDefense
{
	public class DragArrowMaker : MonoBehaviour
	{
		[SerializeField] LayerMask dragEndFilter = 0;

		RaycastHit2D[] results = new RaycastHit2D[10];
		UIDragArrow _dragArrow;

		public void OnDragStarted(Vector2 point)
		{
			_dragArrow = UILevelControlsManager.Instance.GetControl(UILevelControlsManager.LevelControl.DragArrow) as UIDragArrow;

			var ownerBuilding = GetComponent<Building>();
			if (ownerBuilding != null && ownerBuilding.SoldiersCount <= 0)
			{
				_dragArrow.End();
				_dragArrow = null;
			}
			else
				_dragArrow.Show(transform.position);
		}

		public void OnDragMoved(Vector2 point)
		{
			if (_dragArrow != null)
			{
				int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(point),
					Vector2.zero, results, Mathf.Infinity, dragEndFilter);

				if (hits > 0)
				{
					var target = results[hits - 1].transform.gameObject;
					if (target != null && target.GetInstanceID() != GetInstanceID())
					{
						_dragArrow.UpdatePosition(point, true);
					}
				}
				else
					_dragArrow.UpdatePosition(point, false);
			}
		}

		public void OnDragEnded(Vector2 point)
		{
			if (_dragArrow != null)
			{
				int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(point),
					Vector2.zero, results, Mathf.Infinity, dragEndFilter);

				if (hits > 0)
				{
					var target = results[hits - 1].transform.gameObject;
					if (target != null && target.GetInstanceID() != GetInstanceID())
					{
						var building = target.GetComponent<Building>();
						if (building != null)
						{
							if (building.SoldiersCount >= building.MaxSoldiersCount)
								return;

							GetComponent<Building>().RemoveSoldier().AssignToBuilding(building);
						}

						var wizard = target.GetComponent<Wizard>();
						if (wizard != null)
						{
							GetComponent<Building>().RemoveSoldier().AttackWizard(wizard);
						}
					}
				}

				_dragArrow.End();
				_dragArrow = null;
			}
		}
	}
}