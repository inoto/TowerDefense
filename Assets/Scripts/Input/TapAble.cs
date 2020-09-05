using UnityEngine;

namespace TowerDefense
{
	public class TapAble : MonoBehaviour
	{
		public void OnTap(Vector2 point)
		{
			if (UILevelControlsManager.Instance.IsSomeControlShown)
				UILevelControlsManager.Instance.Clear();
		}
	}
}