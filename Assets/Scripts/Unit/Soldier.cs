using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Soldier : Unit
	{
		public static event Action<Soldier, Building> ChangedBuildingEvent;
		public static event Action<Soldier> FreeSoldierEvent;
		
		public bool CurrentlyInBuilding = false;
		public Specialization Specialization;
        public bool AttackingWizard = false;
        public bool IsBusy = false;

        Building building;
		bool movingToBuilding = false;
        SoldierWeapon weapon;
        Food targetFood;
		
		protected override void Awake()
		{
			base.Awake();
			
			Specialization = GetComponent<Specialization>();
            weapon = GetComponentInChildren<SoldierWeapon>();
        }

		public void AssignToBuilding(Building newBuilding, bool instantly = false)
        {
	        if (building != null)
				UnAssignFromBuilding();
			
			building = newBuilding;

			if (instantly)
			{
				return;
			}

			GoToAssignedBuilding();

			ChangedBuildingEvent?.Invoke(this, building);
		}

        void UnAssignFromBuilding()
        {
	        if (CurrentlyInBuilding)
		        building.UnloadSoldier(this);
	        else
		        StopMoving();

	        building = null;
        }

        void GoToAssignedBuilding()
        {
	        movingToBuilding = true;
	        _moveByTransform.AssignTransform(building.transform);

	        ArrivedDestinationEvent += LoadToBuilding;
        }

        void LoadToBuilding()
        {
			building.LoadSoldier(this);
        }

        public void EnterBuilding()
        {
	        if (movingToBuilding)
	        {
		        ArrivedDestinationEvent -= EnterBuilding;
		        movingToBuilding = false;
	        }

	        CurrentlyInBuilding = true;

	        gameObject.SetActive(false);
        }

        public void ExitBuilding()
        {
	        CurrentlyInBuilding = false;

	        gameObject.SetActive(true);
        }

        protected override void Corpse()
        {
	        StopMoving();
	        base.Corpse();
        }

        public void AttackWizard(Wizard wizard)
        {
            if (IsBusy || AttackingWizard)
                return;

            IsBusy = true;
            if (CurrentlyInBuilding)
	            building.UnloadSoldier(this);
            else
	            StopMoving();

            AttackingWizard = true;
            weapon.SetTarget(wizard);
            weapon.TargetDiedEvent += WizardDiedEvent;
        }

        void WizardDiedEvent(Weapon weapon1)
        {
	        weapon.TargetDiedEvent -= WizardDiedEvent;

	        AttackingWizard = false;
	        IsBusy = false;

	        GoToAssignedBuilding();
        }

        public void TakeFood(Food food)
        {
	        if (IsBusy)
		        return;

	        IsBusy = true;
	        if (CurrentlyInBuilding)
		        building.UnloadSoldier(this);
	        else
		        StopMoving();

	        targetFood = food;
	        targetFood.SoldierAssigned = true;

	        _moveByTransform.AssignTransform(food.transform);
	        ArrivedDestinationEvent += GetFood;
        }

        void GetFood()
        {
	        ArrivedDestinationEvent -= GetFood;

	        PlayerController.Instance.AddFood(targetFood.Amount, targetFood.transform);
	        targetFood.IsUsed = true;
	        Destroy(targetFood.gameObject);
	        targetFood = null;
	        IsBusy = false;

	        GoToAssignedBuilding();
        }

        OccupiedByEnemy occupied;

        public void FreedOccupiedTower(OccupiedByEnemy occupied)
        {
	        _moveByTransform.AssignTransform(occupied.transform, 1.2f);
	        this.occupied = occupied;
	        ArrivedDestinationEvent += OccupiedArrived;
        }

        void OccupiedArrived()
        {
	        ArrivedDestinationEvent -= OccupiedArrived;
			if (occupied.NumberOfAliveMobs > 0)
			{
				var mob = occupied.DefineMob(this);
				weapon.SetTarget(mob, false); // DOES NOT WORK
			}
			else
			{
				var occupiedBuilding = occupied.GetComponent<Building>();
				if (occupiedBuilding.SoldiersCount < occupiedBuilding.MaxSoldiersCount)
					occupiedBuilding.AddSoldier(this);
				else
					GoToAssignedBuilding();
				
			}
        }

        // void OccupiedInRange(Weapon weapon)
        // {
	       //  weapon.SetTarget(mob);
        // }
	}
}