using System;

namespace TowerDefense
{
	public interface IUnitOrder
	{
		void StartOrder();
		void PauseOrder();
	}
}