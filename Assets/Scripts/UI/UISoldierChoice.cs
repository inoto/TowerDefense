using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class UISoldierChoice : UILevelControl
	{
		public event Action<UISoldierChoice, int, GameObject> SoldierClickedEvent;

		[SerializeField] Color greyColor = Color.grey;
		[SerializeField] GridLayoutGroup grid = null;

		List<Image> images = new List<Image>();
		List<Button> buttons = new List<Button>();
		List<Soldier> soldiers;
		GameObject trapPrefab;

		void Awake()
		{
			gameObject.SetActive(false);

			for (int i = 0; i < grid.transform.childCount; i++)
			{
				var button = grid.transform.GetChild(i).GetComponentInChildren<Button>();
				var i1 = i;
				button.onClick.AddListener(() => OnSoldierClicked(i1));
				buttons.Add(button);
			}
		}

		public void Show(Building building, GameObject trapPrefab)
		{
			this.soldiers = building.Soldiers;
			this.trapPrefab = trapPrefab;

			for (int i = 0; i < grid.transform.childCount; i++)
			{
				if (i > soldiers.Count-1)
					grid.transform.GetChild(i).gameObject.SetActive(false);
				else
				{
					buttons[i].GetComponentInChildren<Image>().color = greyColor;
					buttons[i].interactable = true;
					grid.transform.GetChild(i).gameObject.SetActive(true);
				}
			}

			transform.position = building.transform.position;
			Show();
		}

		public void OnSoldierClicked(int index)
		{
			SoldierClickedEvent?.Invoke(this, index, trapPrefab);
			Hide();
		}
	}
}