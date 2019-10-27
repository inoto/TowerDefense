using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
	public class BezierCurve : MonoBehaviour
	{
	    [HideInInspector] public BezierCurveCreator Creator;
	    
		[SerializeField][HideInInspector] List<Vector2> points = new List<Vector2>();
	    bool _isClosed;
	    bool _autoSetControlPoints;
		
		public void Init(Vector2 centre, BezierCurveCreator creator)
		{
		    Creator = creator;
			points = new List<Vector2>
			{
				centre + Vector2.left,
				centre + (Vector2.left + Vector2.up) * .5f,
				centre + (Vector2.right + Vector2.down) * .5f,
				centre + Vector2.right
			};
		}
		
		public Vector2 this[int i]
        {
            get { return points[i]; }
            set { points[i] = value; }
        }

        public bool IsClosed
        {
            get { return _isClosed; }
            set
            {
                if (_isClosed != value)
                {
                    _isClosed = value;

                    if (_isClosed)
                    {
                        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
                        points.Add(points[0] * 2 - points[1]);
                        if (_autoSetControlPoints)
                        {
                            AutoSetAnchorControlPoints(0);
                            AutoSetAnchorControlPoints(points.Count - 3);
                        }
                    }
                    else
                    {
                        points.RemoveRange(points.Count - 2, 2);
                        if (_autoSetControlPoints)
                        {
                            AutoSetStartAndEndControls();
                        }
                    }
                }
            }
        }

        public bool AutoSetControlPoints
        {
            get { return _autoSetControlPoints; }
            set
            {
                if (_autoSetControlPoints != value)
                {
                    _autoSetControlPoints = value;
                    if (_autoSetControlPoints)
                    {
                        AutoSetAllControlPoints();
                    }
                }
            }
        }

        public int NumPoints
        {
            get { return points.Count; }
        }

        public int NumSegments
        {
            get { return points.Count / 3; }
        }

        public Vector2[] Anchors
        {
            get { return points.Where((e, i) => i % 3 == 0).ToArray(); }
        }
        
        public Vector2 FirstSegment
        {
            get { return points[0]; }
        }

        public void AddSegment(Vector2 anchorPos)
        {
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add((points[points.Count - 1] + anchorPos) * .5f);
            points.Add(anchorPos);

            if (_autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(points.Count - 1);
            }
        }

        public void SplitSegment(Vector2 anchorPos, int segmentIndex)
        {
            points.InsertRange(segmentIndex * 3 + 2, new Vector2[] {Vector2.zero, anchorPos, Vector2.zero});
            if (_autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(segmentIndex * 3 + 3);
            }
            else
            {
                AutoSetAnchorControlPoints(segmentIndex * 3 + 3);
            }
        }

        public void DeleteSegment(int anchorIndex)
        {
            if (NumSegments > 2 || !_isClosed && NumSegments > 1)
            {
                if (anchorIndex == 0)
                {
                    if (_isClosed)
                    {
                        points[points.Count - 1] = points[2];
                    }

                    points.RemoveRange(0, 3);
                }
                else if (anchorIndex == points.Count - 1 && !_isClosed)
                {
                    points.RemoveRange(anchorIndex - 2, 3);
                }
                else
                {
                    points.RemoveRange(anchorIndex - 1, 3);
                }
            }
        }

        public Vector2[] GetPointsInSegment(int i)
        {
            return new Vector2[] {points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)]};
        }

        public void MovePoint(int i, Vector2 pos)
        {
            Vector2 deltaMove = pos - points[i];

            if (i % 3 == 0 || !_autoSetControlPoints)
            {
                points[i] = pos;

                if (_autoSetControlPoints)
                {
                    AutoSetAllAffectedControlPoints(i);
                }
                else
                {

                    if (i % 3 == 0)
                    {
                        if (i + 1 < points.Count || _isClosed)
                        {
                            points[LoopIndex(i + 1)] += deltaMove;
                        }

                        if (i - 1 >= 0 || _isClosed)
                        {
                            points[LoopIndex(i - 1)] += deltaMove;
                        }
                    }
                    else
                    {
                        bool nextPointIsAnchor = (i + 1) % 3 == 0;
                        int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
                        int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

                        if (correspondingControlIndex >= 0 && correspondingControlIndex < points.Count || _isClosed)
                        {
                            float dst = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControlIndex)]).magnitude;
                            Vector2 dir = (points[LoopIndex(anchorIndex)] - pos).normalized;
                            points[LoopIndex(correspondingControlIndex)] = points[LoopIndex(anchorIndex)] + dir * dst;
                        }
                    }
                }
            }
        }

        public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
        {
            List<Vector2> evenlySpacedPoints = new List<Vector2>();
            evenlySpacedPoints.Add(points[0]);
            Vector2 previousPoint = points[0];
            float dstSinceLastEvenPoint = 0;

            for (int segmentIndex = 0; segmentIndex < NumSegments; segmentIndex++)
            {
                Vector2[] p = GetPointsInSegment(segmentIndex);
                float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
                float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLength / 2f;
                int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
                float t = 0;
                while (t <= 1)
                {
                    t += 1f / divisions;
                    Vector2 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                    dstSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

                    while (dstSinceLastEvenPoint >= spacing)
                    {
                        float overshootDst = dstSinceLastEvenPoint - spacing;
                        Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                        evenlySpacedPoints.Add(newEvenlySpacedPoint);
                        dstSinceLastEvenPoint = overshootDst;
                        previousPoint = newEvenlySpacedPoint;
                    }

                    previousPoint = pointOnCurve;
                }
            }

            return evenlySpacedPoints.ToArray();
        }


        void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
        {
            for (int i = updatedAnchorIndex - 3; i <= updatedAnchorIndex + 3; i += 3)
            {
                if (i >= 0 && i < points.Count || _isClosed)
                {
                    AutoSetAnchorControlPoints(LoopIndex(i));
                }
            }

            AutoSetStartAndEndControls();
        }

        void AutoSetAllControlPoints()
        {
            for (int i = 0; i < points.Count; i += 3)
            {
                AutoSetAnchorControlPoints(i);
            }

            AutoSetStartAndEndControls();
        }

        void AutoSetAnchorControlPoints(int anchorIndex)
        {
            Vector2 anchorPos = points[anchorIndex];
            Vector2 dir = Vector2.zero;
            float[] neighbourDistances = new float[2];

            if (anchorIndex - 3 >= 0 || _isClosed)
            {
                Vector2 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
                dir += offset.normalized;
                neighbourDistances[0] = offset.magnitude;
            }

            if (anchorIndex + 3 >= 0 || _isClosed)
            {
                Vector2 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
                dir -= offset.normalized;
                neighbourDistances[1] = -offset.magnitude;
            }

            dir.Normalize();

            for (int i = 0; i < 2; i++)
            {
                int controlIndex = anchorIndex + i * 2 - 1;
                if (controlIndex >= 0 && controlIndex < points.Count || _isClosed)
                {
                    points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * .5f;
                }
            }
        }

        void AutoSetStartAndEndControls()
        {
            if (!_isClosed)
            {
                points[1] = (points[0] + points[2]) * .5f;
                points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * .5f;
            }
        }

        int LoopIndex(int i)
        {
            return (i + points.Count) % points.Count;
        }
	}
}