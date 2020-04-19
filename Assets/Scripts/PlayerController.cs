using System;
using UnityEngine;

namespace TowerDefense
{
    public class PlayerController : Singleton<PlayerController>
    {
        public static event Action<int, int, Transform> FoodAmountChangedEvent;
        public static event Action<int, int, Transform> ReagentsAmountChangedEvent;
        public static event Action<int, int> MoraleChangedEvent;

        public int Food = 0;
        public int Reagents = 0;
        public int Morale = 0;

        public int MinMoraleIfStarvation = -1;

        void Start()
        {

        }

        public void AddFood(int amount, Transform trans)
        {
            Food += amount;
            FoodAmountChangedEvent?.Invoke(amount, Food, trans);
        }

        public void SpendFood(int amount, Transform trans)
        {
            if (Food >= amount)
                AddFood(-amount, trans);
            else if (Morale > MinMoraleIfStarvation)
            {
                Morale -= 1;
                MoraleChangedEvent?.Invoke(-1, Morale);
            }
        }

        public void AddReagents(int amount, Transform trans)
        {
            Reagents += amount;
            ReagentsAmountChangedEvent?.Invoke(amount, Reagents, trans);
        }

        public void SpendReagents(int amount, Transform trans)
        {
            AddReagents(-amount, trans);
        }
    }
}