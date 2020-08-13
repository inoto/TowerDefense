using System;
using UnityEngine;

namespace TowerDefense
{
    public class InputMouse : MonoBehaviour
    {
        [Header("Drag")]
        [SerializeField] LayerMask draggableLayers = 0;
        [SerializeField] float minLengthToStart = 1f;

        [SerializeField] bool dragStarted = false;
        [SerializeField] bool dragMoving = false;
        [SerializeField] float distance = 0f;

		[Header("Long tap")]
        [SerializeField] LayerMask longTapableLayers = 0;
        [SerializeField] float detectionTime = 0.5f;

        

        RaycastHit2D[] results = new RaycastHit2D[10];
        public static IClickable selected = null;
        public static IDraggable draggable = null;

		// bool dragStarted = false;
        Vector2 startPoint = Vector2.zero;

        void Update()
        {
	        if (Input.GetMouseButtonDown(0))
	        {
		        startPoint = Input.mousePosition;
	        }
            else if (Input.GetMouseButtonUp(0))
	        {
		        if (draggable == null)
			        return;

				draggable.OnDragEnded(Input.mousePosition);
				draggable = null;
				dragStarted = false;
	        }
            else if (Input.GetMouseButton(0))
	        {
		        dragMoving = false;
		        Vector2 diff = startPoint - (Vector2)Input.mousePosition;
		        distance = diff.magnitude;
                if (draggable == null)
                {
	                if (diff.magnitude >= minLengthToStart)
	                {
		                StartDrag();
	                }
	                return;
                }

		        if (Input.GetAxis("Mouse X") == 0f || Input.GetAxis("Mouse Y") == 0f)
			        return;

				draggable.OnDragMoved(Input.mousePosition);
				dragMoving = true;
	        }
        }

        void StartDrag()
        {
	        int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(Input.mousePosition),
		        Vector2.zero, results, Mathf.Infinity, draggableLayers);
	        if (hits == 0)
		        return;

	        Debug.Log($"last hit {results[hits - 1].transform.name}");
	        draggable = results[hits - 1].transform.gameObject.GetComponent<IDraggable>();
	        draggable?.OnDragStarted(Input.mousePosition);
	        dragStarted = true;
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