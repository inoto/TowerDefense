using TMPro;
using UnityEngine;

namespace TowerDefense
{
	public class UISoldiersCountCloud : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI textMeshPro = null;

		Building building;

		public void Attach(Building building)
		{
			this.building = building;

			building.SoldiersCountChangedEvent += OnSoldiersCountChanged;

			OnSoldiersCountChanged();
		}

		void OnDestroy()
		{
			if (building != null)
				building.SoldiersCountChangedEvent -= OnSoldiersCountChanged;
		}

		void OnSoldiersCountChanged()
		{
			textMeshPro.text = $"{building.SoldiersCount}";
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.black;
			Gizmos.DrawSphere((Vector2)transform.position + GetComponent<RectTransform>().pivot, 0.02f);
		}
	}
}