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
        public bool IsBusy = false;

		[SerializeField] bool Movable = true;
		
		Building building;
        SoldierWeapon weapon;

        Food targetFood;
		
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

			_moveByTransform.AssignTransform(building.transform);
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
			
			// if (building != null)
			// {
			// 	InBuilding = true;
			// 	building.ActivateSoldier();
			// }

   //          if (AttackingWizard)
   //          {
   //              Weapon weapon = GetComponentInChildren<SoldierWeapon>();
   //          }
			// else
   //              gameObject.SetActive(false);
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
        }

        public void TakeFood(Food food)
        {
	        if (IsBusy)
		        return;

	        if (InBuilding)
		        ExitTower();
	        else
		        StopMoving();
	        targetFood = food;
	        IsBusy = true;

	        _moveByTransform.AssignTransform(food.transform);
	        ArrivedDestinationInstanceEvent += GetFood;
        }

        void GetFood()
        {
	        ArrivedDestinationInstanceEvent -= GetFood;

	        PlayerController.Instance.AddFood(targetFood.Amount, targetFood.transform);
	        targetFood.IsUsed = true;
	        Destroy(targetFood.gameObject);
	        IsBusy = false;

	        _moveByTransform.AssignTransform(building.transform);
	        ArrivedDestinationInstanceEvent += EnterBuilding;
        }

        void EnterBuilding()
        {
	        ArrivedDestinationInstanceEvent -= EnterBuilding;

	        InBuilding = true;
			gameObject.SetActive(false);
        }

        void TargetDiedEvent()
        {
            weapon.TargetDiedEvent -= TargetDiedEvent;

            AttackingWizard = false;
			NowFree();
        }

		// void OnTriggerEnter2D(Collider2D other)
		// {
		// 	var food = other.GetComponent<Food>();
		// 	if (food != null)
		// 	{
		// 		PlayerController.Instance.Food += food.Amount;
		// 		food.IsUsed = true;
		// 		Destroy(food.gameObject);
		// 		ArrivedDestination();
		// 	}
		// }

		protected override void Corpse()
        {
			base.Corpse();

			StopMoving();
        }
    }
}