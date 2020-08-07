using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class FPSCounter : MonoBehaviour
	{
		[SerializeField] TextAnchor alignment = TextAnchor.UpperLeft;
		[SerializeField] int fontSize = 25;
		[SerializeField] Color textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

		float deltaTime = 0.0f;
		GUIStyle style = new GUIStyle();

		void Update()
		{
			deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		}

		void OnGUI()
		{
			int w = Screen.width, h = Screen.height;

			Rect rect = new Rect(0, 0, w, h * 2 / 100);
			style.alignment = alignment;
			style.fontSize = fontSize;
			style.normal.textColor = textColor;
			float msec = deltaTime * 1000.0f;
			float fps = 1.0f / deltaTime;
			string text = $"{msec:0.0} ms ({fps:0.} fps)";
			GUI.Label(rect, text, style);
		}
	}
}