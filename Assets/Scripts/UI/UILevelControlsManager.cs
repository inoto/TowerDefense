using System;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
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
			SoldierChoiceMultiple,
			SpecChoice,
			TrapChoice,
			SoldierChoice
		}

		[SerializeField] Camera uiCamera = null;
		public Camera UICamera => uiCamera;
		[SerializeField] UIDragArrow dragArrow = null;
		[SerializeField] UITowerInfo towerInfo = null;
		[SerializeField] UIRaidInfo raidInfo = null;
		[SerializeField] UISpecChoiceClouds _specChoiceClouds = null;
		[SerializeField] UISoldierChoiceMultiple _soldierChoiceMultiple = null;

		public bool IsSomeControlShown => currentlyShownControl != null;
		public UILevelControl CurrentlyShownControl => currentlyShownControl;

		UILevelControl currentlyShownControl;
		// UILevelControl currentlyShownChooseSpec

		[Serializable]
		public class LevelControlsDict : SerializableDictionaryBase<LevelControl, UILevelControl> {}

		public LevelControlsDict LevelControls = new LevelControlsDict(); 

		void Start()
		{
			// dragArrow.ShownEvent += OnControlShown;
			// towerInfo.ShownEvent += OnControlShown;
			// raidInfo.ShownEvent += OnControlShown;
			// // _specChoiceClouds.ShownEvent += OnControlShown;
			// _soldierChoiceMultiple.ShownEvent += OnControlShown;
			foreach (var kvp in LevelControls)
			{
				kvp.Value.Hide();
				if (kvp.Key == LevelControl.SpecChoice)
					continue;

				kvp.Value.ShownEvent += OnControlShown;
			}
		}

		void OnDestroy()
		{
			// dragArrow.ShownEvent -= OnControlShown;
			// towerInfo.ShownEvent -= OnControlShown;
			// raidInfo.ShownEvent -= OnControlShown;
			// // _specChoiceClouds.ShownEvent -= OnControlShown;
			// _soldierChoiceMultiple.ShownEvent -= OnControlShown;
			foreach (var kvp in LevelControls)
			{
				if (kvp.Key == LevelControl.SpecChoice)
					continue;

				kvp.Value.ShownEvent -= OnControlShown;
			}
		}

		public T GetControl<T>(LevelControl type) where T : class
		{
			return LevelControls[type].GetComponent<T>();
		}

		void HideAll()
		{
			// dragArrow.gameObject.SetActive(false);
			// towerInfo.gameObject.SetActive(false);
			// raidInfo.gameObject.SetActive(false);
			// // _specChoiceClouds.gameObject.SetActive(false);
			foreach (var kvp in LevelControls)
			{
				if (kvp.Key == LevelControl.SpecChoice)
					continue;

				kvp.Value.gameObject.SetActive(false);
			}
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