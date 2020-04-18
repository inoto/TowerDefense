using System;
using UnityEngine;

namespace TowerDefense
{
    public class WizardMagicStarvation : WizardMagic
    {
        public int Amount = -1;

        protected override void Tick()
        {
            PlayerController.Instance.AddFood(Amount, transform);
        }
    }
}