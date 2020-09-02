using UnityEngine;

namespace TowerDefense
{
	public class DragArrowMaker : MonoBehaviour
	{
		[SerializeField] LayerMask dragEndFilter = 0;

		RaycastHit2D[] results = new RaycastHit2D[10];
		DragArrow dragArrow;

		public void OnDragStarted(Vector2 point)
		{
			dragArrow = UILevel.Instance.CreateDragArrow();
			var success = dragArrow.Show(gameObject);
			if (!success)
				dragArrow = null;
		}

		public void OnDragMoved(Vector2 point)
		{
			if (dragArrow != null)
			{
				int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(point),
					Vector2.zero, results, Mathf.Infinity, dragEndFilter);

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
					Vector2.zero, results, Mathf.Infinity, dragEndFilter);

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