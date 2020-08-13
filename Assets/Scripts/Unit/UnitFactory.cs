using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace TowerDefense
{
	public class UnitFactory : Singleton<UnitFactory>
	{
		[SerializeField] PreloadDict preloadUnits = new PreloadDict();

		[SerializeField] GameObject soldierPrefab = null;

		void Start()
		{
			foreach (var kvp in preloadUnits)
			{
				SimplePool.Preload(kvp.Key, kvp.Value);
			}
		}

		public Soldier CreateSoldier()
		{
			var go = SimplePool.Spawn(soldierPrefab);
			go.transform.SetParent(transform);
			return go.GetComponent<Soldier>();
		}
	}

	[System.Serializable]
	public class PreloadDict : SerializableDictionaryBase<GameObject, int> { }
}