using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEditor;

namespace TowerDefense
{
	
	
	public class WaveManager : MonoBehaviour
	{
		public static event Action<int> WaveStartedEvent;
		public static event Action<int> WaveEndedEvent;
		public static event Action<Mob, WaveManager> MobSpawnedEvent;
		public static event Action<Wizard, WaveManager> WizardSpawnedEvent; 
		public static event Action<WaveManager> LookingForSpawnPointsEvent;
		
		public bool ShowWaypoints;
		public bool Active = true;
		[ReorderableList]
		public List<WaveData> Waves = new List<WaveData>();
		[SerializeField] List<BezierCurve> paths = new List<BezierCurve>();
		[SerializeField] List<WizardCave> wizardCaves = new List<WizardCave>();

        public float StartDelay = 5f;

        int waveIndex;
		[ShowNativeProperty]
		public WaveData CurrentWave => Waves?[waveIndex];

		void Start()
		{

			Invoke("NextWave", StartDelay);
		}

		void OnDestroy()
		{
			LeanTween.cancel(gameObject);
		}

		void NextWave()
		{
			Invoke("StartWave", Waves[waveIndex].BeforeWaveDelay);
		}

		protected virtual void StartWave()
		{
            WaveStartedEvent?.Invoke(waveIndex);
			Debug.Log($"# Wave # Wave {waveIndex} started");

			float mobsDelay = 0f, wizardsDelay = 0f;
			float overallDelay = 0f;

			for (int i = 0; i < CurrentWave.Mobs.Count; i++)
			{
				mobsDelay += CurrentWave.Mobs[i].Interval * CurrentWave.Mobs[i].MobCount;
				var i1 = i;
				LeanTween.delayedCall(CurrentWave.Mobs[i].Delay, () => SpawnMobs(CurrentWave.Mobs[i1]));
			}

			for (int i = 0; i < CurrentWave.Wizards.Count; i++)
			{
				wizardsDelay += CurrentWave.Wizards[i].Delay;
				var i1 = i;
				LeanTween.delayedCall(CurrentWave.Wizards[i].Delay, () => SpawnWizard(CurrentWave.Wizards[i1]));
			}
			overallDelay = mobsDelay > wizardsDelay ? mobsDelay : wizardsDelay;

			Invoke("EndWave", Waves[waveIndex].AfterWaveDelay + overallDelay);
		}

		protected virtual void EndWave()
		{
            WaveEndedEvent?.Invoke(waveIndex);
			Debug.Log($"# Wave # Wave {waveIndex} ended");

			if (Waves.Count >= waveIndex)
			{
				Debug.Log("# Wave # No more waves");
				return;
			}

			waveIndex += 1;
			Invoke("NextWave", Waves[waveIndex].AfterWaveDelay);
		}

		protected void SpawnMobs(WaveData.MobData mobData)
		{
			StartCoroutine(Spawning(mobData));
		}

		IEnumerator Spawning(WaveData.MobData mobData)
		{
			int mobCounter = 0;
			while (mobCounter < mobData.MobCount)
			{
				yield return new WaitForSeconds(mobData.Interval);
				SpawnMob(mobData.MobPrefab, mobData.PathName);
				mobCounter += 1;
			}
        }

		void SpawnMob(GameObject mobPrefab, string pathName)
		{
			var path = paths.Find(e => e.gameObject.name == pathName);
			Mob mob = SimplePool.Spawn(mobPrefab,
				path.FirstSegment, transform.rotation).GetComponent<Mob>();
			//				mob.transform.position += new Vector3(i % 2 == 0 ? WaveControl.RangeBetweenMobsInGroup.x*i : 0,
			//					i % 2 == 0 ? WaveControl.RangeBetweenMobsInGroup.y*i : 0, 0);

            MoveByPath mbp = mob.GetComponent<MoveByPath>();
            mbp.SetPath(pathName);

            MobSpawnedEvent?.Invoke(mob, this);
		}

		void SpawnWizard(WaveData.WizardData data)
		{
			Wizard wizard = SimplePool.Spawn(data.WizardPrefab).GetComponent<Wizard>();
			wizard.transform.position = wizardCaves[data.CaveIndex].transform.position + (Vector3)wizardCaves[data.CaveIndex].SpawnOffset;

			WizardSpawnedEvent?.Invoke(wizard, this);
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