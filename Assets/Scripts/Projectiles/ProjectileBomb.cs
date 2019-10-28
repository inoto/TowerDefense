using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class ProjectileBomb : Projectile
	{
		[Header("ProjectileBomb")]
		public float SpeedMultiplierMin = 0.9f;
		public float SpeedMultiplierMax = 1.1f;
		public GameObject ExplosionPrefab;

		float duration = 1f;
		
		[SerializeField] AnimationCurve curve;
		Vector3 newPoint;
		Vector3 lookPoint;
		Vector3 endPoint;

		public override void Init(Weapon weapon)
		{
			base.Init(weapon);

			StartCoroutine(Arc());
		}
		
		IEnumerator Arc()
		{
			if (target == null || target.IsDied)
			{
				Destroy(gameObject);
				yield break;
			}

			float time = 0f;

			Vector3 startPoint = _transform.position;
			endPoint = target.Point;

			float linearT, height;
			float angleRad, angleDeg;
			float speedMultiplier = Random.Range(SpeedMultiplierMin, SpeedMultiplierMax);
     
			while (time < duration)
			{
				time += Time.fixedDeltaTime * (1 / Speed) * speedMultiplier;
 
				linearT = time * duration;
				height = curve.Evaluate(linearT);
				//height = Mathf.Lerp(0f, 2.0f, heightT); // change 3 to however tall you want the arc to be

				newPoint = Vector2.Lerp(startPoint, endPoint, linearT) + new Vector2(0f, height);

				if (time < duration * 0.99)
				{
					angleRad = Mathf.Atan2(newPoint.y - _transform.position.y, newPoint.x - _transform.position.x);
					angleDeg = (180 / Mathf.PI) * angleRad;
					_transform.rotation = Quaternion.AngleAxis(angleDeg, Vector3.forward);
				}

				transform.position = newPoint;
 
				yield return new WaitForSeconds(0.01f);
			}

			MakeExplosion();
		}

		void MakeExplosion()
		{
			float damageDelay = 0.1f;
			
			GameObject go = Instantiate(ExplosionPrefab);
			go.transform.position = _transform.position;
			
			WeaponCannon wc = _weapon as WeaponCannon;
			if (wc == null)
				return;
			
			Collider2D[] colliders = new Collider2D[20];
			int count = Physics2D.OverlapCircleNonAlloc(_transform.position, wc.FullDamageRange, colliders);
			for (int i = 0; i < count; i++)
			{
				ITargetable t = colliders[i].GetComponent<ITargetable>();
				if (t != null)
					LeanTween.delayedCall(damageDelay, () => t.Damage(wc.Damage));
			}
//			Debug.Log("colliders count: " + count);
			count = Physics2D.OverlapCircleNonAlloc(_transform.position, wc.SplashDamageRange, colliders);
			for (int i = 0; i < count; i++)
			{
				ITargetable t = colliders[i].GetComponent<ITargetable>();
				if (t != null)
					LeanTween.delayedCall(damageDelay, () => t.Damage(wc.Damage * wc.SplashDamageFactor));
			}
			
			Destroy(gameObject);
		}

		void OnDrawGizmos()
		{
			if (_transform != null && endPoint != null)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(_transform.position, endPoint);
				Gizmos.DrawSphere(endPoint, Vector3.one.x * 0.05f);
			}
		}
	}
}