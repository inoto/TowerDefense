using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class RangeIndicatorCollideObj : MonoBehaviour
	{
		[SerializeField] RangeIndicator RangeIndicator;

		bool isActive = false;
		bool found = false;
		Action<Unit> useAction;
		
		public void Init(RangeIndicator rangeIndicator, float range)
		{
			
			this.RangeIndicator = rangeIndicator;

			float rangeCollider = range / 100;
			GetComponent<EllipseCollider2D>().RadiusX = rangeCollider;
			GetComponent<EllipseCollider2D>().RadiusY = rangeCollider * 0.75f;
		}

		public void Run(Action<Unit> action)
		{
			useAction = action;
			isActive = true;
			StartCoroutine(DestroyInNextFrame());
		}

		void OnTriggerStay2D(Collider2D other)
		{
			if (!isActive)
				return;
			
			Unit unit = other.gameObject.GetComponent<Unit>();
			if (unit != null)
			{
				useAction(unit);
				found = true;
			}
		}

		IEnumerator DestroyInNextFrame()
		{
			int counter = 0;
			while (true)
			{
				yield return new WaitForEndOfFrame();
				// if make less than 3 then no trigger something
				if (counter > 3 || found)
				{
					Destroy(gameObject);
					yield break;
				}
				counter++;
			}
		}
	}
}