using UnityEngine;

namespace TowerDefense
{
    public class InputMouse : MonoBehaviour
    {
        [SerializeField] LayerMask clickableLayers;

        RaycastHit2D[] results = new RaycastHit2D[10];
        public static IClickable selected = null;

        bool swipeStarted = false;
        Vector2 startPoint = Vector2.zero;
        IClickable swipeClickable;

        void Update()
        {
	        if (Input.GetMouseButtonDown(0))
	        {
		        swipeClickable = FindClickable();
		        if (swipeClickable != null)
		        {
			        swipeStarted = true;
			        swipeClickable.OnDragStarted(Input.mousePosition);
		        }
	        }
            else if (Input.GetMouseButtonUp(0))
	        {
		        if (swipeClickable == null)
			        return;

		        IClickable c = FindClickable();
		        if (c != null && c != swipeClickable)
		        {
					swipeClickable.OnDragEnded(Input.mousePosition);
		        }
	        }
            else if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
	        {
		        if (swipeClickable == null)
			        return;

				swipeClickable.OnDragMoved(Input.mousePosition);
			}
        }

        IClickable FindClickable()
        {
	        int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(Input.mousePosition),
		        Vector2.zero, results, Mathf.Infinity, clickableLayers);
	        if (hits == 0)
		        return null;

	        // Debug.Log($"hits count {hits}");
	        Debug.Log($"last hit {results[hits - 1].transform.name}");
	        return results[hits - 1].transform.gameObject.GetComponent<IClickable>();
        }

        public static void ClearSelection()
        {
            TowerInfo.Instance.Hide();
            ConstructionWheel.Instance.Hide();
            RaidInfo.Instance.Hide();
            selected = null;
        }
    }
}