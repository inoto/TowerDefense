using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class OccupiedByEnemy : MonoBehaviour
	{
		[SerializeField] Weapon weapon = null;
		[SerializeField] List<Mob> mobPrefabs = new List<Mob>();

		public int NumberOfAliveMobs => mobs.Count + MobsInFight.Keys.Count;
		public Dictionary<Mob, List<Soldier>> MobsInFight = new Dictionary<Mob, List<Soldier>>();

		List<Mob> mobs = new List<Mob>();

		void Start()
		{
			for (int i = 0; i < mobPrefabs.Count; i++)
			{
				var go = Instantiate(mobPrefabs[i].gameObject, transform);
				go.transform.position = transform.position;
				go.SetActive(false);
				mobs.Add(go.GetComponent<Mob>());
			}
		}

		public Mob DefineMob(Soldier soldier)
		{
			Mob mob = null;
			if (mobs.Count > 0)
			{
				mob = mobs[mobs.Count - 1];
				mobs.Remove(mob);
				MobsInFight.Add(mob, new List<Soldier>() {soldier});
				mob.gameObject.SetActive(true);

				mob.Weapon.SetTarget(soldier);
				mob.DiedEvent += () =>
				{
					MobsInFight.Remove(mob);
					DefineMob(soldier);
				};
			}
			else if (MobsInFight.Count > 0)
			{
				int lowestNumberOfSoldiers = int.MaxValue;
				Mob suitableMob = null;
				foreach (var key in MobsInFight.Keys)
				{
					if (MobsInFight[key].Count < lowestNumberOfSoldiers)
					{
						lowestNumberOfSoldiers = MobsInFight[key].Count;
						suitableMob = key;
					}
				}
				mob = suitableMob;
				MobsInFight[mob].Add(soldier);
			}

			
			return mob;
		}
	}
}