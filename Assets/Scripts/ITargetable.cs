using System;
using UnityEngine;

namespace TowerDefense
{
	public interface ITargetable
	{
		void Damage(Weapon weapon);
		void Damage(float damage, DamageType type = DamageType.Physical);
		GameObject GameObj { get; }
		bool IsDied { get; }
		Vector2 Point { get; }
		Vector2 WaypointPoint { get; }
		int PathIndex { get; }
		float Health { get; }
		int MaxHealth { get; }
		Collider2D Collider { get; }
		Vector2 PointToDamage { get; }
		void InRangeByWeapon(Weapon weapon);
		void NotInRangeByWeapon(Weapon weapon);
	}
}