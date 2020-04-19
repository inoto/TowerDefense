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
			Icons[3].SetActive(true); // morale is 0
		}

        void OnEnable()
        {
			PlayerController.MoraleChangedEvent += PlayerControllerOnMoraleChangedEvent;
        }

        void OnDisable()
        {
            PlayerController.MoraleChangedEvent -= PlayerControllerOnMoraleChangedEvent;
        }

		void PlayerControllerOnMoraleChangedEvent(int amount, int morale)
        {
            HideAllIcons();
            Icons[morale+3].SetActive(true);
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