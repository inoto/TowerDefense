using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TowerDefense
{
    [CustomEditor(typeof(Road))]
    public class RoadEditor : Editor
    {
        Road _road;
        
        void OnEnable()
        {
            _road = (Road)target;
        }

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//        }

        void OnSceneGUI()
        {
            if (_road.AutoUpdate && Event.current.type == EventType.Repaint)
            {
                _road.UpdatePath();
            }
        }
    }
}
