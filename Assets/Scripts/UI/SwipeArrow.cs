using UnityEngine;
using UnityEngine.UI.Extensions;

namespace TowerDefense
{
	[RequireComponent(typeof(UILineRenderer))]
	public class SwipeArrow : Singleton<SwipeArrow>
	{
		[SerializeField] LayerMask clickableLayers;

		UILineRenderer lineRenderer;

		Tower tower;

		protected override void Awake()
		{
			base.Awake();

			lineRenderer = GetComponent<UILineRenderer>();
		}

		void Start()
		{
			Hide();
			Tower.DragStartedEvent += Show;
			Tower.DragMovedEvent += UpdatePosition;
			Tower.DragEndedEvent += Hide;
		}

		void Show(Tower tower)
		{
			if (InputMouse.selected == tower)
				return;

			if (InputMouse.selected != null)
				InputMouse.ClearSelection();

			InputMouse.selected = tower;
			this.tower = tower;

			lineRenderer.Points[1] = lineRenderer.Points[0] =
				Camera.main.WorldToScreenPoint(tower.transform.position);
			lineRenderer.SetVerticesDirty();
			gameObject.SetActive(true);
		}

		void UpdatePosition(Vector2 point)
		{
			lineRenderer.Points[1] = point;
			lineRenderer.SetVerticesDirty();

			RaycastHit2D[] results = new RaycastHit2D[10]; ;
			int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(Input.mousePosition),
				Vector2.zero, results, Mathf.Infinity, clickableLayers);
			if (hits == 0)
			{
				lineRenderer.color = Color.white;
				return;
			}

			// Debug.Log($"hits count {hits}");
			Debug.Log($"last hit {results[hits - 1].transform.name}");
			var clickable = results[hits - 1].transform.gameObject.GetComponent<IClickable>();
			if (clickable != null)
			{
				lineRenderer.color = Color.green;
			}
			else
			{
				lineRenderer.color = Color.white;
			}
		}

		void Hide(Vector2 point)
		{
			Hide();

			
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}
