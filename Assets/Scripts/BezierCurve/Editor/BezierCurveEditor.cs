using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
	[CustomEditor(typeof(BezierCurve))]
	[CanEditMultipleObjects]
	public class BezierCurveEditor : Editor
	{
	    BezierCurve curve;

//	    bool isSelected;

        const float SEGMENT_SELECT_DISTANCE_THRESHOLD = .1f;
        int selectedSegmentIndex = -1;

	    void OnEnable()
        {
            curve = (BezierCurve)target;
        }

	    public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            
            
            EditorGUILayout.HelpBox("move: click LMB on point and drag" + Environment.NewLine +
                                    "add: click shift+LMB on space" + Environment.NewLine +
                                    "remove: click RMB on point", MessageType.Info, true);
            
            curve.IsClosed = EditorGUILayout.Toggle("IsClosed:", curve.IsClosed);
            curve.AutoSetControlPoints = EditorGUILayout.Toggle("AutoSetControlPoints:", curve.AutoSetControlPoints);
            // if (GUILayout.Button("TestMove"))
            // {
            //     _curve.TestMove();
            // }

            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();
        }

        void OnSceneGUI()
        {
            Input();
            Draw(null);
        }

        void Input()
        {
            Event guiEvent = Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            if (guiEvent.type == EventType.MouseDown && guiEvent.shift)
            {
                if (guiEvent.button == 0)
                {
                    if (selectedSegmentIndex != -1)
                    {
                        Undo.RecordObject(curve.Creator, "Split segment");
                        curve.SplitSegment(mousePos, selectedSegmentIndex);
                    }
                    else if (!curve.IsClosed)
                    {
                        Undo.RecordObject(curve.Creator, "Add segment");
                        curve.AddSegment(mousePos);
                    }
                }
            }
            
            // if (guiEvent.type == EventType.MouseDown && guiEvent.control && guiEvent.button == 0)
            // {
            //     if (_selectedSegmentIndex != -1)
            //     {
            //         Undo.RecordObject(_curve.Creator, "Split segment");
            //         _curve.SplitSegment(mousePos, _selectedSegmentIndex);
            //     }
            // }
            
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
            {
                float minDstToAnchor = curve.Creator.AnchorDiameter * .5f;
                int closestAnchorIndex = -1;

                for (int i = 0; i < curve.NumPoints; i += 3)
                {
                    float dst = Vector2.Distance(mousePos, curve[i]);
                    if (dst < minDstToAnchor)
                    {
                        minDstToAnchor = dst;
                        closestAnchorIndex = i;
                    }
                }

                if (closestAnchorIndex != -1)
                {
                    Undo.RecordObject(curve.Creator, "Delete segment");
                    curve.DeleteSegment(closestAnchorIndex);
                }
            }

            if (guiEvent.type == EventType.MouseMove)
            {
                float minDstToSegment = SEGMENT_SELECT_DISTANCE_THRESHOLD;
                int newSelectedSegmentIndex = -1;

                for (int i = 0; i < curve.NumSegments; i++)
                {
                    Vector2[] points = curve.GetPointsInSegment(i);
                    Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
                    float dst = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
                    if (dst < minDstToSegment)
                    {
                        minDstToSegment = dst;
                        newSelectedSegmentIndex = i;
                    }
                }
                
                if (newSelectedSegmentIndex != selectedSegmentIndex)
                {
                    selectedSegmentIndex = newSelectedSegmentIndex;
                    HandleUtility.Repaint();
                } 
            }

            HandleUtility.AddDefaultControl(0);
        }
	    
        void Draw(SceneView sceneView)
        {
            for (int j = 0; j < curve.NumSegments; j++)
            {
                Vector2[] points = curve.GetPointsInSegment(j);
                if (curve.Creator.DisplayControlPoints)
                {
                    Handles.color = Color.black;
                    Handles.DrawLine(points[1], points[0]);
                    Handles.DrawLine(points[2], points[3]);
                }
                Color segmentCol;
//                if (curve.IsSelected)
//                    segmentCol = (j == selectedSegmentIndex && Event.current.shift)
//                        ? curve.Creator.HighlightedSegmentColor
//                        : curve.Creator.SelectedLineColor;
//                else
                    segmentCol = curve.Creator.LineColor;
                Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, 2);
            }

            for (int j = 0; j < curve.NumPoints; j++)
            {
                if (j % 3 == 0 || curve.Creator.DisplayControlPoints)
                {
                    Handles.color = (j % 3 == 0) ? curve.Creator.AnchorColor : curve.Creator.ControlColor;
                    float handleSize = (j % 3 == 0) ? curve.Creator.AnchorDiameter : curve.Creator.ControlDiameter; Vector2 connectionPoint = Vector2.zero;
//                    if (j % 3 == 0 && path.Creator.FindPathToConnect(Paths[i], Paths[i][j], out connectionPoint))
//                    {
//                            creator.connections.Add(new Connection(new Vector2[]{Paths[i][j], connectionPoint}));
//                        Handles.color = path.Creator.ConnectionCol;
//                            if (Handles.Button(Paths[i][j], Quaternion.identity, handleSize, 0, Handles.CylinderHandleCap))
//                        if (Event.current.control && Event.current.button == 1)
//                            Paths[i][j] = connectionPoint;
//                        else if (Event.current.control && Event.current.button == 0)
//                            Paths[i][j] = new Vector2(connectionPoint.x, connectionPoint.y);
//                    }
                    Vector2 newPos = Vector2.zero;
                    newPos = Handles.FreeMoveHandle(curve[j], Quaternion.identity, handleSize, Vector2.zero, Handles.CylinderHandleCap);
                    if (curve[j] != newPos)
                    {
                        Undo.RecordObject(curve.Creator, "Move point");
                        curve.MovePoint(j, newPos);
                        EditorUtility.SetDirty(curve);
                    }
//                        else
//                        {
//                            Handles.CylinderHandleCap(0, Paths[i][j], Quaternion.identity, handleSize, EventType.Repaint);
//                        }
                }
            }
        }
	}
}