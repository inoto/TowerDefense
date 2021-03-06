﻿using System;
using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class FloatingText : MonoBehaviour
	{
		[SerializeField] LeanTweenType TweenType = LeanTweenType.easeOutExpo;
		[SerializeField] float MovingTime = 3;
		[SerializeField] float MovingAdditionalYDistance = 30;
		
		Unit attachedUnit;
		
		TextMeshProUGUI _textMesh;
		Image _icon;
		Camera _cameraMain;

		void Awake()
		{
			_textMesh = GetComponentInChildren<TextMeshProUGUI>();
			_icon = GetComponentInChildren<Image>();
			_cameraMain = Camera.main;
		}

		void OnEnable()
		{
			_icon.enabled = false;
		}

		public void Color(Color color) => _textMesh.color = color;

		public void Text(string text) => _textMesh.text = text;

		public void Icon(Sprite sprite, Color color)
		{
			_icon.enabled = true;
			_icon.sprite = sprite;
			_icon.color = color;
		}

		public void StartMoving(float height = 40)
		{
			float halfMovingTime = MovingTime / 2;
//			LeanTween.alpha(TextMesh.gameObject, 0f, halfMovingTime).setDelay(halfMovingTime);
			LeanTween.delayedCall(halfMovingTime / 2, () =>
			{
				_textMesh.CrossFadeAlpha(0f, halfMovingTime, true);
				if (_icon.enabled)
					_icon.CrossFadeAlpha(0f, halfMovingTime, true);
			});
			LeanTween.moveLocalY(gameObject, transform.localPosition.y + MovingAdditionalYDistance, 0f);
			LeanTween.moveLocalY(gameObject, transform.localPosition.y + MovingAdditionalYDistance + height, MovingTime) 
			         .setEase(TweenType)
			         .setOnComplete(() =>
			         {
				         SimplePool.Despawn(gameObject);
			         });
		}

		public void StartMovingAttached(float height = 0, Unit unit = null)
		{
			attachedUnit = unit;
			
			StartMoving(height);
		}

		void Update()
		{
			if (attachedUnit != null)
			{
				transform.position = _cameraMain.WorldToScreenPoint(attachedUnit.transform.position);
				_textMesh.color.SetAlpha(_textMesh.color.a - 2);
			}
		}
	}
}