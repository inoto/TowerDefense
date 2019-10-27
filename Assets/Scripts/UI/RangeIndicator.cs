using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class RangeIndicator : MonoBehaviour
	{
		public static event Action HideEvent;
		public static event Action<float, String, Action<Unit>> ShowEvent;
		
		[SerializeField] GameObject CollideObjPrefab;
		GameObject _collideObj;
		TextMeshProUGUI _tipText;
		Action<Unit> _useAction;

		bool found = false;

		void Awake()
		{
			_tipText = GetComponentInChildren<TextMeshProUGUI>();
		}

		public void Init(float range, string text, Action<Unit> action)
		{
			_useAction = action;
			_tipText.text = text;

			RectTransform rectTrans = GetComponent<RectTransform>();
			rectTrans.sizeDelta = new Vector2(range, range*0.75f);

			_collideObj = Instantiate(CollideObjPrefab);
			_collideObj.GetComponent<RangeIndicatorCollideObj>().Init(this, range);
		}

		void Update()
		{
			transform.position = Input.mousePosition;
			if (_collideObj != null)
			{
				_collideObj.transform.position = Camera.main.ScreenToWorldPoint(transform.position);
			}
			if (Input.GetMouseButtonUp(0))
			{
				// StartCoroutine(DestroyInNextFrame());
				_collideObj.GetComponent<RangeIndicatorCollideObj>().Run(_useAction);
				Destroy(gameObject);
				if (HideEvent != null)
					HideEvent();
			}
		}

		public static void Show(float range, String text, Action<Unit> action)
		{
			if (ShowEvent != null)
				ShowEvent(range, text, action);
		}

		IEnumerator DestroyInNextFrame()
		{
			int counter = 0;
			while (true)
			{
				yield return new WaitForEndOfFrame();
				counter++;
				if (found || counter == 2)
				{
					yield return new WaitForEndOfFrame();
					Destroy(gameObject);
					

					yield break;
				}
			}
		}
	}
}