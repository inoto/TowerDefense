using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense
{
    public class RoadCreator : BezierCurveCreator
    {
        [Header("RoadCreator")]
        [HideInInspector] public List<Road> Roads = new List<Road>();

        public void AddRoad()
        {
            Road newRoad = Editor.Instantiate(BezierCurvePrefab).GetComponent<Road>();
            newRoad.transform.parent = transform;
            newRoad.transform.position = transform.position;
            newRoad.gameObject.name = DefaultName + newRoad.transform.GetSiblingIndex().ToString();
            newRoad.Init(transform.position, this);
            
            Roads.Add(newRoad);
            
//            if (paths.Count == 1)
//                Selection.activeObject = paths[0].gameObject;
        }

        public void Clear()
        {
            Roads.Clear();
        }

        void Reset()
        {
            AddRoad();
        }
    }
}
