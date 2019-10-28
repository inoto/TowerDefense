using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace TowerDefense
{
	public class Tower : Building
	{
		public static event Action<Tower, SoldiersDispenser.Priority, SoldiersDispenser.Priority> PriorityChangedEvent;
		public static event Action<Tower> DesiredChangedEvent;

		public event Action SpecChangedSingleEvent;
		
		public const int MAX_DESIRED_COUNT = 9;
		
		[Header("Tower")]
		public int Cost = 0;
		public Sprite Icon;
		public int DesiredCount = 0;
		SoldiersDispenser.Priority priorityForDesired = SoldiersDispenser.Priority.Normal;
		public SoldiersDispenser.Priority PriorityForDesired
		{
			get { return priorityForDesired; }
			set
			{
				SoldiersDispenser.Priority oldPriority = priorityForDesired;
				priorityForDesired = value;
				if (PriorityChangedEvent != null)
				{
					PriorityChangedEvent(this, oldPriority, priorityForDesired);
				}
			}
		}

		public Specialization.Type Specialization;

		BoxCollider coll;
		protected Weapon weapon;
		public TowerCanvas Canvas;
		protected Transform spriteTransform;

		public virtual int[] Damage
		{
			get
			{
				return new []{weapon.DamageMin, weapon.DamageMax};
			}
		}

		public virtual float AttackSpeed => weapon.AttackSpeed;


		void Awake()
		{
			coll = GetComponent<BoxCollider>();
			weapon = GetComponentInChildren<Weapon>();
			Canvas = GetComponentInChildren<TowerCanvas>();
			spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
		}

		public override void Init(Selectable fromSelectable = null)
		{
			base.Init(fromSelectable);

			weapon.gameObject.SetActive(false);
			Canvas.UpdateCounterText(Soldiers.Count(s => s.InBuilding), DesiredCount);
		}

		public virtual void AddDesired()
		{
			// let's keep 9 for now
			if (DesiredCount >= 9)
				return;
			
			DesiredCount += 1;
			Canvas.UpdateCounterText(Soldiers.Count(s => s.InBuilding), DesiredCount);

			if (DesiredChangedEvent != null)
				DesiredChangedEvent(this);
		}

		public virtual void RemoveDesired()
		{
			if (DesiredCount <= 0)
				return;
			
			DesiredCount -= 1;
			Canvas.UpdateCounterText(Soldiers.Count(s => s.InBuilding), DesiredCount);
			
			if (DesiredChangedEvent != null)
				DesiredChangedEvent(this);
		}

		public override void ActivateSoldier()
		{
			base.ActivateSoldier();
			
			Canvas.UpdateCounterText(Soldiers.Count(s => s.InBuilding), DesiredCount);
		}

		public override Soldier RemoveSoldier()
		{
			Soldier soldier = base.RemoveSoldier();
			Canvas.UpdateCounterText(Soldiers.Count(s => s.InBuilding), DesiredCount);

//			soldier.transform.position = spriteTransform.position;
			
			return soldier;
		}
		
		public void HideCanvas()
		{
			Canvas.gameObject.SetActive(false);
		}

		public void ShowCanvas()
		{
			Canvas.gameObject.SetActive(true);
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
