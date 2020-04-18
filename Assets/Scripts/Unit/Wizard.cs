using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class Wizard : Unit
    {
        public bool CreatedManually = false;
        public int ReagentReward = 0;

        void Start()
        {
            LoadData();
            StartCoroutine(CheckCreatedManually());
        }


        void LoadData()
        {
            // _healthy.SetMaxHealth(StatsData.Hp);
            //
            // MobWeapon weapon = GetComponentInChildren<MobWeapon>();
            // weapon.DamageMin = StatsData.DamageMin;
            // weapon.DamageMax = StatsData.DamageMax;
            // weapon.AttackInterval = StatsData.AttackRate;
            //
            // _healthy.ArmorType = StatsData.Armor;
            //
            // FoodReward = StatsData.FoodReward;
        }

        IEnumerator CheckCreatedManually()
        {
            yield return new WaitForSeconds(2f);
            if (CreatedManually)
            {
                
            }
        }
    }
}
