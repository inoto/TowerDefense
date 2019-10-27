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

		CampCanvas _canvas;

		void Awake()
		{
			_canvas = GetComponentInChildren<CampCanvas>();
		}

		public override void Init(Selectable fromSelectable = null)
		{
			base.Init(fromSelectable);
			
			_canvas.UpdateCounterText(Soldiers.Count(s => s.InBuilding));
		}

		public override void AddSoldier(Soldier soldier)
		{
			base.AddSoldier(soldier);

			if (SoldierAssignedEvent != null)
				SoldierAssignedEvent(soldier);
		}

		public override void ActivateSoldier()
		{
			base.ActivateSoldier();
			
			_canvas.UpdateCounterText(Soldiers.Count(s => s.InBuilding));
		}

		public override Soldier RemoveSoldier()
		{
			Soldier soldier = base.RemoveSoldier();

			if (SoldierUnassignedEvent != null)
				SoldierUnassignedEvent(soldier);
			
			_canvas.UpdateCounterText(Soldiers.Count(s => s.InBuilding));
			return soldier;
		}
	}
}