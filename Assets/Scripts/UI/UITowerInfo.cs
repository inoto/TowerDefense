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

		[Header("Desired cloud")]
		[SerializeField] TextMeshProUGUI ValueText;
		[SerializeField] TextMeshProUGUI DesiredLabelText;
		[SerializeField] Button[] DesiredButtons;
		[SerializeField] Image[] DesiredButtonsOutline;
		
		[Header("Priority cloud")]
		[SerializeField] TextMeshProUGUI PriorityLabelText;
		[SerializeField] TextMeshProUGUI PriorityText;
		[Tooltip("from Low to High")]
		[SerializeField] Color[] PriorityColors;
		[SerializeField] Button[] PriorityButtons;
		[SerializeField] Image[] PriorityButtonsOutline;
		
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
		
		Transform _transform;
		Tower tower;

		void Awake()
		{
			_transform = GetComponent<Transform>();
		}

		public void Show(LongTapAble longtapAble)
		{
			DesiredLabelText.gameObject.SetActive(false);
			PriorityLabelText.gameObject.SetActive(false);

			tower = longtapAble.GetComponent<Tower>();

			tower.SoldiersCountChangedSingleEvent += UpdateSoldiersCount;
			tower.SoldiersCountChangedSingleEvent += UpdateStats;
			tower.SpecChangedSingleEvent += SpecChanged;
			tower.HideCanvas();

			for (int i = 0; i < DesiredButtons.Length; i++)
				DesiredButtons[i].interactable = false;
			for (int i = 0; i < PriorityButtons.Length; i++)
				PriorityButtons[i].interactable = false;
			ChooseSpecButton.interactable = false;
			ResetSpecButton.interactable = false;

			UpdateSoldiersCount();
			UpdateDesiredCount();
			UpdatePriority();
			UpdateStats();
			UpdateSpec();

			LeanTween.scale(gameObject, Vector3.zero, 0f);
			gameObject.SetActive(true);
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
			for (int i = 0; i < DesiredButtons.Length; i++)
				DesiredButtons[i].interactable = true;
			for (int i = 0; i < PriorityButtons.Length; i++)
				PriorityButtons[i].interactable = true;
			ChooseSpecButton.interactable = true;
			ResetSpecButton.interactable = true;
			
			DesiredLabelText.gameObject.SetActive(true);
			PriorityLabelText.gameObject.SetActive(true);
		}

		public void Hide()
		{
			if (tower != null)
			{
				tower.SoldiersCountChangedSingleEvent -= UpdateSoldiersCount;
				tower.SoldiersCountChangedSingleEvent -= UpdateStats;
				tower.SpecChangedSingleEvent -= SpecChanged;
				tower.ShowCanvas();
				tower = null;
			}

			gameObject.SetActive(false);
			// UIChooseSpecWheel.Instance.Hide();
		}
		
		// using by button
		public void DesiredAdd()
		{
			tower.AddDesired();
			UpdateSoldiersCount();
			UpdateDesiredCount();
		}

		// using by button
		public void DesiredRemove()
		{
			tower.RemoveDesired();
			UpdateSoldiersCount();
			UpdateDesiredCount();
		}

		// using by button
		public void PriorityUp()
		{
			tower.PriorityForDesired += 1;
			UpdatePriority();
		}
		
		// using by button
		public void PriorityDown()
		{
			tower.PriorityForDesired -= 1;
			UpdatePriority();
		}

		void UpdateSoldiersCount()
		{
			ValueText.text = $"{tower.SoldiersCountInBuilding}/{tower.DesiredCount}";
		}
		
		void UpdateDesiredCount()
		{
			if (tower.DesiredCount <= 0)
			{
				DesiredButtonsOutline[1].gameObject.SetActive(false);
			}
			else if (tower.DesiredCount >= tower.MaxDesired)
			{
				DesiredButtonsOutline[0].gameObject.SetActive(false);
			}
			else
			{
				if (!DesiredButtonsOutline[1].gameObject.activeSelf)
					DesiredButtonsOutline[1].gameObject.SetActive(true);
				if (!DesiredButtonsOutline[0].gameObject.activeSelf)
					DesiredButtonsOutline[0].gameObject.SetActive(true);
			}
		}

		void UpdatePriority()
		{
			PriorityText.text = tower.PriorityForDesired.ToString();
			PriorityText.color = PriorityColors[(int)tower.PriorityForDesired];
			if (tower.PriorityForDesired <= SoldiersDispenser.Priority.Low)
			{
				PriorityButtonsOutline[1].gameObject.SetActive(false);
			}
			else if (tower.PriorityForDesired >= SoldiersDispenser.Priority.High)
			{
				PriorityButtonsOutline[0].gameObject.SetActive(false);
			}
			else
			{
				if (!PriorityButtonsOutline[1].gameObject.activeSelf)
					PriorityButtonsOutline[1].gameObject.SetActive(true);
				if (!PriorityButtonsOutline[0].gameObject.activeSelf)
					PriorityButtonsOutline[0].gameObject.SetActive(true);
			}
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

		// using by button
		public void SpecChoose()
		{
			SpecChooseLabelText1.gameObject.SetActive(false);
			// UIChooseSpecWheel.Instance.Show(ChooseSpecButton.transform, tower);
			UILevelControlsManager.Instance.GetControl(UILevelControlsManager.LevelControl.ChooseSpecWheel);
		}
		
		// using by button
		void SpecChanged() // TODO: move to tower, add event for tower to track here
		{
			if (tower.Specialization == Specialization.Type.None)
				return;

			tower.Canvas.SetNoSpecIcon(false);
			SpecIsActive();
			// UIChooseSpecWheel.Instance.Hide();
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
			tower.Canvas.SetNoSpecIcon(true);

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