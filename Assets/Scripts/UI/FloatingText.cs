using System;
using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class FloatingText : MonoBehaviour
	{
		TextMeshProUGUI _textMesh;
		[SerializeField] LeanTweenType TweenType = LeanTweenType.easeOutExpo;
		[SerializeField] float MovingTime = 3;
		[SerializeField] float MovingAdditionalYDistance = 30;
		Unit _attachedUnit;

		void Awake()
		{
			_textMesh = GetComponent<TextMeshProUGUI>();
		}

		public void SetColor(Color color)
		{
			_textMesh.color = color;
		}

		public void SetText(string text)
		{
			_textMesh.text = text;
		}

		public void StartMoving(float height = 0)
		{
			float halfMovingTime = MovingTime / 2;
//			LeanTween.alpha(TextMesh.gameObject, 0f, halfMovingTime).setDelay(halfMovingTime);
			LeanTween.delayedCall(halfMovingTime / 4, () =>
			{
				_textMesh.CrossFadeAlpha(0f, halfMovingTime, true);
			});
//			Vector3 endPoint = new Vector3(transform.position.x + Random.Range(-10f, 10f),
//										transform.position.y + Random.Range(-20f, 20f), 0);
			LeanTween.moveLocalY(gameObject, MovingAdditionalYDistance + height, MovingTime).setEase(TweenType).setOnComplete(() => Destroy(gameObject));
		}

		public void StartMovingAttached(float height = 0, Unit unit = null)
		{
			_attachedUnit = unit;
			
			StartMoving(height);
		}

		void Update()
		{
			if (_attachedUnit != null)
			{
				transform.position = Camera.main.WorldToScreenPoint(_attachedUnit.Point);
				_textMesh.color.SetAlpha(_textMesh.color.a - 2);
			}
		}
	}
}