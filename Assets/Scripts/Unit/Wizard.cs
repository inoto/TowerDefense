using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TowerDefense
{
    public class Wizard : Unit, IClickable
    {
        public static event Action<Wizard> ClickedEvent;

        [SerializeField] MobStatsData StatsData;
        public bool CreatedManually = false;
        public int ReagentReward = 0;
        [SerializeField] Image MagicIcon = null;
        [SerializeField] List<WizardMagic> Magics;

        void Start()
        {
            LoadData();

            StartCoroutine(CheckCreatedManually());


        }

        void LoadData()
        {
            _healthy.SetMaxHealth(StatsData.Hp);
            
            // MobWeapon weapon = GetComponentInChildren<MobWeapon>();
            // weapon.DamageMin = StatsData.DamageMin;
            // weapon.DamageMax = StatsData.DamageMax;
            // weapon.AttackInterval = StatsData.AttackRate;
            
            _healthy.ArmorType = StatsData.Armor;
            
            ReagentReward = StatsData.ReagentReward;
        }

        void Reset()
        {
            MagicIcon.gameObject.SetActive(false);
        }

        IEnumerator CheckCreatedManually()
        {
            yield return new WaitForSeconds(2f);
            // if (CreatedManually)
            // {
                CastSomething();
            // }
        }

        public void OnClick()
        {
            ClickedEvent?.Invoke(this);
        }

        void CastSomething()
        {
            int rand = Random.Range(0, Magics.Count);
            var magic = Magics[rand];

            MagicIcon.sprite = magic.Icon;
            MagicIcon.color = magic.IconColor;
            MagicIcon.gameObject.SetActive(true);
            magic.gameObject.SetActive(true);

            magic.DurationEndedEvent += CastEnded;
        }

        void CastEnded(WizardMagic magic)
        {
            magic.DurationEndedEvent -= CastEnded;

            MagicIcon.gameObject.SetActive(false);

            Invoke("CastSomething", 10f);
        }

        protected override void Corpse()
        {
            base.Corpse();

            foreach (var magic in Magics)
            {
                magic.gameObject.SetActive(false);
            }
            MagicIcon.gameObject.SetActive(false);
        }
    }
}
