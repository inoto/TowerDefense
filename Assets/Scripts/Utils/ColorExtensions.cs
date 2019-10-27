using UnityEngine;

namespace TowerDefense
{
	public static class ColorExtensions
	{
		public static void SetAlpha(this Color color, float a)
		{
			color = new Color(color.r, color.g, color.b, a);
		}
	}
}