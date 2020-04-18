using System;
using UnityEngine;

namespace TowerDefense
{
    public class PlayerController : Singleton<PlayerController>
    {
        public static event Action<int, Transform> FoodAmountChangedEvent;
        public static event Action<int, Transform> ReagentsAmountChangedEvent;

        int food;
        int reagents;

        public void AddFood(int amount, Transform trans)
        {
            food += amount;
            FoodAmountChangedEvent?.Invoke(amount, trans);
        }

        public void AddReagents(int amount, Transform trans)
        {
            reagents += amount;
            ReagentsAmountChangedEvent?.Invoke(amount, trans);
        }
    }
}