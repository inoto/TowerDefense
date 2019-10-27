using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TowerDefense
{
    [CustomEditor(typeof(PathCreator))]
    public class PathCreatorEditor : Editor
    {
        PathCreator _creator;

        List<BezierCurve> Paths
        {
            get { return _creator.Curves; }
        }

        const float SEGMENT_SELECT_DISTANCE_THRESHOLD = .1f;

        void OnEnable()
        {
            _creator = (PathCreator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            
            
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add curve"))
                {
                    Undo.RecordObject(_creator, "Add curve");
                    _creator.AddCurve();
                }

                if (GUILayout.Button("Clear curves"))
                {
                    Undo.RecordObject(_creator, "Clear curves");
                    for (int i = _creator.transform.childCount - 1; i >= 0; i--)
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