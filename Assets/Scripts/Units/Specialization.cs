using System;
using UnityEditorInternal;
using UnityEngine;

namespace TowerDefense
{
	public class Specialization : MonoBehaviour
	{
		public enum Type
		{
			None,
			Strength,
			Agility,
			Intellect,
			Cunning
		}
		
		public int[] SpecPoints;
		public int[] SpecLevel;
		public int PointsForLevel = 500;

		void Start()
		{
			Init();
		}

		void Init()
		{
			int count = Enum.GetNames(typeof(Type)).Length;
			SpecPoints = new int[count];
			SpecLevel = new int[count];
			
			
		}

		public void Modify(Type spec, int value)
		{
			if (spec == Type.None)
				return;
			
			int index = (int)spec;
			SpecPoints[index] += value;
			if (SpecPoints[index] >= PointsForLevel)
			{
				SpecLevel[index] += 1;
			}
			
			// can reduce other specs here
		}

		[Serializable]
		public class Data
		{
			public Type OfType;
			public string Name;
			public Sprite Icon;
			public string TooltipText;
		}
	}
}