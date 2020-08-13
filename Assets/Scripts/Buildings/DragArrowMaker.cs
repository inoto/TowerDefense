using UnityEngine;

namespace TowerDefense
{
	public class DragArrowMaker : MonoBehaviour, IDraggable
	{
		[SerializeField] LayerMask clickableLayers = 0;

		RaycastHit2D[] results = new RaycastHit2D[10];
		DragArrow dragArrow;

		public void OnDragStarted(Vector2 point)
		{
			dragArrow = InterToolsFactory.Instance.CreateDragArrow();
			var success = dragArrow.Start(gameObject);
			if (!success)
				dragArrow = null;
		}

		public void OnDragMoved(Vector2 point)
		{
			if (dragArrow != null)
			{
				int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(point),
					Vector2.zero, results, Mathf.Infinity, clickableLayers);

				if (hits > 0)
				{
					dragArrow.UpdatePosition(point, results[hits - 1].transform.gameObject);
				}
				else
					dragArrow.UpdatePosition(point, null);
			}
		}

		public void OnDragEnded(Vector2 point)
		{
			if (dragArrow != null)
			{
				int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(point),
					Vector2.zero, results, Mathf.Infinity, clickableLayers);

				if (hits > 0)
				{
					dragArrow.End(results[hits - 1].transform.gameObject);
				}
				else
					dragArrow.End(null);

				dragArrow = null;
			}
		}
	}
}