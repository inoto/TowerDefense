using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
	[CustomEditor(typeof(AttachmentPoints))]
	[CanEditMultipleObjects]
	public class AttachmentPointsEditor : Editor
	{
		static float handleSize = 0.075f;
		static Color moveHandleColor = Color.red;
		static Color labelColor = Color.blue;

		void OnSceneGUI()
		{
			AttachmentPoints ap = (AttachmentPoints)target;

			GUIStyle style = new GUIStyle();
			style.alignment = TextAnchor.UpperCenter;
			style.normal.textColor = labelColor;
			
			PointsDict copy = new PointsDict();
			ap.Points.CopyTo(copy);

			Vector2 pos = ap.transform.position;

			foreach (var kvp in copy)
			{
				Handles.color = moveHandleColor;
				Vector2 point = kvp.Value + pos;
				Vector2 newPos = Handles.FreeMoveHandle(point, Quaternion.identity, handleSize, Vector2.zero, Handles.CylinderHandleCap);

				Handles.Label(point, kvp.Key.ToString(), style);
				
				if (point != newPos)
				{
					Undo.RecordObject(ap, "Move point");
					ap.Points[kvp.Key] = newPos - pos;
				}
			}
		}
	}
}