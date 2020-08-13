using UnityEngine;

namespace TowerDefense
{
	public interface IDraggable
	{
		void OnDragStarted(Vector2 point);
		void OnDragMoved(Vector2 point);
		void OnDragEnded(Vector2 point);
	}
}