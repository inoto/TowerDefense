using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Soldier : Unit
	{
		public static event Action<Soldier, Building> ChangedBuildingEvent;
		public static event Action<Soldier> FreeSoldierEvent;
		
		public bool InBuilding = false;
		public Specialization Specialization;
        public bool AttackingWizard = false;

		[SerializeField] bool Movable = true;
		
		Building building;
        SoldierWeapon weapon;
		
		protected override void Awake()
		{
			base.Awake();
			
			Specialization = GetComponent<Specialization>();
            weapon = GetComponentInChildren<SoldierWeapon>();
        }

		void Start()
		{
			StartCoroutine(CheckCreatedManually());
		}

		IEnumerator CheckCreatedManually()
		{
			yield return new WaitForSeconds(0.2f);
			if (Movable)
			{
				FreeSoldierEvent?.Invoke(this);
			}
		}

        public void AssignToBuilding(Building building)
        {
            if (AttackingWizard)
                return;

			if (this.building != null)
				UnAssignFromBuilding();
			
			this.building = building;
			building.AddSoldier(this);
			ChangedBuildingEvent?.Invoke(this, this.building);

			GetComponent<MoveByTransform>().AssignTransform(building.transform);
		}
		
		public void NowFree()
		{
            if (AttackingWizard)
                return;

			if (building != null)
				UnAssignFromBuilding();
			
			FreeSoldierEvent?.Invoke(this);
		}

		void UnAssignFromBuilding()
		{
			building = null;
			
			if (InBuilding)
				ExitTower();
			else
				StopMoving();
		}

		void ExitTower()
		{
			InBuilding = false;
			
			gameObject.SetActive(true);
		}

		public override void ArrivedDestination()
		{
			base.ArrivedDestination();
			
			if (building != null)
			{
				InBuilding = true;
				building.ActivateSoldier();
			}

            if (AttackingWizard)
            {
                Weapon weapon = GetComponentInChildren<SoldierWeapon>();
            }
			else
                gameObject.SetActive(false);
		}

        public void AttackWizard(Wizard wizard)
        {
            if (AttackingWizard)
                return;

            if (building != null)
                UnAssignFromBuilding();

            AttackingWizard = true;
			_moveByTransform.AssignTransform(wizard.transform);
            weapon.TargetDiedEvent += TargetDiedEvent;
            // AddOrder(_moveByTransform);

        }

        void TargetDiedEvent()
        {
            weapon.TargetDiedEvent -= TargetDiedEvent;

            AttackingWizard = false;
			NowFree();
        }

        protected override void Corpse()
        {
			base.Corpse();

			StopMoving();
        }
    }
}