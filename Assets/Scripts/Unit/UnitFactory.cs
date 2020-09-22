using System;
using System.Collections.Generic;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace TowerDefense
{
	public class UnitFactory : Singleton<UnitFactory>
	{
		[System.Serializable]
		public enum Type
		{
			Soldier,
			Food
		}

		[System.Serializable]
		public class PreloadObject
		{
			public Type Type;
			public GameObject Prefab;
			public int Amount;
		}

		[SerializeField] List<PreloadObject> objects = new List<PreloadObject>();

		void Start()
		{
			foreach (var obj in objects)
			{
				SimplePool.Preload(obj.Prefab, obj.Amount);
			}
		}

		public T SpawnObject<T>(Type type) where T : class
		{
			var go = SimplePool.Spawn(objects.Find(e => e.Type == type).Prefab);
			go.transform.SetParent(transform);
			return go.GetComponent<T>();
		}
	}
}