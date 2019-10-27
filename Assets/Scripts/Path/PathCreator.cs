using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
	public class PathCreator : BezierCurveCreator
	{
		[Header("PathCreator")]
		public float MaxPathPointOffset = 0.3f;
		public List<BezierCurve> Curves = new List<BezierCurve>();

		void OnEnable()
		{
			Unit.LookingForPathEvent += ProvidePath;
			Wave.LookingForSpawnPointsEvent += ProvideSpawnPoints;
		}

		void OnDisable()
		{
			Unit.LookingForPathEvent -= ProvidePath;
			Wave.LookingForSpawnPointsEvent -= ProvideSpawnPoints;
		}

		void ProvidePath(Unit unit, string pathName)
		{
			for (int i = 0; i < Curves.Count; i++)
			{
				if (Curves[i].gameObject.name == pathName)
				{
					// Debug.Log("# Path # ProvidePath [" + pathName + "] for [" + unit.gameObject.name + "]");
					Vector2[] anchorsWithOffset = new Vector2[Curves[i].Anchors.Length];
					Vector2 offset = new Vector2(Random.Range(-MaxPathPointOffset, MaxPathPointOffset), 
					                             Random.Range(-MaxPathPointOffset, MaxPathPointOffset));
					for (int j = 0; j < anchorsWithOffset.Length; j++)
					{
						anchorsWithOffset[j] = Curves[i].Anchors[j] + offset;
					}
					
					unit.AssignPath(Curves[i] != null ? anchorsWithOffset : null);
				}
			}
		}

		void ProvideSpawnPoints(Wave wave)
		{
			wave.SpawnPoints = new Dictionary<string, Vector2>();
			for (int i = 0; i < Curves.Count; i++)
			{
				wave.SpawnPoints.Add(Curves[i].name, Curves[i].FirstSegment);
			}
		}

		public void AddCurve()
		{
			BezierCurve newCurve = Editor.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
			newCurve.transform.parent = transform;
			newCurve.transform.position = transform.position;
			newCurve.gameObject.name = DefaultName + newCurve.transform.GetSiblingIndex().ToString();
			newCurve.Init(transform.position, this);
            
			Curves.Add(newCurve);
			if (Curves.Count == 1)
				Selection.activeObject = Curves[0].gameObject;
		}

		public void Clear()
		{
			Curves.Clear();
		}
	}
}