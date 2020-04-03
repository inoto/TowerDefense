﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefense
{
	[RequireComponent(typeof(BoxCollider))]
	public class Selectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public static event Action<Tower> TowerClickedEvent;
		public static event Action<Selectable, List<ConstructData>> BuildPlaceClickedEvent;
		
		static Selectable selected;

		// [SerializeField] Sprite sprite;
	
		void OnMouseUp()
		{
			if (selected == this)
				return;
			
			Debug.Log($"# Selection # {gameObject.name} clicked");
			
			ClearSelection();

			Tower tower = GetComponent<Tower>();
			if (tower != null)
			{
				TowerClickedEvent?.Invoke(tower);
				selected = this;
				return;
			}
			
			BuildPlace buildPlace = GetComponent<BuildPlace>();
			if (buildPlace != null)
			{
				BuildPlaceClickedEvent?.Invoke(this, buildPlace.ConstructDataList);
				selected = this;
				return;
			}
		}

		void ClearSelection()
		{
			TowerInfo.Instance.Hide();
			ConstructionWheel.Instance.Hide();
			selected = null;
		}
		
		public void OnPointerEnter(PointerEventData eventData)
		{
//			if (sprite != null)
//				sprite.texture. = Color.yellow;
		}
 
		public void OnPointerExit(PointerEventData eventData)
		{
//			if (sprite != null)
//				sprite.color = Color.white;
		}
	}
}