using System;
using UnityEngine;

namespace TowerDefense
{
	public interface IAlive
	{
		void RaiseDamagedEvent(int damage, DamageType type);
		void RaiseDiedEvent();
	}
	
	public class Healthy : MonoBehaviour
	{
		IAlive _alive;
		
		public bool IsActive;
		[Space]
		[SerializeField] HealthBar HealthBar;
		[Space]
		public bool Damageable = true;
		public int MaxHealth = 10;
		public int CurrentHealth = 10;
		[Range(0, 1f)] public float HealthPercent = 1f;
		public Armor ArmorType = Armor.None;
		public bool Damaged = false;
		public bool IsDied = false;

		public void Init(IAlive alive)
		{
			_alive = alive;
			Damaged = false;
			IsDied = false;

			ResetHealth();
		}

		void ResetHealth()
		{
			if (CurrentHealth < MaxHealth)
				CurrentHealth = MaxHealth;
			HealthPercent = (float) CurrentHealth / MaxHealth;
			UpdateHealthBar();
		}
		
		void UpdateHealthBar()
		{
			if (HealthBar != null)
			{
				HealthBar.SetPercent(HealthPercent);
				if (HealthPercent >= 1f || HealthPercent <= 0f)
					HealthBar.gameObject.SetActive(false);
				else
					HealthBar.gameObject.SetActive(true);
			}
		}
		
		public void Damage(Weapon weapon)
		{
			if (!Damageable)
				return;
			
			Damage(weapon.Damage, weapon.DamageType);
		}

		public void Damage(int damage, DamageType type = DamageType.Physical)
		{
			if (IsDied || !Damageable)
				return;

			if (type == DamageType.Magical && ArmorType == Armor.Spiritual
			    || type == DamageType.Physical && ArmorType == Armor.Fortified)
				damage = Mathf.RoundToInt(damage * 0.5f);
			
			CurrentHealth = CurrentHealth - damage;
			HealthPercent = (float) CurrentHealth / MaxHealth;
			UpdateHealthBar();
			if (!Damaged && CurrentHealth < MaxHealth)
			{
				Damaged = true;
			}
			if (CurrentHealth <= 0)
			{
				Die();
			}
			
			_alive?.RaiseDamagedEvent(damage, type);
		}
		
		void Die()
		{
			_alive?.RaiseDiedEvent();
		}
	}
}