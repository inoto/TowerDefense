using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense
{
	[RequireComponent(typeof(Selectable))]
	public class BuildPlace : MonoBehaviour, IClickable
    {
        public static event Action<BuildPlace, List<ConstructData>> ClickedEvent;

		public List<ConstructData> ConstructDataList;
        public void OnClick()
        {
            ClickedEvent?.Invoke(this, ConstructDataList);
        }
    }
}