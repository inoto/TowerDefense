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
		public int SoldiersCountInBuilding => Soldiers.Count(s => s.CurrentlyInBuilding);
		public int MaxSoldiersCount => maxSoldiersCount;
		public UISoldiersCountCloud SoldiersCountCloud;
		
		protected bool initialized = false;

		protected virtual void Start()
		{
			Instances.Add(this);
			
			if (initialized)
				return;

			WaveManager.WaveEndedEvent += OnWaveWaveEnded;

			Soldiers = new List<Soldier>(maxSoldiersCount);

			for (int i = 0; i < startSoldiersCount; i++)
			{
				var soldier = UnitFactory.Instance.SpawnObject<Soldier>(UnitFactory.Type.Soldier);
				soldier.transform.position = transform.position;
				AddSoldier(soldier, true);
				// soldier.AssignToBuilding(this, true);
			}

			initialized = true;
		}

		void OnDestroy()
		{
			WaveManager.WaveEndedEvent -= OnWaveWaveEnded;
		}

        void OnWaveWaveEnded(int waveNumber)
        {
	        if (SoldiersCountInBuilding > 0)
			    PlayerController.Instance.SpendFood(SoldiersCountInBuilding, transform);
		}

        public virtual void AddSoldier(Soldier soldier, bool instantly = false)
        {
	        Soldiers.Add(soldier);
	        soldier.AssignToBuilding(this, instantly);
			if (instantly) LoadSoldier(soldier);

	        AnySoldiersCountChangedEvent?.Invoke(this);
	        SoldiersCountChangedEvent?.Invoke();
        }

        public virtual void AddSoldiers(List<Soldier> soldiers, bool instantly = false)
        {
	        for (int i = 0; i < soldiers.Count; i++)
	        {
		        Soldiers.Add(soldiers[i]);
		        soldiers[i].AssignToBuilding(this, instantly);
		        if (instantly) LoadSoldier(soldiers[i]);
	        }

	        AnySoldiersCountChangedEvent?.Invoke(this);
	        SoldiersCountChangedEvent?.Invoke();
        }

        public virtual void LoadSoldier(int index)
        {
			Soldiers[index].EnterBuilding();
        }

        public virtual void LoadSoldier(Soldier soldier)
        {
			LoadSoldier(Soldiers.IndexOf(soldier));
        }

        public virtual Soldier UnloadSoldier(int index)
        {
			Soldiers[index].ExitBuilding();
			return Soldiers[index];
        }

		public virtual Soldier UnloadSoldier(Soldier soldier)
		{
			return UnloadSoldier(Soldiers.IndexOf(soldier));
		}

		public virtual Soldier UnloadLastSoldier()
		{
			return UnloadSoldier(Soldiers.Count - 1);
		}

		public virtual List<Soldier> UnloadSoldiers(List<int> indexes)
		{
			List<Soldier> unloadedSoldiers = new List<Soldier>();
			for (int i = 0; i < indexes.Count; i++)
			{
				if (Soldiers[indexes[i]].CurrentlyInBuilding)
					UnloadSoldier(indexes[i]);

				unloadedSoldiers.Add(Soldiers[indexes[i]]);
			}

			return unloadedSoldiers;
		}

        public virtual Soldier RemoveSoldier(int index)
		{
			if (Soldiers[index].CurrentlyInBuilding)
				UnloadSoldier(index);

			Soldier soldier = Soldiers[index];
			Soldiers.RemoveAt(index);

			AnySoldiersCountChangedEvent?.Invoke(this);
			SoldiersCountChangedEvent?.Invoke();

			return soldier;
		}

		public virtual List<Soldier> RemoveSoldiers(List<int> indexes)
		{
			List<Soldier> soldiersToRemove = new List<Soldier>();
			for (int i = 0; i < indexes.Count; i++)
			{
				if (Soldiers[indexes[i]].CurrentlyInBuilding)
					UnloadSoldier(indexes[i]);

				soldiersToRemove.Add(Soldiers[indexes[i]]);
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