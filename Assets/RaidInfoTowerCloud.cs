using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class RaidInfoTowerCloud : MonoBehaviour
    {
        public event Action<RaidInfoTowerCloud> MoreSoldiersButtonClickedEvent;
        public event Action<RaidInfoTowerCloud> LessSoldiersButtonClickedEvent;

        public Building Building { get; set; } = null;

        public Image MoreSoldiersButton;
        public Image LessSoldiersButton;
        public int DesiredSoldiersCount = 0;
        public TextMeshProUGUI DesiredSoldiersText;

        public void UpdateText()
        {
            DesiredSoldiersText.text = DesiredSoldiersCount.ToString();

            if (DesiredSoldiersCount <= 0)
            {
                LessSoldiersButton.gameObject.SetActive(false);
                if (!MoreSoldiersButton.gameObject.activeSelf)
                    MoreSoldiersButton.gameObject.SetActive(true);
                else if (Building.SoldiersCountInBuilding == 0)
                    MoreSoldiersButton.gameObject.SetActive(false);
            }
            else if (DesiredSoldiersCount >= Building.SoldiersCountInBuilding)
            {
                MoreSoldiersButton.gameObject.SetActive(false);
                if (!LessSoldiersButton.gameObject.activeSelf)
                    LessSoldiersButton.gameObject.SetActive(true);
            }
            else
            {
                if (!MoreSoldiersButton.gameObject.activeSelf)
                    MoreSoldiersButton.gameObject.SetActive(true);
                if (!LessSoldiersButton.gameObject.activeSelf)
                    LessSoldiersButton.gameObject.SetActive(true);
            }
        }

        public void MoreSoldiersButtonClicked()
        {
            MoreSoldiersButtonClickedEvent?.Invoke(this);
        }

        public void LessSoldiersButtonClicked()
        {
            LessSoldiersButtonClickedEvent?.Invoke(this);
        }
    }
}
