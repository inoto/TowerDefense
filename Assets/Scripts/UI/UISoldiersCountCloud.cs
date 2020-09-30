using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class UISoldiersCountCloud : MonoBehaviour
	{
		[SerializeField] Image _cloudImage = null;
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

		public void TowerOccupied(OccupiedByEnemy occupied)
		{
			if (occupied.NumberOfAliveMobs > 0)
				_cloudImage.color = Color.red;
			else
				_cloudImage.color = Color.white;
			textMeshPro.text = $"{occupied.NumberOfAliveMobs}";
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.black;
			Gizmos.DrawSphere((Vector2)transform.position + GetComponent<RectTransform>().pivot, 0.02f);
		}
	}
}