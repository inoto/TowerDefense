namespace TowerDefense
{
	public interface ICanAttack
	{
		void AddOrder(IUnitOrder order, bool startImmidiate = true);
		void OrderEnded(IUnitOrder order);
	}
}