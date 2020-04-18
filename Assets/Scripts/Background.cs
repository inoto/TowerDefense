using UnityEngine;

namespace TowerDefense
{
    public class Background : MonoBehaviour, IClickable
    {
        public void OnClick()
        {
            InputMouse.ClearSelection();
        }
    }
}