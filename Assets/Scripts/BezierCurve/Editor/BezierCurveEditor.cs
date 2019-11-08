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
	    BezierCurve _curve;

//	    bool isSelected;

        const float SEGMENT_SELECT_DISTANCE_THRESHOLD = .1f;
        int _selectedSegmentIndex = -1;

	    void OnEnable()
        {
            _curve = (BezierCurve)target;
        }

	    public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            
            
            EditorGUILayout.HelpBox("move: click LMB on point and drag" + Environment.NewLine +
                                    "add: click shift+LMB on space" + Environment.NewLine +
                                    "remove: click RMB on point", MessageType.Info, true);
            
            _curve.IsClosed = EditorGUILayout.Toggle("IsClosed:", _curve.IsClosed);
            _curve.AutoSetControlPoints = EditorGUILayout.Toggle("AutoSetControlPoints:", _curve.AutoSetControlPoints);

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
                    if (_selectedSegmentIndex != -1)
                    {
                        Undo.RecordObject(_curve.Creator, "Split segment");
                        _curve.SplitSegment(mousePos, _selectedSegmentIndex);
                    }
                    else if (!_curve.IsClosed)
                    {
                        Undo.RecordObject(_curve.Creator, "Add segment");
                        _curve.AddSegment(mousePos);
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
                float minDstToAnchor = _curve.Creator.AnchorDiameter * .5f;
                int closestAnchorIndex = -1;

                for (int i = 0; i < _curve.NumPoints; i += 3)
                {
                    float dst = Vector2.Distance(mousePos, _curve[i]);
                    if (dst < minDstToAnchor)
                    {
                        minDstToAnchor = dst;
                        closestAnchorIndex = i;
                    }
                }

                if (closestAnchorIndex != -1)
                {
                    Undo.RecordObject(_curve.Creator, "Delete segment");
                    _curve.DeleteSegment(closestAnchorIndex);
                }
            }

            if (guiEvent.type == EventType.MouseMove)
            {
                float minDstToSegment = SEGMENT_SELECT_DISTANCE_THRESHOLD;
                int newSelectedSegmentIndex = -1;

                for (int i = 0; i < _curve.NumSegments; i++)
                {
                    Vector2[] points = _curve.GetPointsInSegment(i);
                    Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
                    float dst = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
                    if (dst < minDstToSegment)
                    {
                        minDstToSegment = dst;
                        newSelectedSegmentIndex = i;
                    }
                }
                
                if (newSelectedSegmentIndex != _selectedSegmentIndex)
                {
                    _selectedSegmentIndex = newSelectedSegmentIndex;
                    HandleUtility.Repaint();
                } 
            }

            HandleUtility.AddDefaultControl(0);
        }
	    
        void Draw(SceneView sceneView)
        {
            for (int j = 0; j < _curve.NumSegments; j++)
            {
                Vector2[] points = _curve.GetPointsInSegment(j);
                if (_curve.Creator.DisplayControlPoints)
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
                    segmentCol = _curve.Creator.LineColor;
                Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, 2);
            }

            for (int j = 0; j < _curve.NumPoints; j++)
            {
                if (j % 3 == 0 || _curve.Creator.DisplayControlPoints)
                {
                    Handles.color = (j % 3 == 0) ? _curve.Creator.AnchorColor : _curve.Creator.ControlColor;
                    float handleSize = (j % 3 == 0) ? _curve.Creator.AnchorDiameter : _curve.Creator.ControlDiameter; Vector2 connectionPoint = Vector2.zero;
//                    if (j % 3 == 0 && path.Creator.FindPathToConnect(Paths[i], Paths[i][j], out connectionPoint))
//                    {
////                            creator.connections.Add(new Connection(new Vector2[]{Paths[i][j], connectionPoint}));
//                        Handles.color = path.Creator.ConnectionCol;
////                            if (Handles.Button(Paths[i][j], Quaternion.identity, handleSize, 0, Handles.CylinderHandleCap))
//                        if (Event.current.control && Event.current.button == 1)
//                            Paths[i][j] = connectionPoint;
//                        else if (Event.current.control && Event.current.button == 0)
//                            Paths[i][j] = new Vector2(connectionPoint.x, connectionPoint.y);
//                    }
                    Vector2 newPos = Vector2.zero;
                    newPos = Handles.FreeMoveHandle(_curve[j], Quaternion.identity, handleSize, Vector2.zero, Handles.CylinderHandleCap);
                    if (_curve[j] != newPos)
                    {
                        Undo.RecordObject(_curve.Creator, "Move point");
                        _curve.MovePoint(j, newPos);
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