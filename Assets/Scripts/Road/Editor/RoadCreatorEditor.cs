using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TowerDefense
{
    [CustomEditor(typeof(RoadCreator))]
    public class RoadCreatorEditor : Editor
    {
        RoadCreator _creator;

        List<Road> Roads
        {
            get { return _creator.Roads; }
        }

        const float SEGMENT_SELECT_DISTANCE_THRESHOLD = .1f;
        
        void OnEnable()
        {
            _creator = (RoadCreator) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add road"))
                {
                    Undo.RecordObject(_creator, "Add road");
                    _creator.AddRoad();
                }
                if (GUILayout.Button("Clear roads"))
                {
                    Undo.RecordObject(_creator, "Clear roads");
                    for (int i = _creator.transform.childCount-1; i >= 0; i--)
                        DestroyImmediate(_creator.transform.GetChild(i).gameObject);
                    _creator.Clear();
                }
            }
            EditorGUILayout.EndHorizontal();

//            for (int i = 0; i < Paths.Count; i++)
//            {
//                EditorGUILayout.BeginVertical("Box");
//                {
//                    EditorGUILayout.BeginHorizontal();
//                    {
//                        GUILayout.Label("[" + i.ToString() + "] " + Paths[i].gameObject.name);
//                        GUILayout.Space(10);
//                        if (GUILayout.Button("X", GUILayout.Width(30)))
//                        {
//                            Undo.RecordObject(creator, "Remove path");
//                            Paths.RemoveAt(i);
//                            DestroyImmediate(Paths[i].gameObject);
//                        }
//                    }
//                    EditorGUILayout.EndHorizontal();
//                }
//                EditorGUILayout.EndVertical();
//            }

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }

        
    }
}
