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
		public int Cost = 0;
		public Sprite Icon;

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
    }
}
