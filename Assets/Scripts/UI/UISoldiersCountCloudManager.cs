using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class UISoldiersCountCloudManager : MonoBehaviour
	{
		const string AttachmentPointName = "SoldiersCountCloud";

		[SerializeField] GameObject cloudTemplate = null;

		List<UISoldiersCountCloud> clouds = new List<UISoldiersCountCloud>(8);

		void Start()
		{
			cloudTemplate.SetActive(false);

			var buildings = FindObjectsOfType<Building>();
			foreach (var building in buildings)
			{
				var attachmentPoints = building.GetComponent<AttachmentPoints>();
				if (attachmentPoints == null)
					return;
				if (!attachmentPoints.Points.ContainsKey(AttachmentPointName))
					return;

				var go = Instantiate(cloudTemplate, transform);
				go.transform.position = (Vector2)building.transform.position + attachmentPoints.Points[AttachmentPointName];
				clouds.Add(go.GetComponent<UISoldiersCountCloud>());
				clouds[clouds.Count-1].Attach(building);
				go.SetActive(true);
			}
		}
	}
}