using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TowerDefense
{
	
	
	public class Wave : MonoBehaviour
	{
		public static event Action<int> StartedEvent;
		public static event Action<int> EndedEvent;
		public static event Action<Unit, Wave> MobSpawnedEvent;
		public static event Action<Wave> LookingForSpawnPointsEvent;
		
		public bool ShowWaypoints;
		public bool Active = true;
		[NonSerialized] public Dictionary<string, Vector2> SpawnPoints;

        public float StartDelay = 5f;
        public WaveData WaveData;

		int waveNumber;

		void Start()
		{
			LookingForSpawnPointsEvent?.Invoke(this);
		}

		public virtual void InitWave(int waveNumber)
		{
			this.waveNumber = waveNumber;
			Invoke("StartWave", StartDelay);
        }

		protected virtual void StartWave()
		{
            StartedEvent?.Invoke(waveNumber);
			Debug.Log($"# Wave # Wave {waveNumber} started");
		}

		protected virtual void EndWave()
		{
            EndedEvent?.Invoke(waveNumber);
			Debug.Log($"# Wave # Wave {waveNumber} ended");
        }

		protected void StartSpawn(GameObject mobPrefab, int mobCount, float interval, string pathName)
		{
			StartCoroutine(Spawning(mobPrefab, mobCount, interval, pathName));
		}

		IEnumerator Spawning(GameObject mobPrefab, int mobCount, float interval, string pathName)
		{
			int mobCounter = 0;
			while (mobCounter < mobCount)
			{
				float timeElapsed = 0;
				while (timeElapsed < interval)
				{
					timeElapsed += Time.fixedDeltaTime;
					//Debug.Log(Time.deltaTime.ToString());
					yield return null;
				}
				SpawnMob(mobPrefab, pathName);
				mobCounter += 1;
			}
			EndWave();
		}

		void SpawnMob(GameObject mobPrefab, string pathName)
		{
			Mob mob = SimplePool.Spawn(mobPrefab,
				SpawnPoints[pathName], transform.rotation).GetComponent<Mob>();
			//				mob.transform.position += new Vector3(i % 2 == 0 ? WaveControl.RangeBetweenMobsInGroup.x*i : 0,
			//					i % 2 == 0 ? WaveControl.RangeBetweenMobsInGroup.y*i : 0, 0);

            MoveByPath mbp = mob.GetComponent<MoveByPath>();
            mbp.SetPath(pathName);
            // mob.AddOrder(mbp);

            MobSpawnedEvent?.Invoke(mob, this);
		}
		
		void OnDrawGizmos()
		{
			if (ShowWaypoints)
			{
				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.blue;
				style.fontStyle = FontStyle.Bold;

				if (transform.childCount > 0)
				{
					Gizmos.color = Color.blue;
					for (int i = 0; i < transform.childCount - 1; i++)
					{
						Gizmos.DrawSphere(transform.GetChild(i).position, Vector3.one.x * 0.05f);
						Handles.Label(transform.GetChild(i).position, i.ToString(), style);
						Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
					}

					int indexLast = transform.childCount - 1;
					Gizmos.DrawSphere(transform.GetChild(indexLast).position, Vector3.one.x * 0.05f);
					Handles.Label(transform.GetChild(indexLast).position, indexLast.ToString(), style);
				}
			}
		}
	}
}