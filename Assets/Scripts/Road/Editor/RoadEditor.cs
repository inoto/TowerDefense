using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TowerDefense
{
    [CustomEditor(typeof(Road))]
    public class RoadEditor : Editor
    {
        Road road;
        
        void OnEnable()
        {
            road = (Road)target;
        }

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//        }

        void OnSceneGUI()
        {
            if (road.AutoUpdate && Event.current.type == EventType.Repaint)
            {
                road.UpdatePath();
            }
        }
    }
}
