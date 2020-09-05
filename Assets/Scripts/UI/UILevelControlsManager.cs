using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
	public class UILevelControlsManager : Singleton<UILevelControlsManager>
	{
		public enum LevelControl
		{
			DragArrow = 0,
			TowerInfo,
			RaidInfo,
			ChooseSpecWheel,
			SoldierChoice
		}

		[SerializeField] UIDragArrow dragArrow = null;
		[SerializeField] UITowerInfo towerInfo = null;
		[SerializeField] UIRaidInfo raidInfo = null;
		[SerializeField] UIChooseSpecWheel chooseSpecWheel = null;
		[SerializeField] UISoldierChoice soldierChoice = null;

		public bool IsSomeControlShown => currentlyShownControl != null;
		public UILevelControl CurrentlyShownControl => currentlyShownControl;

		UILevelControl currentlyShownControl;

		void Start()
		{
			dragArrow.ShownEvent += OnControlShown;
			towerInfo.ShownEvent += OnControlShown;
			raidInfo.ShownEvent += OnControlShown;
			chooseSpecWheel.ShownEvent += OnControlShown;
			soldierChoice.ShownEvent += OnControlShown;
		}

		void OnDestroy()
		{
			dragArrow.ShownEvent -= OnControlShown;
			towerInfo.ShownEvent -= OnControlShown;
			raidInfo.ShownEvent -= OnControlShown;
			chooseSpecWheel.ShownEvent -= OnControlShown;
			soldierChoice.ShownEvent -= OnControlShown;
		}

		public UILevelControl GetControl(LevelControl type, bool activate = false)
		{
			UILevelControl control = null;
			switch (type)
			{
				case LevelControl.DragArrow:
				{
					// HideAll();
					// dragArrow.gameObject.SetActive(activate);
					control = dragArrow;
					break;
				}
				case LevelControl.TowerInfo:
				{
					// HideAll();
					// towerInfo.gameObject.SetActive(activate);
					control = towerInfo;
					break;
				}
				case LevelControl.RaidInfo:
				{
					// HideAll();
					// raidInfo.gameObject.SetActive(activate);
					control = raidInfo;
					break;
				}
				case LevelControl.ChooseSpecWheel:
				{
					// HideAll();
					// chooseSpecWheel.gameObject.SetActive(activate);
					control = chooseSpecWheel;
					break;
				}
				case LevelControl.SoldierChoice:
				{
					// HideAll();
					// soldierChoice.gameObject.SetActive(activate);
					control = soldierChoice;
					break;
				}
			}

			return control;
		}

		void HideAll()
		{
			dragArrow.gameObject.SetActive(false);
			towerInfo.gameObject.SetActive(false);
			raidInfo.gameObject.SetActive(false);
			chooseSpecWheel.gameObject.SetActive(false);
		}

		void OnControlShown(UILevelControl control)
		{
			currentlyShownControl = control;
		}

		void OnControlHidden(UILevelControl control)
		{
			currentlyShownControl = null;
		}

		public void Clear()
		{
			if (currentlyShownControl != null)
			{
				currentlyShownControl.Hide();
				currentlyShownControl = null;
			}
		}
	}
}