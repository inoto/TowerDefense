using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace TowerDefense
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(SortingGroup))]
    public class Road : MonoBehaviour
    {
        [HideInInspector] public BezierCurve Curve;
        [SerializeField] MeshFilter Mesh;
        [SerializeField] MeshRenderer MeshRenderer;
        [Range(.05f, 1.5f)] public float Spacing = 1;
        public float RoadWidth = 1;
        public bool AutoUpdate;
        public float Tiling;
        public Texture2D Texture;

        public void Init(Vector2 centre, RoadCreator creator)
        {
            Curve = gameObject.AddComponent<BezierCurve>();
            Curve.Init(centre, creator);
            
            MeshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Transparent"));
            MeshRenderer.sharedMaterial.mainTexture = Texture;
            
            UpdatePath();
        }
        
        public void UpdatePath()
        {
            Vector2[] points = CalculateEvenlySpacedPoints(Spacing);
            Mesh.sharedMesh = CreatePathMesh(points, Curve.IsClosed);
            
            int textureRepeat = Mathf.RoundToInt(Tiling * points.Length * Spacing * .05f);
            MeshRenderer.sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);
        }
        
        Mesh CreatePathMesh(Vector2[] points, bool isClosed)
        {
            Vector3[] verts = new Vector3[points.Length * 2];
            Vector2[] uvs = new Vector2[verts.Length];
            int numTris = 2 * (points.Length - 1) + ((isClosed) ? 2 : 0);
            int[] tris = new int[numTris * 3];
            int vertIndex = 0;
            int triIndex = 0;

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 forward = Vector2.zero;
                if (i < points.Length - 1 || isClosed)
                {
                    forward += points[(i + 1) % points.Length] - points[i];
                }

                if (i > 0 || isClosed)
                {
                    forward += points[i] - points[(i - 1 + points.Length) % points.Length];
                }

                forward.Normalize();
                Vector2 left = new Vector2(-forward.y, forward.x);

                verts[vertIndex] = points[i] + left * RoadWidth * .5f;
                verts[vertIndex + 1] = points[i] - left * RoadWidth * .5f;

                float completionPercent = i / (float) (points.Length - 1);
                //                float v = 1 - Mathf.Abs(2 * completionPercent - 1);
                uvs[vertIndex] = new Vector2(0, completionPercent);
                uvs[vertIndex + 1] = new Vector2(1, completionPercent);

                if (i < points.Length - 1 || isClosed)
                {
                    tris[triIndex] = vertIndex;
                    tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
                    tris[triIndex + 2] = vertIndex + 1;

                    tris[triIndex + 3] = vertIndex + 1;
                    tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
                    tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
                }

                vertIndex += 2;
                triIndex += 6;
            }
            Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uvs;

            return mesh;
        }

        public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
        {
            List<Vector2> evenlySpacedPoints = new List<Vector2>();
            evenlySpacedPoints.Add(Curve[0]);
            Vector2 previousPoint = Curve[0];
            float dstSinceLastEvenPoint = 0;

            for (int segmentIndex = 0; segmentIndex < Curve.NumSegments; segmentIndex++)
            {
                Vector2[] p = Curve.GetPointsInSegment(segmentIndex);
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
    }
}