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

		// CampCanvas _canvas;

		void Awake()
		{
			// _canvas = GetComponentInChildren<CampCanvas>();
		}

		protected override void Start()
		{
			base.Start();
			
			// _canvas.UpdateCounterText(SoldiersCountInBuilding);
		}

		public override void AddSoldier(Soldier soldier)
		{
			base.AddSoldier(soldier);

			SoldierAssignedEvent?.Invoke(soldier);
		}

		public override void ActivateSoldier()
		{
			base.ActivateSoldier();
			
			// _canvas.UpdateCounterText(SoldiersCountInBuilding);
		}

		public override Soldier RemoveLastSoldier()
		{
			Soldier soldier = base.RemoveLastSoldier();

			// SoldierUnassignedEvent?.Invoke(soldier);

			// _canvas.UpdateCounterText(SoldiersCountInBuilding); // not calling, need to rework all these events
			
			return soldier;
		}

		public override List<Soldier> RemoveSoldiers(List<bool> indexes)
		{
			List<Soldier> soldiers = base.RemoveSoldiers(indexes);

			// _canvas.UpdateCounterText(SoldiersCountInBuilding);

			return soldiers;
		}
	}
}
