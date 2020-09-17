using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
	public class Camp : Building
	{
		public static event Action<Soldier> SoldierAssignedEvent;
		public static event Action<Soldier> SoldierUnassignedEvent;

		public override void AddSoldier(Soldier soldier, bool instantly = false)
		{
			base.AddSoldier(soldier, instantly);

			SoldierAssignedEvent?.Invoke(soldier);
		}

		public override Soldier RemoveLastSoldier()
		{
			Soldier soldier = base.RemoveLastSoldier();

			// SoldierUnassignedEvent?.Invoke(soldier);

			return soldier;
		}

		public override List<Soldier> RemoveSoldiers(List<int> indexes)
		{
			List<Soldier> soldiers = base.RemoveSoldiers(indexes);

			// SoldierUnassignedEvent?.Invoke(soldier);

			return soldiers;
		}
	}
}
