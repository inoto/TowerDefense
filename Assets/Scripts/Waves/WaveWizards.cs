using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class WaveWizards : Wave
	{
        [SerializeField] float EndDelay = 10f;

		protected override void StartWave()
        {
            base.StartWave();

            StartCoroutine(Process());
        }

        IEnumerator Process()
        {
            foreach (var wizard in WaveData.Wizards)
            {
                wizard.Stone.SpawnWizard();
            }

            yield return new WaitForSeconds(EndDelay);

            EndWave();

            yield return null;
        }
    }
}