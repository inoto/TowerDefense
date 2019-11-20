using System;
using UnityEngine;

namespace TowerDefense
{
	public interface ITargetable
	{
		void Damage(Weapon weapon);
		void Damage(int damage, DamageType type = DamageType.Physical);
		GameObject GameObj { get; }
		bool IsDied { get; }
		Vector2 Position { get; }
		Vector2 Waypoint { get; }
		int PathIndex { get; }
		float Health { get; }
		int MaxHealth { get; }
		Collider2D Collider { get; }
		Vector2 PointToDamage { get; }
	}
}