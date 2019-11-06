﻿using System;
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
		public static event Action<Tower, SoldiersDispenser.Priority> PriorityChangedEvent;
		public static event Action<Tower> DesiredChangedEvent;

		public event Action SpecChangedSingleEvent;
		
		[Header("Tower")]
		public int Cost = 0;
		public Sprite Icon;
		public int MaxDesired = 3;
		public int DesiredCount = 0;
		SoldiersDispenser.Priority _priorityForDesired = SoldiersDispenser.Priority.Normal;
		public SoldiersDispenser.Priority PriorityForDesired
		{
			get => _priorityForDesired;
			set
			{
				SoldiersDispenser.Priority oldPriority = _priorityForDesired;
				_priorityForDesired = value;
				PriorityChangedEvent?.Invoke(this, oldPriority);
			}
		}

		public Specialization.Type Specialization;
		[SerializeField] SpecializationsData SpecDataAsset;

		protected Weapon Weapon;
		[HideInInspector] public TowerCanvas Canvas;
		protected Transform SpriteTransform;

		public virtual int[] Damage
		{
			get
			{
				return new []{Weapon.DamageMin, Weapon.DamageMax};
			}
		}

		public virtual float AttackSpeed => Weapon.AttackSpeed;


		protected virtual void Awake()
		{
			Weapon = GetComponentInChildren<Weapon>();
			Canvas = GetComponentInChildren<TowerCanvas>();
			SpriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
		}

		public override void Init(Selectable fromSelectable = null)
		{
			base.Init(fromSelectable);

			Weapon.gameObject.SetActive(false);
			Canvas.UpdateCounterText(SoldiersCountInBuilding, DesiredCount);
		}

		public virtual void AddDesired()
		{
			if (DesiredCount >= MaxDesired)
				return;

			DesiredCount += 1;
			Canvas.UpdateCounterText(SoldiersCountInBuilding, DesiredCount);

			DesiredChangedEvent?.Invoke(this);
		}

		public virtual void RemoveDesired()
		{
			if (DesiredCount <= 0)
				return;
			
			Debug.Log("tower RemoveDesired");
			
			DesiredCount -= 1;
			Canvas.UpdateCounterText(SoldiersCountInBuilding, DesiredCount);

			DesiredChangedEvent?.Invoke(this);
		}

		public override void ActivateSoldier()
		{
			base.ActivateSoldier();
			
			Canvas.UpdateCounterText(SoldiersCountInBuilding, DesiredCount);
		}

		public override Soldier RemoveSoldier()
		{
			Soldier soldier = base.RemoveSoldier();
			
			Canvas.UpdateCounterText(SoldiersCountInBuilding, DesiredCount);
			
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
