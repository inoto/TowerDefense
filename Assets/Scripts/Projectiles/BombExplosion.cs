using System;
using UnityEngine;

namespace TowerDefense
{
	public class BombExplosion : MonoBehaviour
	{
		void OnDisable()
		{
			SimplePool.Despawn(gameObject);
		}
	}
}