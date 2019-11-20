namespace TowerDefense
{
	public interface ICanAttack
	{
		void AddOrder(IUnitOrder order);
		void OrderEnded(IUnitOrder order);
	}
}