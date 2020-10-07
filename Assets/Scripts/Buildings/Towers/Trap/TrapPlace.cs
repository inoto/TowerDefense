using UnityEngine;

namespace TowerDefense
{
	public class TrapPlace : MonoBehaviour
	{
		public Tower Tower;
		public bool IsBusy = false;

		public void Take()
		{
			IsBusy = true;
			gameObject.SetActive(false);
		}

		public void Free()
		{
			IsBusy = false;
			gameObject.SetActive(true);
		}
	}
}