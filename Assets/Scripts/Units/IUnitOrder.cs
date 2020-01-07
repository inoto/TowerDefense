using System;

namespace TowerDefense
{
	public interface IUnitOrder
	{
		void Start();
		void Pause();
		string OrderName();
	}
}