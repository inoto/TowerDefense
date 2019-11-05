using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TowerInfo : Singleton<TowerInfo>
	{
		Transform _transform;
		Tower _tower;

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

		protected override void Awake()
		{
			base.Awake();
			
			_transform = GetComponent<Transform>();
		}

		void Start()
		{
			DesiredLabelText.gameObject.SetActive(false);
			PriorityLabelText.gameObject.SetActive(false);
			Hide();
			Selectable.TowerClickedEvent += Show;
		}

		void Show(Tower tower)
		{
			_tower = tower;

			_tower.SoldiersCountChangedSingleEvent += UpdateSoldiersCount;
			_tower.SoldiersCountChangedSingleEvent += UpdateStats;
			_tower.SpecChangedSingleEvent += SpecChanged;
			_tower.HideCanvas();
			
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
			_transform.position = _tower.transform.position; // TODO: use attachment point
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
			if (_tower != null)
			{
				_tower.SoldiersCountChangedSingleEvent -= UpdateSoldiersCount;
				_tower.SoldiersCountChangedSingleEvent -= UpdateStats;
				_tower.SpecChangedSingleEvent -= SpecChanged;
				_tower.ShowCanvas();
				_tower = null;
			}

			gameObject.SetActive(false);
			ChooseSpecWheel.Instance.Hide();
		}
		
		// using by button
		public void DesiredAdd()
		{
			_tower.AddDesired();
			UpdateSoldiersCount();
			UpdateDesiredCount();
		}

		// using by button
		public void DesiredRemove()
		{
			Debug.Log("DesiredRemove press");
			_tower.RemoveDesired();
			UpdateSoldiersCount();
			UpdateDesiredCount();
			Debug.Log("DesiredRemove end");
		}

		// using by button
		public void PriorityUp()
		{
			_tower.PriorityForDesired += 1;
			UpdatePriority();
		}
		
		// using by button
		public void PriorityDown()
		{
			_tower.PriorityForDesired -= 1;
			UpdatePriority();
		}

		void UpdateSoldiersCount()
		{
			ValueText.text = $"{_tower.SoldiersCountInBuilding}/{_tower.DesiredCount}";
		}
		
		void UpdateDesiredCount()
		{
			if (_tower.DesiredCount <= 0)
			{
				DesiredButtonsOutline[1].gameObject.SetActive(false);
			}
			else if (_tower.DesiredCount >= _tower.MaxDesired)
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
			PriorityText.text = _tower.PriorityForDesired.ToString();
			PriorityText.color = PriorityColors[(int)_tower.PriorityForDesired];
			if (_tower.PriorityForDesired <= SoldiersDispenser.Priority.Low)
			{
				PriorityButtonsOutline[1].gameObject.SetActive(false);
			}
			else if (_tower.PriorityForDesired >= SoldiersDispenser.Priority.High)
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
			if (_tower.Damage[1] <= 0)
				DamageText.text = "-";
			else if (_tower.Damage[0] == _tower.Damage[1])
				DamageText.text = _tower.Damage[0].ToString();
			else if (_tower.Damage[1] > 0)
				DamageText.text = $"{_tower.Damage[0]}-{_tower.Damage[1]}";
			
			AttackSpeedText.text = _tower.AttackSpeed > 0 ? _tower.AttackSpeed.ToString() : "-";
		}

		// using by button
		public void SpecChoose()
		{
			SpecChooseLabelText1.gameObject.SetActive(false);
			ChooseSpecWheel.Instance.Show(ChooseSpecButton.transform, _tower);
		}
		
		// using by button
		void SpecChanged() // TODO: move to tower, add event for tower to track here
		{
			if (_tower.Specialization == Specialization.Type.None)
				return;

			_tower.Canvas.SetNoSpecIcon(false);
			SpecIsActive();
			ChooseSpecWheel.Instance.Hide();
		}

		void UpdateSpec()
		{
			if (_tower.Specialization == Specialization.Type.None)
				SpecIsEmpty();
			else
				SpecIsActive();
		}

		// using by button
		public void SpecReset()
		{
			_tower.SetSpec(Specialization.Type.None);
			_tower.Canvas.SetNoSpecIcon(true);

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
			if (_tower != null)
				SpecNameText.text = _tower.Specialization.ToString().Substring(0, 3);
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