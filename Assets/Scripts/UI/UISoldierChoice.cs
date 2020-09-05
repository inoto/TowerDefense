using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class UISoldierChoice : UILevelControl
	{
		public event Action<UISoldierChoice, List<bool>> GoButtonClickedEvent;

		[SerializeField] Color greyColor = Color.grey;
		[SerializeField] GridLayoutGroup grid = null;
		[SerializeField] GameObject soldier = null;
		[SerializeField] Button goButton = null;
		[SerializeField] Button cancelButton = null;

		List<Soldier> soldiers;
		List<bool> soldiersMarkers = new List<bool>();

		void Awake()
		{
			gameObject.SetActive(false);

			for (int i = 0; i < grid.transform.childCount; i++)
			{
				var button = grid.transform.GetChild(i).GetComponentInChildren<Button>();
				var i1 = i;
				button.onClick.AddListener(() => OnSoldierClicked(i1));
			}

			goButton.onClick.AddListener(OnGoButtonClicked);
			cancelButton.onClick.AddListener(OnCancelButtonClicked);
		}

		public void Show(Building building)
		{
			if (UILevelControlsManager.Instance.IsSomeControlShown)
				return;

			this.soldiers = building.Soldiers;

			soldiersMarkers.Clear();
			for (int i = 0; i < soldiers.Count; i++)
				soldiersMarkers.Add(false);

			for (int i = 0; i < grid.transform.childCount; i++)
			{
				if (i > soldiers.Count-1)
					grid.transform.GetChild(i).gameObject.SetActive(false);
				else
				{
					grid.transform.GetChild(i).GetComponent<Image>().color = greyColor;
					grid.transform.GetChild(i).gameObject.SetActive(true);
				}
			}

			transform.position = building.transform.position;
			Show();
		}

		public void OnSoldierClicked(int index)
		{
			var image = grid.transform.GetChild(index).GetComponentInChildren<Image>();
			if (!soldiersMarkers[index])
			{
				image.color = Color.green;
				soldiersMarkers[index] = true;
			}
			else
			{
				image.color = greyColor;
				soldiersMarkers[index] = false;
			}
		}

		void OnGoButtonClicked()
		{
			GoButtonClickedEvent?.Invoke(this, soldiersMarkers);
			Hide();
		}

		void OnCancelButtonClicked()
		{
			Hide();
		}
	}
}