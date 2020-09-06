using System;
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
		public static event Action<Building> AnySoldiersCountChangedEvent;
		public event Action SoldiersCountChangedEvent;

		[SerializeField] int maxSoldiersCount = 3;
		[SerializeField] int startSoldiersCount = 0;

		public List<Soldier> Soldiers;
		public int SoldiersCount => Soldiers.Count;
		public int SoldiersCountInBuilding => Soldiers.Count(s => s.InBuilding);
		public int MaxSoldiersCount => maxSoldiersCount;
		
		protected bool initialized = false;

		protected virtual void Start()
		{
			Instances.Add(this);
			
			if (initialized)
				return;

			Wave.EndedEvent += OnWaveEndedEvent;

			Soldiers = new List<Soldier>(maxSoldiersCount);
			for (int i = 0; i < startSoldiersCount; i++)
			{
				var soldier = UnitFactory.Instance.CreateSoldier();
				soldier.transform.position = transform.position;
				soldier.AssignToBuilding(this);
			}
			initialized = true;
		}

        void OnWaveEndedEvent(int waveNumber)
        {
            // int counter = 0;
            // foreach (var soldier in Soldiers.Where(s => s.InBuilding))
            //     counter += 1;
			if (SoldiersCountInBuilding > 0)
			    PlayerController.Instance.SpendFood(SoldiersCountInBuilding, transform);
		}

        public virtual void AddSoldier(Soldier soldier)
		{
			Soldiers.Add(soldier);
			SoldiersCountChangedEvent?.Invoke();
		}

		public virtual void ActivateSoldier()
		{
			AnySoldiersCountChangedEvent?.Invoke(this);
			SoldiersCountChangedEvent?.Invoke();
		}

		public virtual Soldier RemoveSoldier(int index)
		{
			Soldier soldier = Soldiers[index];
			Soldiers.RemoveAt(index);

			AnySoldiersCountChangedEvent?.Invoke(this);
			SoldiersCountChangedEvent?.Invoke();

			return soldier;
		}

		public virtual List<Soldier> RemoveSoldiers(List<bool> indexes)
		{
			List<Soldier> soldiersToRemove = new List<Soldier>();
			for (int i = 0; i < indexes.Count; i++)
			{
				if (indexes[i])
					soldiersToRemove.Add(Soldiers[i]);
			}
			for (int i = 0; i < soldiersToRemove.Count; i++)
			{
				Soldiers.Remove(soldiersToRemove[i]);
			}

			AnySoldiersCountChangedEvent?.Invoke(this);
			SoldiersCountChangedEvent?.Invoke();

			return soldiersToRemove;
		}

		public virtual Soldier RemoveLastSoldier()
		{
			return RemoveSoldier(Soldiers.Count - 1);
		}
	}
}