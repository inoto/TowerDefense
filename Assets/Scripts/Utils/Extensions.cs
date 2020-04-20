using UnityEngine;

namespace TowerDefense
{
	public static class Extensions
	{
		public static Color SetAlpha(this Color color, float a)
		{
			return new Color(color.r, color.g, color.b, a);
		}
	}
}