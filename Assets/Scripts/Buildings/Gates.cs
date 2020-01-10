using System.Collections;
using UnityEngine;

namespace TowerDefense
{
	public class Gates : MonoBehaviour, IAlive
	{
		void Start()
		{
			GetComponent<Healthy>().Init(this);
		}

		void Corpse()
		{
			LeanTween.alpha(gameObject, 0f, 2f).setOnComplete(() => Destroy(gameObject));
		}

#region IAlive

		public void RaiseDamagedEvent(int damage, DamageType type)
		{
			// DamagedEvent?.Invoke(this, damage, type);
		}

		public void RaiseDiedEvent()
		{
			Corpse();
			// DiedEvent?.Invoke(this);
			// DiedInstanceEvent?.Invoke();
		}
		
#endregion
	}
}