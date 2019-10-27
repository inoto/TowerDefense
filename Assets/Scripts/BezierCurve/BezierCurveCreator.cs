using UnityEngine;

namespace TowerDefense
{
	public class BezierCurveCreator : MonoBehaviour
	{
		[Header("BezierCurveCreator")]
		public GameObject BezierCurvePrefab;
		public string DefaultName;

		public Color AnchorColor = Color.red;
		public Color ControlColor = Color.magenta;
		public Color LineColor = Color.white;
		public Color SelectedLineColor = Color.green;
		public Color HighlightedSegmentColor = Color.yellow;
		public float AnchorDiameter = .2f;
		public float ControlDiameter = .125f;
		public bool DisplayControlPoints = true;
	}
}