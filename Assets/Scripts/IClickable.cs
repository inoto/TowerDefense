using UnityEngine;

namespace TowerDefense
{
    public interface IClickable
    {
        void OnTap();
        void OnLongTap();
    }
}