using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class MoraleResource : MonoBehaviour
	{
		[SerializeField] List<GameObject> Icons;

		void Start()
		{
			HideAllIcons();
			Icons[4].SetActive(true); // temporary enable 1
		}

		void HideAllIcons()
		{
			foreach (var icon in Icons)
			{
				icon.SetActive(false);
			}
		}

	}
}