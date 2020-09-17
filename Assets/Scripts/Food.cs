using UnityEngine;

namespace TowerDefense
{
	public class Food : MonoBehaviour
	{
		public int Amount;
		public bool IsUsed = false;
		public bool SoldierAssigned = false;
		[SerializeField] FoodMerger foodMerger = null;

		void Start()
		{
			foodMerger.CollidedEvent += OnMergerCollided;
		}

		void OnDestroy()
		{
			foodMerger.CollidedEvent -= OnMergerCollided;
		}

		void OnMergerCollided(Collider2D other)
		{
			if (IsUsed)
				return;

			var food = other.GetComponent<FoodMerger>().Food;
			Amount += food.Amount;
			food.IsUsed = true;
			transform.position = transform.position + (other.transform.position - transform.position) * 0.5f;
			Destroy(food.gameObject);
		}
	}
}