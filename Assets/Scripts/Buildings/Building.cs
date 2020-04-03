﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
	public abstract class Building : MonoBehaviour
	{
		public static List<Building> Instances = new List<Building>();
		
		public static event Action<Building> BuiltEvent;
		public static event Action<Building> SoldiersCountChangedEvent;
		public event Action SoldiersCountChangedSingleEvent;

		public List<Soldier> Soldiers;
		public int SoldiersCount => Soldiers.Count;
		public int SoldiersCountInBuilding => Soldiers.Count(s => s.InBuilding);
		
		protected bool initialized = false;

		void Start()
		{
			Instances.Add(this);
			
			if (initialized)
				return;

			StartCoroutine(CheckCreatedManually());
		}

		IEnumerator CheckCreatedManually()
		{
			yield return new WaitForSeconds(0.1f);
			if (!initialized)
			{
				Init();
				BuiltEvent?.Invoke(this);
			}
		}

		public virtual void Init(Selectable fromSelectable = null)
		{
			if (initialized)
				return;
			
			Soldiers = new List<Soldier>();
			initialized = true;
		}
		
		public virtual void AddSoldier(Soldier soldier)
		{
			Soldiers.Add(soldier);
		}

		public virtual void ActivateSoldier()
		{
			SoldiersCountChangedEvent?.Invoke(this);
			SoldiersCountChangedSingleEvent?.Invoke();
		}

		public virtual Soldier RemoveSoldier()
		{
			Soldier soldier = Soldiers[Soldiers.Count-1];
			Soldiers.RemoveAt(Soldiers.Count-1);

			SoldiersCountChangedEvent?.Invoke(this);
			SoldiersCountChangedSingleEvent?.Invoke();

			return soldier;
		}
	}
}