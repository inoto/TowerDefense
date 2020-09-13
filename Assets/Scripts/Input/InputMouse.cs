using System;
using UnityEngine;
using UnityEngine.UI;

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

        bool longTapped = false;

		[Header("Tap")]
		[SerializeField] LayerMask tapableFilter = 0;
		[SerializeField] LayerMask uiFilter = 0;
        

        RaycastHit2D[] results = new RaycastHit2D[10];
        public static LongTapAble longTapAbleSelected = null;
        public static DragArrowMaker draggable = null;

        float longTapTimer = 0;

		// bool dragStarted = false;
        Vector2 startPoint = Vector2.zero;

        void Start()
        {
	        // Physics2D.queriesStartInColliders = false;
        }

        void Update()
        {
	        if (Input.GetMouseButtonDown(0))
	        {
		        startPoint = Input.mousePosition;
	        }
            else if (Input.GetMouseButtonUp(0))
	        {
		        if (!dragStarted && !longTapped)
		        {
			        TapDetect();
			        return;
		        }

				if (dragStarted)
				{
					draggable.OnDragEnded(Input.mousePosition);
					draggable = null;
					dragStarted = false;
				}
				if (longTapped)
				{
					longTapAbleSelected = null;
					longTapped = false;
				}
	        }
            else if (Input.GetMouseButton(0))
	        {
		        if (dragStarted)
		        {
			        if (Math.Abs(Input.GetAxis("Mouse X")) > 0f || Math.Abs(Input.GetAxis("Mouse Y")) > 0f)
				        draggable.OnDragMoved(Input.mousePosition);
			        return;
		        }

		        if (longTapped)
			        return;

		        Vector2 diff = startPoint - (Vector2)Input.mousePosition;
                if (diff.magnitude >= minLengthToStart)
                {
	                StartDrag();
                }
                else
                {
					longTapTimer += Time.deltaTime;
					if (longTapTimer >= detectionTime)
					{
						LongTapDetect();
						longTapTimer = 0;
					}
                }
	        }
        }

        void TapDetect()
        {
	        int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(Input.mousePosition),
		        Vector2.zero, results, Mathf.Infinity, uiFilter);
	        if (hits > 0)
	        {
				var button = results[0].transform.GetComponent<UIButton>();
				if (button != null)
				{
					button.OnTap(Input.mousePosition);
					
				}
				return;
	        }

	        hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(Input.mousePosition),
		        Vector2.zero, results, Mathf.Infinity, tapableFilter);
	        if (hits == 0)
		        return;

	        Debug.Log($"# Input # TapDetect on {results[0].transform.gameObject.name}");
	        results[0].transform.gameObject.GetComponent<TapAble>().OnTap(Input.mousePosition);
		}

        void LongTapDetect()
        {
	        int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(Input.mousePosition),
		        Vector2.zero, results, Mathf.Infinity, longTapableFilter);
	        if (hits == 0)
	        {
		        // longTapAbleSelected = null;
		        return;
	        }

	        longTapped = true;
	        Debug.Log($"# Input # LongTapDetect on {results[hits - 1].transform.gameObject.name}");
	        longTapAbleSelected = results[hits - 1].transform.gameObject.GetComponent<LongTapAble>();
	        longTapAbleSelected?.OnLongTap(Input.mousePosition);
		}

        void StartDrag()
        {
	        int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(startPoint),
		        Vector2.zero, results, Mathf.Infinity, draggableFilter);
	        if (hits == 0)
		        return;

	        Debug.Log($"# Input # StartDrag on {results[hits - 1].transform.gameObject.name}");
	        draggable = results[hits - 1].transform.gameObject.GetComponent<DragArrowMaker>();
	        draggable?.OnDragStarted(Input.mousePosition);
	        dragStarted = true;
        }
    }
}