using UnityEngine;

namespace TowerDefense
{
	public class LongTapAble : MonoBehaviour
	{
		public enum Type
		{
			TowerInfo = 0
		}

		[SerializeField] Type uiType = 0;

		public void OnLongTap(Vector2 point)
		{
			switch (uiType)
			{
				case Type.TowerInfo:
				{
					var towerInfo = UILevelControlsManager.Instance.GetControl<UITowerInfo>(UILevelControlsManager.LevelControl.TowerInfo);
					if (UILevelControlsManager.Instance.IsSomeControlShown)
					{
						if (UILevelControlsManager.Instance.CurrentlyShownControl.Owner.GetInstanceID() ==
						    gameObject.GetInstanceID())
							return;
						
						UILevelControlsManager.Instance.Clear();
					}

					towerInfo.Show(this);
					break;
				}
			}
		}
	}
}