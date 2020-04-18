using System;
using UnityEngine;

namespace TowerDefense
{
    public class WizardMagicDanger : WizardMagic
    {
        public int DamagePerRate = 2;

        protected override void Tick()
        {
            Debug.Log("Damage tick");
            var soldiers = GameObject.FindObjectsOfType<Soldier>(); // TODO: do not use Find
            foreach (var soldier in soldiers)
            {
                if (soldier.gameObject.activeSelf)
                    soldier.Damage(DamagePerRate, DamageType.Magical);
            }
        }
    }
}