using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
	public class InterToolsFactory : Singleton<InterToolsFactory>
	{
		[SerializeField] DragArrow dragArrowPrefab = null;

		public DragArrow CreateDragArrow()
		{
			return Instantiate(dragArrowPrefab.gameObject, transform).GetComponent<DragArrow>();
		}
	}
}