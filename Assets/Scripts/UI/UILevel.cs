using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
	public class UILevel : Singleton<UILevel>
	{
		public enum LevelControl
		{
			DragArrow = 0,
			TowerInfo,
			RaidInfo,
			ChooseSpecWheel
		}

		[SerializeField] DragArrow dragArrow = null;
		[SerializeField] TowerInfo towerInfo = null;
		[SerializeField] RaidInfo raidInfo = null;
		[SerializeField] ChooseSpecWheel chooseSpecWheel = null;

		public void Show(LevelControl control)
		{
			switch (control)
			{
				case LevelControl.DragArrow:
				{
					HideAll();
					dragArrow.gameObject.SetActive(true);
					break;
				}
				case LevelControl.TowerInfo:
				{
					HideAll();
					towerInfo.gameObject.SetActive(true);
					break;
				}
				case LevelControl.RaidInfo:
				{
					HideAll();
					raidInfo.gameObject.SetActive(true);
					break;
				}
				case LevelControl.ChooseSpecWheel:
				{
					HideAll();
					chooseSpecWheel.gameObject.SetActive(true);
					break;
				}
			}
		}

		void HideAll()
		{
			dragArrow.gameObject.SetActive(false);
			towerInfo.gameObject.SetActive(false);
			raidInfo.gameObject.SetActive(false);
			chooseSpecWheel.gameObject.SetActive(false);
		}
	}
}