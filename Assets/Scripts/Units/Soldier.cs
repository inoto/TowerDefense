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
		
		protected override void Awake()
		{
			base.Awake();
			
			Specialization = GetComponent<Specialization>();
		}

		void OnEnable()
		{
			ArrivedDestinationInstanceEvent += ArrivedDestination;
		}

		void OnDisable()
		{
			ArrivedDestinationInstanceEvent -= ArrivedDestination;
		}

		void Start()
		{
			StartCoroutine(CheckCreatedManually());
		}

		IEnumerator CheckCreatedManually()
		{
			yield return new WaitForSeconds(2f);
			if (!IsActive)
			{
				if (FreeSoldierEvent != null)
					FreeSoldierEvent(this);
			}
		}

		public void AssignToBuilding(Building building)
		{
			if (this._building != null)
				UnAssignFromBuilding();
			
			this._building = building;
			building.AddSoldier(this);
			if (ChangedBuildingEvent != null)
				ChangedBuildingEvent(this, this._building);
			
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);
			StartCoroutine(MoveByPoint(building.transform.position));
		}

		public void UnAssignFromBuilding()
		{
			StopMoving();
			_building = null;
			InBuilding = false;
		}

		void ArrivedDestination()
		{
			if (_building != null)
			{
				InBuilding = true;
				_building.ActivateSoldier();
			}

			DeInit();
			gameObject.SetActive(false);
		}
	}
}