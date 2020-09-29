using Boo.Lang;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "MobStatsData", menuName = "TowerDefense/MobStatsData", order = 0)]
    public class MobStatsData : ScriptableObject
    {
        public int Hp;
        public int DamageMin;
        public int DamageMax;
        public float AttackRate;
        public ArmorType Armor;
        public MoveSpeedType SpeedType;
        public MoveSpeed Speed;
        public int FoodReward;
        public int ReagentReward;
    }
}