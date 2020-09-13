using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class UITowerInfo : UILevelControl
	{
		[SerializeField] float ShowAnimationTime = 0.1f;
		[SerializeField] LeanTweenType ShowAnimationTween = LeanTweenType.notUsed;

		[Header("Stats cloud")]
		[SerializeField] TextMeshProUGUI StatsLabelText;
		[SerializeField] TextMeshProUGUI DamageText;
		[SerializeField] TextMeshProUGUI AttackSpeedText;
		[SerializeField] TextMeshProUGUI ResourceText;
		[Header("Spec cloud")]
		[SerializeField] TextMeshProUGUI SpecLabelText;
		[SerializeField] TextMeshProUGUI SpecChooseLabelText1;
		[SerializeField] TextMeshProUGUI SpecChooseLabelText2;
		[SerializeField] Button ChooseSpecButton;
		[SerializeField] TextMeshProUGUI SpecNameText;
		[SerializeField] Image SpecIcon;
		[SerializeField] Button ResetSpecButton;

		public Tower Tower => tower;
		
		Transform _transform;
		Tower tower;
		UISpecChoiceClouds specChoiceControl;

		void Awake()
		{
			_transform = GetComponent<Transform>();
		}

		public void Show(LongTapAble longTapable)
		{
			Owner = longTapable.gameObject;

			this.tower = longTapable.GetComponent<Tower>();

			// tower.SoldiersCountChangedEvent += UpdateSoldiersCount;
			tower.SoldiersCountChangedEvent += UpdateStats;
			tower.SpecChangedSingleEvent += SpecChanged;
			// tower.HideCanvas();

			ChooseSpecButton.interactable = false;
			ChooseSpecButton.onClick.AddListener(SpecChoiceButtonClicked);
			ResetSpecButton.interactable = false;

			// UpdateSoldiersCount();
			UpdateStats();
			UpdateSpec();

			LeanTween.scale(gameObject, Vector3.zero, 0f);
			Show();
			StartShowAnimation();
		}

		void StartShowAnimation()
		{
			_transform.position = tower.transform.position; // TODO: use attachment point
			LeanTween.scale(gameObject, Vector3.one, ShowAnimationTime)
			         .setEase(ShowAnimationTween)
			         .setOnComplete(ShowAnimationCompleted);
		}

		void ShowAnimationCompleted()
		{
			ChooseSpecButton.interactable = true;
			ResetSpecButton.interactable = true;
		}

		public override void Hide()
		{
			if (tower != null)
			{
				// tower.SoldiersCountChangedEvent -= UpdateSoldiersCount;
				tower.SoldiersCountChangedEvent -= UpdateStats;
				tower.SpecChangedSingleEvent -= SpecChanged;
				// tower.ShowCanvas();
				tower = null;
			}
			if (specChoiceControl != null)
				specChoiceControl.Hide();

			base.Hide();
		}

		void UpdateStats()
		{
			if (tower.Damage[1] <= 0)
				DamageText.text = "-";
			else if (tower.Damage[0] == tower.Damage[1])
				DamageText.text = tower.Damage[0].ToString();
			else if (tower.Damage[1] > 0)
				DamageText.text = $"{tower.Damage[0]}-{tower.Damage[1]}";
			
			AttackSpeedText.text = tower.AttackSpeed > 0 ? tower.AttackSpeed.ToString() : "-";
		}

		public void SpecChoiceButtonClicked()
		{
			SpecChooseLabelText1.gameObject.SetActive(false);

			specChoiceControl = UILevelControlsManager.Instance
				.GetControl<UISpecChoiceClouds>(UILevelControlsManager.LevelControl.SpecChoice);
			specChoiceControl.Show(ChooseSpecButton.transform, tower);
		}
		
		// using by button
		void SpecChanged() // TODO: move to tower, add event for tower to track here
		{
			if (tower.Specialization == Specialization.Type.None)
				return;

			// tower.Canvas.SetNoSpecIcon(false);
			SpecIsActive();
			// UISpecChoiceClouds.Instance.Hide();
		}

		void UpdateSpec()
		{
			if (tower.Specialization == Specialization.Type.None)
				SpecIsEmpty();
			else
				SpecIsActive();
		}

		// using by button
		public void SpecReset()
		{
			tower.SetSpec(Specialization.Type.None);
			// tower.Canvas.SetNoSpecIcon(true);

			SpecIsEmpty();
		}
		
		[Button("SpecIsActive")]
		void SpecIsActive()
		{
			ChooseSpecButton.gameObject.SetActive(false);
			SpecChooseLabelText1.gameObject.SetActive(false);
			// SpecChooseLabelText2.gameObject.SetActive(false);
			
			SpecIcon.gameObject.SetActive(true);
			// SpecIcon.sprite = 
			SpecNameText.gameObject.SetActive(true);
			if (tower != null)
				SpecNameText.text = tower.Specialization.ToString().Substring(0, 3);
			else
				SpecNameText.text = "INT";
			ResetSpecButton.gameObject.SetActive(true);
		}

		[Button("SpecIsEmpty")]
		void SpecIsEmpty()
		{
			ChooseSpecButton.gameObject.SetActive(true);
			SpecChooseLabelText1.gameObject.SetActive(true);
			// SpecChooseLabelText2.gameObject.SetActive(true);
			
			SpecIcon.gameObject.SetActive(false);
			SpecNameText.gameObject.SetActive(false);
			ResetSpecButton.gameObject.SetActive(false);
		}
	}
}