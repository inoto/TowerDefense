using UnityEngine;

namespace TowerDefense
{
	public class UICamera : MonoBehaviour
	{
		static Camera camera;

		void Awake()
		{
			camera = GetComponent<Camera>();
		}

		public static Vector2 ScreenToWorldPoint(Vector2 point)
		{
			return camera.ScreenToWorldPoint(point);
		}

		public static Vector2 WorldToScreenPoint(Vector2 point)
		{
			return camera.WorldToScreenPoint(point);
		}
	}
}