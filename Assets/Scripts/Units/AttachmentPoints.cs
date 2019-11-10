using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
	
	public class AttachmentPoints : MonoBehaviour
	{
		public PointsDict Points = new PointsDict();

		public Vector2[] GetByText(string text)
		{
			var dict = Points.Where(kvp => kvp.Key.Contains(text)).ToDictionary(i => i.Key, i => i.Value);
			return dict.Values.ToArray();
		}
	}

	[System.Serializable]
	public class PointsDict : SerializableDictionaryBase<string, Vector2> { }
}