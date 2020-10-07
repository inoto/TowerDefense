using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class Tower : Building
    {
        public static event Action<Tower> ClickedEvent;

        public event Action SpecChangedSingleEvent;

        [Header("Tower")]
        public bool OccupiedByEnemy = false;

		[Header("Traps")]
		[SerializeField] List<TrapPlace> trapPlaces = new List<TrapPlace>();
		[SerializeField] List<Trap> currentTraps = new List<Trap>();

		[Header("Specialization")]
        public Specialization.Type Specialization;
		[SerializeField] SpecializationsSettings SpecDataAsset;

		protected Weapon weapon;
		protected Transform spriteTransform;

		public virtual int[] Damage
		{
			get
			{
				return new []{weapon.DamageMin, weapon.DamageMax};
			}
		}

		public virtual float AttackSpeed => weapon.AttackInterval;


		protected virtual void Awake()
		{
			weapon = GetComponentInChildren<Weapon>();
			spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
		}

		protected override void Start()
		{
			weapon.gameObject.SetActive(false);

			for (int i = 0; i < trapPlaces.Count; i++)
			{
				trapPlaces[i].Tower = this;
			}

			base.Start();
		}

		public void SetSpec(Specialization.Type spec)
		{
			Specialization = spec;
			SpecChangedSingleEvent?.Invoke();
		}

		public void ModifySpecValue(int value)
		{
			for (int i = 0; i < Soldiers.Count; i++)
			{
				Soldiers[i].Specialization.Modify(Specialization, value);
			}
		}

		public void AddTrap(Trap trap)
		{
			currentTraps.Add(trap);
			trap.FullyUsedEvent += OnTrapUsed;
		}

		void OnTrapUsed(Trap trap)
		{
			trap.FullyUsedEvent -= OnTrapUsed;

			trap.TrapPlace.IsBusy = false;
			currentTraps.Remove(trap);
		}
    }
}
