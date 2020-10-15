using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
	public class DialogMaker : MonoBehaviour
	{
		static List<DialogMaker> Instances = new List<DialogMaker>();

		[SerializeField] string id = "unknown";
		[SerializeField] List<DialogData> dialogs = new List<DialogData>();

		public string Id => id;
		public List<DialogData> Dialogs => dialogs;

		void Awake()
		{
			Instances.Add(this);
		}

		void OnDestroy()
		{
			Instances.Remove(this);
		}

		public static void RunDialogs(string id)
		{
			var instance = Instances.Find(e => e.Id.Equals(id));
			if (instance == null) throw new Exception($"Not found dialog with id {id}");

			UIDialogCloud.Instance.transform.position = instance.transform.position;
			for (int i = 0; i < instance.Dialogs.Count; i++)
			{
				UIDialogCloud.Instance.Show(instance.Dialogs[i]);
			}
		}
	}
}