using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class DragArrowMaker : MonoBehaviour
	{
		[SerializeField] LayerMask dragEndFilter = 0;

		RaycastHit2D[] results = new RaycastHit2D[10];
		UIDragArrow _dragArrow;

		Building building;
		Building targetBuilding;
		Wizard targetWizard;
		Food targetFood;

		public void OnDragStarted(Vector2 point)
		{
			_dragArrow = UILevelControlsManager.Instance.GetControl<UIDragArrow>(UILevelControlsManager.LevelControl.DragArrow);
			if (UILevelControlsManager.Instance.IsSomeControlShown)
				UILevelControlsManager.Instance.Clear();

			var ownerBuilding = GetComponent<Building>();
			if (ownerBuilding != null && ownerBuilding.SoldiersCount <= 0)
			{
				_dragArrow.End();
				_dragArrow = null;
			}
			else
				_dragArrow.Show(transform);
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
					if (target != null)
					{
						_dragArrow.UpdatePosition(point, target.GetInstanceID() != gameObject.GetInstanceID());
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
					if (target != null && target.GetInstanceID() != gameObject.GetInstanceID())
					{
						building = GetComponent<Building>();

						targetBuilding = target.GetComponent<Building>();
						if (targetBuilding != null)
						{
							if (targetBuilding.SoldiersCount >= targetBuilding.MaxSoldiersCount)
								return;

							if (building.SoldiersCount == 1)
							{
								building.RemoveLastSoldier().AssignToBuilding(targetBuilding);
							}
							else
							{
								var control = UILevelControlsManager.Instance.GetControl<UISoldierChoice>(UILevelControlsManager.LevelControl.SoldierChoice);
								control.Show(building);
								control.GoButtonClickedEvent += OnGoButtonClicked;
							}
						}

						targetWizard = target.GetComponent<Wizard>();
						if (targetWizard != null)
						{
							if (building.SoldiersCount == 1)
							{
								building.RemoveLastSoldier().AttackWizard(targetWizard);
							}
							else
							{
								var control = UILevelControlsManager.Instance.GetControl<UISoldierChoice>(UILevelControlsManager.LevelControl.SoldierChoice);
								control.Show(building);
								control.GoButtonClickedEvent += OnGoButtonClicked;
							}
						}

						targetFood = target.GetComponent<Food>();
						if (targetFood != null)
						{
							if (building.SoldiersCount == 1)
							{
								building.RemoveLastSoldier().TakeFood(targetFood);
							}
							else
							{
								var control = UILevelControlsManager.Instance.GetControl<UISoldierChoice>(UILevelControlsManager.LevelControl.SoldierChoice);
								control.Show(building);
								control.GoButtonClickedEvent += OnGoButtonClicked;
							}
						}
					}
				}

				_dragArrow.End();
				_dragArrow = null;
			}
		}

		void OnGoButtonClicked(UISoldierChoice control, List<bool> soldiersMarkers)
		{
			control.GoButtonClickedEvent -= OnGoButtonClicked;

			if (targetBuilding != null)
			{
				var soldiers = building.RemoveSoldiers(soldiersMarkers);
				for (int i = 0; i < soldiers.Count; i++)
				{
					soldiers[i].AssignToBuilding(targetBuilding);
				}
				
			}
			else if (targetWizard != null)
			{
				var soldiers = building.RemoveSoldiers(soldiersMarkers);
				for (int i = 0; i < soldiers.Count; i++)
				{
					soldiers[i].AttackWizard(targetWizard);
				}
			}
			else if (targetFood != null)
			{
				var soldiers = building.RemoveSoldiers(soldiersMarkers);
				for (int i = 0; i < soldiers.Count; i++)
				{
					soldiers[i].TakeFood(targetFood);
				}
			}
		}
	}
}