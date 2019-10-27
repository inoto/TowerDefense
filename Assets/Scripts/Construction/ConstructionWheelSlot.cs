using System;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace TowerDefense
{
	public class ConstructionWheelSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public event Action<GameObject> ClickedEvent;

		public bool Active;
		public GameObject BuildingPrefab;
		[SerializeField] Image IconImage;
		[SerializeField] TextMeshProUGUI CostText;
		public Button Button;
		[SerializeField] public string TooltipText;

		public void Init(ConstructData data)
		{
			BuildingPrefab = data.BuildingPrefab;
			IconImage.sprite = data.IconSprite;
			if (data.Cost > 0)
				CostText.text = data.Cost.ToString();
			else
				CostText.transform.parent.gameObject.SetActive(false);
			TooltipText = data.TooltipText;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (Active && !string.IsNullOrEmpty(TooltipText))
				ToolTip.Instance.SetTooltip(TooltipText);
			
			// TODO: draw weapon radius
		}
 
		public void OnPointerExit(PointerEventData eventData)
		{
			if (Active && !string.IsNullOrEmpty(TooltipText))
				ToolTip.Instance.HideTooltip();
		}

		void OnMouseUp()
		{
			if (!Active)
				return;
			
			if (!string.IsNullOrEmpty(TooltipText))
				ToolTip.Instance.HideTooltip();
			
			if (BuildingPrefab != null)
			{
				if (ClickedEvent != null)
					ClickedEvent(BuildingPrefab);
			}
			
		}
	}
}