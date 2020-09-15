using System;
using UnityEngine;

namespace TowerDefense
{
	public class FoodMerger : MonoBehaviour
	{
		public event Action<Collider2D> CollidedEvent;
		[SerializeField] Food food = null;
		public Food Food => food;

		private void OnTriggerEnter2D(Collider2D other)
		{
			CollidedEvent?.Invoke(other);
		}
	}
}