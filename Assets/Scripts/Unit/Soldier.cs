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
		        ExitBuilding();
	        else
		        StopMoving();

	        building = null;
        }

        void GoToAssignedBuilding()
        {
	        movingToBuilding = true;
	        _moveByTransform.AssignTransform(building.transform);

	        ArrivedDestinationEvent += EnterBuilding;
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

        public void AttackWizard(Wizard wizard)
        {
            if (IsBusy || AttackingWizard)
                return;

            IsBusy = true;
            if (CurrentlyInBuilding)
	            ExitBuilding();
            else
	            StopMoving();

            AttackingWizard = true;
			// _moveByTransform.AssignTransform(wizard.transform);
			weapon.SetTarget(wizard);
            weapon.TargetDiedEvent += WizardDiedEvent;
        }

        void WizardDiedEvent()
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
		        ExitBuilding();
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

        protected override void Corpse()
        {
	        StopMoving();
	        base.Corpse();
        }
	}
}