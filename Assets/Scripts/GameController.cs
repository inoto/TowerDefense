using System;
using UnityEngine;

namespace TowerDefense
{
	public class GameController : Singleton<GameController>
    {
        public static event Action GameOverEvent;

		public static int MAX_TARGETS_BUFFER = 50;

		public GameConfig Config = null;

        bool isGameOver = false;

        void Start()
        {
	        AncientMagicTree.DiedEvent += GameOver;

	        Application.targetFrameRate = 60;
        }

        public void GameOver()
        {
            isGameOver = true;
            GameOverEvent?.Invoke();

            Time.timeScale = 0f;
        }
    }
}