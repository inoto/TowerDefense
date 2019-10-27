using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense
{
	[RequireComponent(typeof(Selectable))]
	public class BuildPlace : MonoBehaviour
	{
		public List<ConstructData> ConstructDataList;
	}
}