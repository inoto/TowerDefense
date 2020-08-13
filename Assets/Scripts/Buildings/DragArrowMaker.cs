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
			dragArrow.Init(gameObject);
			if (dragArrow != null)
			{
				dragArrow.Start(point);
			}
		}

		public void OnDragMoved(Vector2 point)
		{
			if (dragArrow != null)
			{
				int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(point),
					Vector2.zero, results, Mathf.Infinity, clickableLayers);

				// Debug.Log($"hits count {hits}");
				// Debug.Log($"last hit {results[hits - 1].transform.name}");
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

				// Debug.Log($"hits count {hits}");
				// Debug.Log($"last hit {results[hits - 1].transform.name}");
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