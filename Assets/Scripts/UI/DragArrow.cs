using UnityEngine;
using UnityEngine.UI.Extensions;

namespace TowerDefense
{
	[RequireComponent(typeof(UILineRenderer))]
	public class DragArrow : MonoBehaviour
	{
		[SerializeField] LayerMask clickableLayers;

		UILineRenderer lineRenderer;

		GameObject owner;

		void Awake()
		{
			lineRenderer = GetComponent<UILineRenderer>();
		}

		public bool Show(GameObject owner)
		{
			this.owner = owner;
			var ownerBuilding = owner.GetComponent<Building>();
			if (ownerBuilding != null && ownerBuilding.SoldiersCount <= 0)
			{
				return false;
			}

			var targetBuilding = owner.GetComponent<Building>();
			if (targetBuilding != null)
			{
				if (targetBuilding.SoldiersCount <= 0)
					return false;
			}

			lineRenderer.Points[1] = lineRenderer.Points[0] =
				Camera.main.WorldToScreenPoint(owner.transform.position);
			lineRenderer.SetVerticesDirty();
			gameObject.SetActive(true);
			return true;
		}

		public void UpdatePosition(Vector2 point, GameObject target)
		{
			lineRenderer.Points[1] = point;
			lineRenderer.SetVerticesDirty();

			if (target != null && target.GetInstanceID() != owner.GetInstanceID())
			{
				lineRenderer.color = Color.green;
			}
			else
			{
				lineRenderer.color = Color.white;
			}
		}

		public void End(GameObject target)
		{
			Destroy(gameObject);
			if (target != null && target.GetInstanceID() != owner.GetInstanceID())
			{
				var building = target.GetComponent<Building>();
				if (building != null)
				{
					if (building.SoldiersCount >= building.MaxSoldiersCount)
						return;

					owner.GetComponent<Building>().RemoveSoldier().AssignToBuilding(building);
				}

				var wizard = target.GetComponent<Wizard>();
				if (wizard != null)
				{
					owner.GetComponent<Building>().RemoveSoldier().AttackWizard(wizard);
				}
			}
		}
	}
}
