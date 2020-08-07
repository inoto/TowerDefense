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
        public void OnTap()
        {
            ClickedEvent?.Invoke(this, ConstructDataList);
        }

        public void OnLongTap()
        {
	        throw new NotImplementedException();
        }

        public void OnDragStarted(Vector2 point)
        {
	        throw new NotImplementedException();
        }

        public void OnDragMoved(Vector2 point)
        {
	        throw new NotImplementedException();
        }

        public void OnDragEnded(Vector2 point)
        {
	        throw new NotImplementedException();
        }
    }
}