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
			MoveByPath.LookingForPathEvent += ProvidePath;
			Wave.LookingForSpawnPointsEvent += ProvideSpawnPoints;
		}

		void OnDisable()
		{
			MoveByPath.LookingForPathEvent -= ProvidePath;
			Wave.LookingForSpawnPointsEvent -= ProvideSpawnPoints;
		}

		void ProvidePath(MoveByPath move, string pathName)
		{
			for (int i = 0; i < Curves.Count; i++)
			{
				if (Curves[i].gameObject.name == pathName)
				{
					move.AssignPath(Curves[i]);
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