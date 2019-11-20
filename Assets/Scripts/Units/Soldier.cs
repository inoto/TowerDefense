using System;
using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Soldier : Unit
	{
		public static event Action<Soldier, Building> ChangedBuildingEvent;
		public static event Action<Soldier> FreeSoldierEvent;

		Building _building;

		public bool InBuilding = false;
		public Specialization Specialization;

		[SerializeField] bool Movable = true;
		
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
			if (!IsActive && Movable)
			{
				Init("");
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
			if (_building != null)
				UnAssignFromBuilding();
			
			_building = building;
			building.AddSoldier(this);
			ChangedBuildingEvent?.Invoke(this, _building);

			GetComponent<MoveByTransform>().Init(building.transform);
		}
		
		public void NowFree()
		{
			if (_building != null)
				UnAssignFromBuilding();
			
			FreeSoldierEvent?.Invoke(this);
		}

		void UnAssignFromBuilding()
		{
			_building = null;
			
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
			
			if (_building != null)
			{
				InBuilding = true;
				_building.ActivateSoldier();
			}

			DeInit();
		}
	}
}