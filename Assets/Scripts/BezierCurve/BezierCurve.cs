using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense
{
    [ExecuteInEditMode]
	public class BezierCurve : MonoBehaviour
	{
	    [HideInInspector] public BezierCurveCreator Creator;
	    
		[SerializeField][HideInInspector] List<Vector2> Points = new List<Vector2>();
	    bool _isClosed;
	    bool _autoSetControlPoints = true;
		
		public void Init(Vector2 centre, BezierCurveCreator creator)
		{
		    Creator = creator;
			Points = new List<Vector2>
			{
				centre + Vector2.left,
				centre + (Vector2.left + Vector2.up) * .5f,
				centre + (Vector2.right + Vector2.down) * .5f,
				centre + Vector2.right
			};
		}
		
		public Vector2 this[int i]
        {
            get => Points[i];
            set => Points[i] = value;
        }

        public bool IsClosed
        {
            get => _isClosed;
            set
            {
                if (_isClosed == value) return;
                
                _isClosed = value;

                if (_isClosed)
                {
                    Points.Add(Points[Points.Count - 1] * 2 - Points[Points.Count - 2]);
                    Points.Add(Points[0] * 2 - Points[1]);
                    if (!_autoSetControlPoints) return;
                        
                    AutoSetAnchorControlPoints(0);
                    AutoSetAnchorControlPoints(Points.Count - 3);
                }
                else
                {
                    Points.RemoveRange(Points.Count - 2, 2);
                    if (_autoSetControlPoints)
                    {
                        AutoSetStartAndEndControls();
                    }
                }
            }
        }

        public bool AutoSetControlPoints
        {
            get => _autoSetControlPoints;
            set
            {
                if (_autoSetControlPoints == value) return;
                
                _autoSetControlPoints = value;
                if (_autoSetControlPoints)
                {
                    AutoSetAllControlPoints();
                }
            }
        }

        public int NumPoints => Points.Count;

        public int NumSegments => Points.Count / 3;

        public Vector2[] Anchors
        {
            get { return Points.Where((e, i) => i % 3 == 0).ToArray(); }
        }
        
        public Vector2 FirstSegment => Points[0];

        public void AddSegment(Vector2 anchorPos)
        {
            Points.Add(Points[Points.Count - 1] * 2 - Points[Points.Count - 2]);
            Points.Add((Points[Points.Count - 1] + anchorPos) * .5f);
            Points.Add(anchorPos);

            if (_autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(Points.Count - 1);
            }
        }

        public void SplitSegment(Vector2 anchorPos, int segmentIndex)
        {
            Points.InsertRange(segmentIndex * 3 + 2, new Vector2[] {Vector2.zero, anchorPos, Vector2.zero});
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
            if (NumSegments <= 2 && (_isClosed || NumSegments <= 1)) return;
            
            if (anchorIndex == 0)
            {
                if (_isClosed)
                {
                    Points[Points.Count - 1] = Points[2];
                }

                Points.RemoveRange(0, 3);
            }
            else if (anchorIndex == Points.Count - 1 && !_isClosed)
            {
                Points.RemoveRange(anchorIndex - 2, 3);
            }
            else
            {
                Points.RemoveRange(anchorIndex - 1, 3);
            }
        }

        public Vector2[] GetPointsInSegment(int i)
        {
            return new Vector2[] {Points[i * 3], Points[i * 3 + 1], Points[i * 3 + 2], Points[LoopIndex(i * 3 + 3)]};
        }

        public void MovePoint(int i, Vector2 pos)
        {
            Vector2 deltaMove = pos - Points[i];

            if (i % 3 != 0 && _autoSetControlPoints) return;
            
            Points[i] = pos;

            if (_autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(i);
            }
            else
            {

                if (i % 3 == 0)
                {
                    if (i + 1 < Points.Count || _isClosed)
                    {
                        Points[LoopIndex(i + 1)] += deltaMove;
                    }

                    if (i - 1 >= 0 || _isClosed)
                    {
                        Points[LoopIndex(i - 1)] += deltaMove;
                    }
                }
                else
                {
                    bool nextPointIsAnchor = (i + 1) % 3 == 0;
                    int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
                    int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

                    if (correspondingControlIndex >= 0 && correspondingControlIndex < Points.Count || _isClosed)
                    {
                        float dst = (Points[LoopIndex(anchorIndex)] - Points[LoopIndex(correspondingControlIndex)]).magnitude;
                        Vector2 dir = (Points[LoopIndex(anchorIndex)] - pos).normalized;
                        Points[LoopIndex(correspondingControlIndex)] = Points[LoopIndex(anchorIndex)] + dir * dst;
                    }
                }
            }
        }

        public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
        {
            List<Vector2> evenlySpacedPoints = new List<Vector2>();
            evenlySpacedPoints.Add(Points[0]);
            Vector2 previousPoint = Points[0];
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
                if (i >= 0 && i < Points.Count || _isClosed)
                {
                    AutoSetAnchorControlPoints(LoopIndex(i));
                }
            }

            AutoSetStartAndEndControls();
        }

        void AutoSetAllControlPoints()
        {
            for (int i = 0; i < Points.Count; i += 3)
            {
                AutoSetAnchorControlPoints(i);
            }

            AutoSetStartAndEndControls();
        }

        void AutoSetAnchorControlPoints(int anchorIndex)
        {
            Vector2 anchorPos = Points[anchorIndex];
            Vector2 dir = Vector2.zero;
            float[] neighbourDistances = new float[2];

            if (anchorIndex - 3 >= 0 || _isClosed)
            {
                Vector2 offset = Points[LoopIndex(anchorIndex - 3)] - anchorPos;
                dir += offset.normalized;
                neighbourDistances[0] = offset.magnitude;
            }

            if (anchorIndex + 3 >= 0 || _isClosed)
            {
                Vector2 offset = Points[LoopIndex(anchorIndex + 3)] - anchorPos;
                dir -= offset.normalized;
                neighbourDistances[1] = -offset.magnitude;
            }

            dir.Normalize();

            for (int i = 0; i < 2; i++)
            {
                int controlIndex = anchorIndex + i * 2 - 1;
                if (controlIndex >= 0 && controlIndex < Points.Count || _isClosed)
                {
                    Points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * .5f;
                }
            }
        }

        void AutoSetStartAndEndControls()
        {
            if (_isClosed) return;
            
            Points[1] = (Points[0] + Points[2]) * .5f;
            Points[Points.Count - 2] = (Points[Points.Count - 1] + Points[Points.Count - 3]) * .5f;
        }

        int LoopIndex(int i)
        {
            return (i + Points.Count) % Points.Count;
        }

        public Vector2 Lerp(int segment, float time)
        {
            Vector2[] points = GetPointsInSegment(segment);
            Vector2 a = Vector2.Lerp(points[0], points[1], time);
            Vector2 b = Vector2.Lerp(points[1], points[2], time);
            Vector2 c = Vector2.Lerp(points[2], points[3], time);
            Vector2 d = Vector2.Lerp(a, b, time);
            Vector2 e = Vector2.Lerp(b, c, time);
            return Vector2.Lerp(d, e, time);
        }

        // public void TestMove()
        // {
        //     point = FirstSegment;
        //     time = 0f;
        //     segment = 0;
        //     testMoving = true;
        // }
        //
        // bool testMoving;
        // float time = 0f;
        // int segment = 0;
        // void Update()
        // {
        //     if (testMoving)
        //     {
        //         
        //         time += Time.fixedDeltaTime;
        //         point = Lerp(segment, time);
        //         if (time >= 1f)
        //         {
        //             time = 0f;
        //             segment += 1;
        //         }
        //     }
        // }
        //
        // Vector2 point;
        // void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawSphere(point, 0.2f);
        // }
    }
}