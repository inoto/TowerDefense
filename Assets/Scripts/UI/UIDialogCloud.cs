using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	[System.Serializable]
	public class DialogData
	{
		public string Name;
		public Sprite Portrait;
		public string Text;
		public float Duration;
		public UIDialogCloud.EPosition Position;
	}

	public class UIDialogCloud : Singleton<UIDialogCloud>
	{
		public enum EPosition
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}

		public event Action<UIDialogCloud> CompletedEvent;
		public event Action<UIDialogCloud> HiddenEvent;
		public event Action<UIDialogCloud> ShownEvent;

		[SerializeField] float animationDuration = 0.5f;
		[SerializeField] LeanTweenType animationEase = LeanTweenType.notUsed;
		[SerializeField] RectTransform positionControl = null;

		[Space]
		[SerializeField] float textSpeed = 1f;

		[Space]
		[SerializeField] TextMeshProUGUI nameTMP = null;
		[SerializeField] Image portrait = null;
		[SerializeField] TextMeshProUGUI textTMP = null;

		bool shown = false;
		int currentIndex = -1;
		EPosition currentPosition;
		bool completed = false;
		DialogData current;
		Queue<DialogData> queue = new Queue<DialogData>(5);

		public void Show(DialogData data, bool clearQueue = false)
		{
			if (clearQueue)
				queue.Clear();

			queue.Enqueue(data);
			if (queue.Count == 1)
			{
				current = data;
				if (shown)
				{
					SetCurrentData();
					StartCurrentData();
				}
				else
					StartShowAnimation();
			}
		}

		void ShowNext()
		{
			var oldPosition = current.Position;
			current = queue.Peek();
			if (oldPosition != current.Position)
			{
				Hide();
				HiddenEvent += OnHiddenEvent_Show;
			}
			else
			{
				SetCurrentData();
				StartCurrentData();
			}
		}

		void OnHiddenEvent_Show(UIDialogCloud cloud)
		{
			HiddenEvent -= OnHiddenEvent_Show;
			StartShowAnimation();
		}

		void SetCurrentData()
		{
			completed = false;
			nameTMP.text = $"{current.Name}";
			portrait.sprite = current.Portrait;
		}

		void StartCurrentData()
		{
			StartCoroutine(CheckDuration());
			StartCoroutine(ShowText());
		}

		void StartShowAnimation()
		{
			LeanTween.cancel(gameObject);
			StopAllCoroutines();

			transform.localScale = Vector3.zero;
			SetCurrentData();
			UpdatePosition();

			gameObject.SetActive(true);

			LeanTween.scale(gameObject, Vector3.one, animationDuration).setEase(animationEase).setOnComplete(() =>
			{
				shown = true;
				ShownEvent?.Invoke(this);
				StartCurrentData();
			});
		}

		void Hide()
		{
			LeanTween.scale(gameObject, Vector3.zero, animationDuration).setEase(animationEase).setOnComplete(() =>
			{
				shown = false;
				HiddenEvent?.Invoke(this);
				// gameObject.SetActive(false);
			});
		}

		void UpdatePosition()
		{
			switch (current.Position)
			{
				case EPosition.TopLeft:
					positionControl.anchorMin = positionControl.anchorMax = new Vector2(0, 1);
					positionControl.anchoredPosition = new Vector2(Mathf.Abs(positionControl.anchoredPosition.x),
						-Mathf.Abs(positionControl.anchoredPosition.y));
					break;
				case EPosition.TopRight:
					positionControl.anchorMin = positionControl.anchorMax = Vector2.one;
					positionControl.anchoredPosition = new Vector2(-Mathf.Abs(positionControl.anchoredPosition.x),
						-Mathf.Abs(positionControl.anchoredPosition.y));
					break;
			}
		}

		IEnumerator ShowText()
		{
			textTMP.text = "";
			currentIndex = 0;
			while (textTMP.text.Length < current.Text.Length)
			{
				textTMP.text += current.Text[currentIndex++];
				yield return new WaitForSeconds(1 / textSpeed * Time.deltaTime);
			}
		}

		IEnumerator CheckDuration()
		{
			yield return new WaitForSeconds(current.Duration);
			Complete();
		}

		void Update()
		{
			if (Input.anyKey)
			{
				Complete();
			}
		}

		void Complete()
		{
			StopAllCoroutines();
			textTMP.text = current.Text;
			completed = true;
			queue.Dequeue();
			CompletedEvent?.Invoke(this);

			if (queue.Count > 0)
				ShowNext();
			else
				Hide();
		}
	}
}