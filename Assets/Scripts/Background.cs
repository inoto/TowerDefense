using UnityEngine;

namespace TowerDefense
{
    public class Background : MonoBehaviour, IClickable
    {
        public void OnTap()
        {
            InputMouse.ClearSelection();
        }

        public void OnLongTap()
        {
	        InputMouse.ClearSelection();
        }

        public void OnDragStarted(Vector2 point)
        {
	        InputMouse.ClearSelection();
        }

        public void OnDragMoved(Vector2 point)
        {
	        InputMouse.ClearSelection();
        }

        public void OnDragEnded(Vector2 point)
        {
	        InputMouse.ClearSelection();
        }
    }
}