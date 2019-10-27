using UnityEngine;
using UnityEngine.Rendering;

namespace TowerDefense
{
	[RequireComponent(typeof(SortingGroup))]
	[ExecuteInEditMode]
	public class UnitSortingDepth : MonoBehaviour
	{
		Transform _transform;
		SortingGroup _sortingGroup;

		void Awake()
		{
			_transform = GetComponent<Transform>();
			_sortingGroup = GetComponent<SortingGroup>();
		}

		void LateUpdate()
		{
			_sortingGroup.sortingOrder = -Mathf.RoundToInt(_transform.position.y*100);
		}
	}
}