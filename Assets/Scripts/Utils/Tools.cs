using System.Threading;
using System.Timers;
using UnityEngine;

namespace TowerDefense
{
	public class Tools : MonoBehaviour
	{
		public void SetGameSpeed(float s)
		{
			Time.timeScale = s;
			Time.fixedDeltaTime = (s + 1) / 100;
		}

		public void Lag()
		{
			Thread.Sleep(180);
		}
	}
}