using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
	public class WeaponCannon : Weapon
	{
		[Header("WeaponCannon")]
		public float FullDamageRange = 0.5f;
		public float SplashDamageRange = 1f;
		public float SplashDamageFactor = 0.5f;

		//		Collider2D[] colliders = new Collider2D[20];

		//		protected override void OnDrawGizmos()
		//		{
		//			
		//			if (Target != null)
		//			{
		//				Gizmos.color = Color.red;
		//				Vector2 point = (Target.WaypointPoint - Target.Point);
		//				Gizmos.DrawSphere(point+Target.Point, 0.1f);
		//				Gizmos.DrawLine(Target.Point, point+Target.Point);
		//				
		//				GUIStyle style = new GUIStyle();
		//				style.normal.textColor = Color.red;
		//				style.fontSize = 20;
		//				Handles.Label(Target.Point, Target.PathIndex.ToString(), style);
		//			}
		//		}
	}
}