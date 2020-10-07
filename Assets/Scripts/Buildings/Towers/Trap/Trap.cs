using System;
using UnityEngine;

namespace TowerDefense
{
	public class Trap : MonoBehaviour
	{
		public event Action<Trap> FullyUsedEvent;

		public TrapPlace TrapPlace;

		protected void RaiseFullyUsedEvent()
		{
			TrapPlace.Free();
			FullyUsedEvent?.Invoke(this);
		}
	}
}