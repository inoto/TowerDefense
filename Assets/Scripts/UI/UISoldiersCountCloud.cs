using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class UISoldiersCountCloud : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI textMeshPro = null;
		[SerializeField] Image specEmptyIcon = null;

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

		public void ShowSpecEmptyIcon(bool show)
		{
			specEmptyIcon.gameObject.SetActive(show);
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.black;
			Gizmos.DrawSphere((Vector2)transform.position + GetComponent<RectTransform>().pivot, 0.02f);
		}
	}
}