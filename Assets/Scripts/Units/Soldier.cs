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

		[SerializeField] bool Movable = true;
		
		Building building;
		
		protected override void Awake()
		{
			base.Awake();
			
			Specialization = GetComponent<Specialization>();
		}

		void Start()
		{
			StartCoroutine(CheckCreatedManually());
		}

		IEnumerator CheckCreatedManually()
		{
			yield return new WaitForSeconds(0.2f);
			if (!IsActive)
			{
				Init("");
				if (Movable)
					FreeSoldierEvent?.Invoke(this);
			}
		}
		
		public override void Init(string pathName = "")
		{
			base.Init(pathName);
			
			gameObject.SetActive(true);
		}

		public override void DeInit()
		{
			IsActive = false;
			StopAllCoroutines();
			gameObject.SetActive(false);
		}

		public void AssignToBuilding(Building building)
		{
			if (this.building != null)
				UnAssignFromBuilding();
			
			this.building = building;
			building.AddSoldier(this);
			ChangedBuildingEvent?.Invoke(this, this.building);

			GetComponent<MoveByTransform>().Init(building.transform);
		}
		
		public void NowFree()
		{
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
			
			Init("");
		}

		public override void ArrivedDestination()
		{
			base.ArrivedDestination();
			
			if (building != null)
			{
				InBuilding = true;
				building.ActivateSoldier();
			}

			DeInit();
		}
	}
}