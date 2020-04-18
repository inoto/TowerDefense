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

		public override void Init()
		{
			base.Init();
			
			_canvas.UpdateCounterText(SoldiersCountInBuilding);
		}

		public override void AddSoldier(Soldier soldier)
		{
			base.AddSoldier(soldier);

			SoldierAssignedEvent?.Invoke(soldier);
		}

		public override void ActivateSoldier()
		{
			base.ActivateSoldier();
			
			_canvas.UpdateCounterText(SoldiersCountInBuilding);
		}

		public override Soldier RemoveSoldier()
		{
			Soldier soldier = base.RemoveSoldier();

			SoldierUnassignedEvent?.Invoke(soldier);

			_canvas.UpdateCounterText(SoldiersCountInBuilding);
			return soldier;
		}
	}
}