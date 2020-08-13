using System;
using UnityEngine;

namespace TowerDefense
{
    public class InputMouse : MonoBehaviour
    {
        [SerializeField] LayerMask clickableLayers;

        RaycastHit2D[] results = new RaycastHit2D[10];
        public static IClickable selected = null;
        public static IDraggable draggable = null;

		bool dragStarted = false;
        Vector2 startPoint = Vector2.zero;

        void Update()
        {
	        if (Input.GetMouseButtonDown(0))
	        {
		        draggable = FindDraggable();
		        draggable?.OnDragStarted(Input.mousePosition);
	        }
            else if (Input.GetMouseButtonUp(0))
	        {
		        if (draggable == null)
			        return;

				draggable.OnDragEnded(Input.mousePosition);
				draggable = null;
	        }
            else if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
	        {
		        if (draggable == null)
			        return;

				draggable.OnDragMoved(Input.mousePosition);
			}
        }

        IDraggable FindDraggable()
        {
	        int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(Input.mousePosition),
		        Vector2.zero, results, Mathf.Infinity, clickableLayers);
	        if (hits == 0)
		        return null;

	        // Debug.Log($"hits count {hits}");
	        Debug.Log($"last hit {results[hits - 1].transform.name}");
	        return results[hits - 1].transform.gameObject.GetComponent<IDraggable>();
        }

        public static void ClearSelection()
        {
            TowerInfo.Instance.Hide();
            ConstructionWheel.Instance.Hide();
            RaidInfo.Instance.Hide();
            selected = null;
            draggable = null;
        }
    }
}