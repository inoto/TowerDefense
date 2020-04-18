using UnityEngine;

namespace TowerDefense
{
    public class BigSkyStone : MonoBehaviour
    {
        [SerializeField] GameObject WizardTemplate;
        public Vector2 SpawnOffset = Vector2.zero;

        void Start()
        {
            SimplePool.Preload(WizardTemplate, 4);
        }

        public void SpawnWizard()
        {
            GameObject go = SimplePool.Spawn(WizardTemplate);
            go.transform.parent = transform;
            go.transform.position = transform.position + (Vector3) SpawnOffset;
        }

        void OnDrawGizmos()
        {
            if (SpawnOffset != Vector2.zero)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)SpawnOffset, 0.1f);
            }
        }
    }
}