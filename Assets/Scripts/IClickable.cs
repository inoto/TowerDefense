using UnityEngine;

namespace TowerDefense
{
    public interface IClickable
    {
        void OnTap();
        void OnLongTap();
        void OnDragStarted(Vector2 point);
        void OnDragMoved(Vector2 point);
        void OnDragEnded(Vector2 point);

    }
}