using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
	[ExecuteInEditMode]
	public class PathCreator : BezierCurveCreator
	{
		[Header("PathCreator")]
		public float MaxPathPointOffset = 0.3f;
		public List<BezierCurve> Curves = new List<BezierCurve>();

		Transform _transform;

		void Awake()
		{
			_transform = GetComponent<Transform>();
		}

		void OnEnable()
		{
			MoveByPath.LookingForPathEvent += ProvidePath;
			// WaveManager.LookingForSpawnPointsEvent += ProvideSpawnPoints;
		}

		void OnDisable()
		{
			MoveByPath.LookingForPathEvent -= ProvidePath;
			// WaveManager.LookingForSpawnPointsEvent -= ProvideSpawnPoints;
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

		// void ProvideSpawnPoints(WaveManager waveManager)
		// {
		// 	waveManager.SpawnPoints = new Dictionary<string, Vector2>();
		// 	for (int i = 0; i < Curves.Count; i++)
		// 	{
		// 		waveManager.SpawnPoints.Add(Curves[i].name, Curves[i].FirstSegment);
		// 	}
		// }

		public void AddCurve()
		{
			BezierCurve newCurve = Editor.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
			newCurve.transform.parent = _transform;
			newCurve.transform.position = _transform.position;
			newCurve.gameObject.name = DefaultName + newCurve.transform.GetSiblingIndex().ToString();
			newCurve.Init(_transform.position, this);
            
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