using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
	[CustomEditor(typeof(PathCreator),true)]
	[InitializeOnLoad]
	public class PathCreatorViewer : Editor
	{
		[DrawGizmo(GizmoType.InSelectionHierarchy)]
		static void DrawHandles(PathCreator creator, GizmoType gizmoType)
		{
			for (int c = 0; c < creator.Curves.Count; c++)
			{
				for (int i = 0; i < creator.Curves[c].Anchors.Length; i++)
				{
					Handles.CircleHandleCap(0, creator.Curves[c].Anchors[i], Quaternion.identity, creator.Curves[c].Creator.AnchorDiameter/2, EventType.Repaint);
					if (i < creator.Curves[c].Anchors.Length - 1)
						Handles.DrawLine(creator.Curves[c].Anchors[i], creator.Curves[c].Anchors[i + 1]);
				}
			}
		}
	}
}