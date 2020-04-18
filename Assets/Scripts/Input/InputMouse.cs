using UnityEngine;

namespace TowerDefense
{
    public class InputMouse : MonoBehaviour
    {
        [SerializeField] LayerMask LayersToClick;

        RaycastHit2D[] results = new RaycastHit2D[10];
        public static IClickable selected = null;

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                int hits = Physics2D.RaycastNonAlloc(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero, results, Mathf.Infinity, LayersToClick);
                if (hits == 0)
                    return;

                // Debug.Log($"hits {hits}");
                Debug.Log($"last hit {results[hits - 1].transform.name}");
                IClickable clickable = results[hits - 1].transform.gameObject.GetComponent<IClickable>();
                if (clickable != null)
                {
                    clickable.OnClick();
                }
            }
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