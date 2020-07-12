using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomToolbar
{
    [InitializeOnLoad]
    public static class RightToolbarPanel
    {
        static RightToolbarPanel()
        {
            CustomToolbar.RightToolbarGUI.Add(OnToolbarGUI);
        }
        static void OnToolbarGUI()
        {
            EditorGUIUtility.SetIconSize(new Vector2(17,17));
            if (GUILayout.Button("R", ToolbarStyles.commandButtonStyle))
            {
                if (EditorApplication.isPlaying)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }

            Time.timeScale = EditorGUILayout.Slider("", Time.timeScale, 1, 20,
                GUILayout.Width(150));
        }
    }
}
