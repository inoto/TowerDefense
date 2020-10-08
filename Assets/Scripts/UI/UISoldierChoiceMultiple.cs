using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class UISoldierChoiceMultiple : UILevelControl
	{
		public event Action<UISoldierChoiceMultiple, List<int>> GoButtonClickedEvent;

		[SerializeField] Color greyColor = Color.grey;
		[SerializeField] GridLayoutGroup grid = null;
		[SerializeField] GameObject soldier = null;
		[SerializeField] Button goButton = null;
		[SerializeField] Button cancelButton = null;
		// [SerializeField] Image suitableIcon = null;
		// [SerializeField] Image notSuitableIcon = null;

		List<Image> images = new List<Image>();
		List<Button> buttons = new List<Button>();
		List<bool> soldiersMarkers = new List<bool>();
		List<Soldier> soldiers;
		Building targetBuilding;

		void Awake()
		{
			gameObject.SetActive(false);

			for (int i = 0; i < grid.transform.childCount; i++)
			{
				var child = grid.transform.GetChild(i);

				images.Add(child.GetComponentInChildren<Image>());

				var button = child.GetComponentInChildren<Button>();
				var i1 = i;
				button.onClick.AddListener(() => OnSoldierClicked(i1));
				buttons.Add(button);
			}

			goButton.onClick.AddListener(OnGoButtonClicked);
			cancelButton.onClick.AddListener(OnCancelButtonClicked);
		}

		public void Show(Building building, Building targetBuilding = null)
		{
			if (UILevelControlsManager.Instance.IsSomeControlShown)
				return;

			this.soldiers = building.Soldiers;
			this.targetBuilding = targetBuilding;

			soldiersMarkers.Clear();
			for (int i = 0; i < soldiers.Count; i++)
				soldiersMarkers.Add(false);

			for (int i = 0; i < grid.transform.childCount; i++)
			{
				if (i > soldiers.Count-1)
					grid.transform.GetChild(i).gameObject.SetActive(false);
				else
				{
					images[i].color = greyColor;
					buttons[i].interactable = true;
					grid.transform.GetChild(i).gameObject.SetActive(true);
				}
			}

			transform.position = building.transform.position;
			Show();
		}

		public void OnSoldierClicked(int index)
		{
			if (!soldiersMarkers[index])
			{
				images[index].color = Color.green;
				soldiersMarkers[index] = true;
			}
			else
			{
				images[index].color = greyColor;
				soldiersMarkers[index] = false;
			}

			if (targetBuilding == null)
				return;
			if (soldiersMarkers.Count(e => e) + targetBuilding.SoldiersCount >= targetBuilding.MaxSoldiersCount)
			{
				for (int i = 0; i < soldiersMarkers.Count; i++)
				{
					if (soldiersMarkers[i])
						continue;

					buttons[i].interactable = false;
				}
			}
			else
			{
				for (int i = 0; i < soldiersMarkers.Count; i++)
				{
					if (soldiersMarkers[i])
						continue;

					if (!buttons[i].interactable)
						buttons[i].interactable = true;
				}
			}
		}

		void OnGoButtonClicked()
		{
			List<int> indexes = new List<int>();
			for (int i = 0; i < soldiersMarkers.Count; i++)
				if (soldiersMarkers[i])
					indexes.Add(i);

			GoButtonClickedEvent?.Invoke(this, indexes);
			Hide();
		}

		void OnCancelButtonClicked()
		{
			Hide();
		}
	}
}