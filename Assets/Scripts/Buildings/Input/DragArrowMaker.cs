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
		Tower targetTower;

		public void OnDragStarted(Vector2 point)
		{
			var tower = GetComponent<Tower>();
			if (tower != null && tower.OccupiedByEnemy)
			{
				return;
			}

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

						targetTower = target.GetComponent<Tower>();
						if (targetTower != null)
						{
							if (targetTower.OccupiedByEnemy)
							{
								if (building.SoldiersCount == 1)
								{
									building.RemoveLastSoldier().FreedOccupiedTower(targetTower.GetComponent<OccupiedByEnemy>());
								}
								else
								{
									var control = UILevelControlsManager.Instance.GetControl<UISoldierChoice>(
											UILevelControlsManager.LevelControl.SoldierChoice);
									control.Show(building);
									control.GoButtonClickedEvent += OnGoButtonClicked;
									control.HiddenEvent += OnControlHidden;

									_dragArrow.End();
									_dragArrow = null;
									return;
								}
							}
						}

						targetBuilding = target.GetComponent<Building>();
						if (targetBuilding != null)
						{
							if (targetBuilding.SoldiersCount >= targetBuilding.MaxSoldiersCount)
							{
								_dragArrow.End();
								_dragArrow = null;
								return;
							}

							if (building.SoldiersCount == 1)
							{
								targetBuilding.AddSoldier(building.RemoveLastSoldier());
							}
							else
							{
								var control = UILevelControlsManager.Instance.GetControl<UISoldierChoice>(UILevelControlsManager.LevelControl.SoldierChoice);
								control.Show(building, targetBuilding);
								control.GoButtonClickedEvent += OnGoButtonClicked;
								control.HiddenEvent += OnControlHidden;
							}
						}

						targetWizard = target.GetComponent<Wizard>();
						if (targetWizard != null)
						{
							if (building.SoldiersCount == 1)
							{
								building.UnloadLastSoldier().AttackWizard(targetWizard);
							}
							else
							{
								var control = UILevelControlsManager.Instance.GetControl<UISoldierChoice>(UILevelControlsManager.LevelControl.SoldierChoice);
								control.Show(building);
								control.GoButtonClickedEvent += OnGoButtonClicked;
								control.HiddenEvent += OnControlHidden;
							}
						}

						targetFood = target.GetComponent<Food>();
						if (targetFood != null && !targetFood.SoldierAssigned)
						{
							if (building.SoldiersCount == 1)
							{
								building.UnloadLastSoldier().TakeFood(targetFood);
							}
							else
							{
								var control = UILevelControlsManager.Instance.GetControl<UISoldierChoice>(UILevelControlsManager.LevelControl.SoldierChoice);
								control.Show(building);
								control.GoButtonClickedEvent += OnGoButtonClicked;
								control.HiddenEvent += OnControlHidden;
							}
						}
					}
				}

				_dragArrow.End();
				_dragArrow = null;
			}
		}

		void OnControlHidden(UILevelControl control)
		{
			var uiSoldierChoice = control as UISoldierChoice;
			uiSoldierChoice.HiddenEvent -= OnControlHidden;
			uiSoldierChoice.GoButtonClickedEvent -= OnGoButtonClicked;
		}

		void OnGoButtonClicked(UISoldierChoice control, List<int> indexes)
		{
			control.HiddenEvent -= OnControlHidden;
			control.GoButtonClickedEvent -= OnGoButtonClicked;

			if (targetTower != null && targetTower.OccupiedByEnemy)
			{
				var soldiers = building.RemoveSoldiers(indexes);
				for (int i = 0; i < soldiers.Count; i++)
				{
					soldiers[indexes[i]].FreedOccupiedTower(targetTower.GetComponent<OccupiedByEnemy>());
				}
			}
			else if (targetBuilding != null)
			{
				var soldiers = building.RemoveSoldiers(indexes);
				targetBuilding.AddSoldiers(soldiers);
			}
			else if (targetWizard != null)
			{
				var soldiers = building.UnloadSoldiers(indexes);
				for (int i = 0; i < soldiers.Count; i++)
				{
					soldiers[indexes[i]].AttackWizard(targetWizard);
				}
			}
			else if (targetFood != null)
			{
				var soldiers = building.UnloadSoldiers(indexes);
				for (int i = 0; i < soldiers.Count; i++)
				{
					soldiers[indexes[i]].TakeFood(targetFood);
				}
			}
		}
	}
}