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
		public static event Action<Building> SoldiersCountChangedEvent;
		public event Action SoldiersCountChangedSingleEvent;

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