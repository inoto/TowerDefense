using UnityEngine;
using UnityEngine.UI.Extensions;

namespace TowerDefense
{
	[RequireComponent(typeof(UILineRenderer))]
	public class UIDragArrow : UILevelControl
	{
		UILineRenderer lineRenderer;

		void Awake()
		{
			lineRenderer = GetComponent<UILineRenderer>();
		}

		public bool Show(Transform trans)
		{
			Owner = trans.gameObject;

			lineRenderer.Points[1] = lineRenderer.Points[0] =
				Camera.main.WorldToScreenPoint(trans.position);
			lineRenderer.SetVerticesDirty();
			gameObject.SetActive(true);
			return true;
		}

		public void UpdatePosition(Vector2 point, bool targetInFocus)
		{
			lineRenderer.Points[1] = point;
			lineRenderer.SetVerticesDirty();

			if (targetInFocus)
			{
				lineRenderer.color = Color.green;
			}
			else
			{
				lineRenderer.color = Color.white;
			}
		}

		public void End()
		{
			Hide();
		}
	}
}
