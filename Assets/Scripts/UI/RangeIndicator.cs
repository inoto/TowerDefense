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
		GameObject collideObj;
		Action<Unit> useAction;
		bool found = false;
		
		TextMeshProUGUI _tipText;
		Camera _cameraMain;

		void Awake()
		{
			_tipText = GetComponentInChildren<TextMeshProUGUI>();
			_cameraMain = Camera.main;
		}

		public void Init(float range, string text, Action<Unit> action)
		{
			useAction = action;
			_tipText.text = text;

			RectTransform rectTrans = GetComponent<RectTransform>();
			rectTrans.sizeDelta = new Vector2(range, range*0.75f);

			collideObj = Instantiate(CollideObjPrefab);
			collideObj.GetComponent<RangeIndicatorCollideObj>().Init(this, range);
		}

		void Update()
		{
			transform.position = Input.mousePosition;
			if (collideObj != null)
			{
				collideObj.transform.position = _cameraMain.ScreenToWorldPoint(transform.position);
			}
			if (Input.GetMouseButtonUp(0))
			{
				// StartCoroutine(DestroyInNextFrame());
				collideObj.GetComponent<RangeIndicatorCollideObj>().Run(useAction);
				Destroy(gameObject);
				HideEvent?.Invoke();
			}
		}

		public static void Show(float range, String text, Action<Unit> action)
		{
			ShowEvent?.Invoke(range, text, action);
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