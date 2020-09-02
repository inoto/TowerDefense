using System;
using UnityEngine;

namespace TowerDefense
{
    public class InputMouse : MonoBehaviour
    {
        [Header("Drag")]
        [SerializeField] LayerMask draggableFilter = 0;
        [SerializeField] float minLengthToStart = 1f;

        bool dragStarted = false;

        [Header("Long tap")]
        [SerializeField] LayerMask longTapableFilter = 0;
        [SerializeField] float detectionTime = 0.5f;

		[Header("Tap")]
		[SerializeField] LayerMask tapableFilter = 0;
        

        RaycastHit2D[] results = new RaycastHit2D[10];
        public static LongTapAble longTapAbleSelected = null;
        public static DragArrowMaker draggable = null;

        float longTapTimer = 0;

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
		        if (!dragStarted)
			        return;

				draggable.OnDragEnded(Input.mousePosition);
				draggable = null;
				dragStarted = false;
	        }
            else if (Input.GetMouseButton(0))
	        {
		        if (!dragStarted)
                {
	                Vector2 diff = startPoint - (Vector2)Input.mousePosition;
	                if (diff.magnitude >= minLengthToStart)
	                {
		                StartDrag();
	                }
	                else
	                {
						longTapTimer += Time.deltaTime;
						if (longTapAbleSelected == null && longTapTimer >= detectionTime)
						{
							LongTapDetect();
							longTapTimer = 0;
						}
						else
						{
							TapDetect();
						}
					}
	                return;
                }

		        if (Math.Abs(Input.GetAxis("Mouse X")) > 0f || Math.Abs(Input.GetAxis("Mouse Y")) > 0f)
			        draggable.OnDragMoved(Input.mousePosition);
	        }
        }

        void TapDetect()
        {
	        ClearSelection();
		}

        void LongTapDetect()
        {
	        int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(Input.mousePosition),
		        Vector2.zero, results, Mathf.Infinity, longTapableFilter);
	        if (hits == 0)
	        {
		        longTapAbleSelected = null;
		        return;
	        }

	        // Debug.Log($"LongTapDetect on {results[hits - 1].transform.name}");
	        longTapAbleSelected = results[hits - 1].transform.gameObject.GetComponent<LongTapAble>();
	        longTapAbleSelected?.OnLongTap(Input.mousePosition);
		}

        void StartDrag()
        {
	        int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(startPoint),
		        Vector2.zero, results, Mathf.Infinity, draggableFilter);
	        if (hits == 0)
		        return;

	        Debug.Log($"StartDrag on {results[hits - 1].transform.name}");
	        draggable = results[hits - 1].transform.gameObject.GetComponent<DragArrowMaker>();
	        draggable?.OnDragStarted(Input.mousePosition);
	        dragStarted = true;
        }

        public static void ClearSelection()
        {
	        // hide all
            longTapAbleSelected = null;
            draggable = null;
        }
    }
}