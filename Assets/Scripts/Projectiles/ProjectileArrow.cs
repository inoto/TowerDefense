using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
	public class ProjectileArrow : Projectile
	{
		[Header("ProjectileArrow")]
		public float SpeedMultiplierMin = 0.9f;
		public float SpeedMultiplierMax = 1.1f;
		public float HeightMultiplierMin = 0.9f;
		public float HeightMultiplierMax = 1.1f;

		const float duration = 1f;
		
		[SerializeField] AnimationCurve curve;
		Vector3 newPoint;
		Vector3 lookPoint;

		public override void Init(Weapon weapon)
		{
			base.Init(weapon);

			if (target == null || target.IsDied)
			{
				Destroy(gameObject);
				return;
			}

			StartCoroutine(Arc());
		}
		
		IEnumerator Arc()
		{
//			Debug.Log("# Arc");
			float time = 0f;

			Vector3 startPoint = trans.position;
			Vector3 endPoint = target.Point;
			float distance = 0f;

			float linearT, height;
			float angleRad, angleDeg;
			float speedMultiplier = Random.Range(SpeedMultiplierMin, SpeedMultiplierMax);
			float heightMultiplier = Random.Range(HeightMultiplierMin, HeightMultiplierMax);
     
			while (time < duration)
			{
				if (!target.IsDied)
				{
					endPoint = target.Point;
					distance = Vector3.Distance(startPoint, endPoint);
				}

				time += Time.fixedDeltaTime * Speed * speedMultiplier / 100;
 
				linearT = time * duration;
				height = curve.Evaluate(linearT);
//				height = Mathf.Lerp(0f, distance, linearT); // change 3 to however tall you want the arc to be

				newPoint = Vector2.Lerp(startPoint, endPoint, linearT) + new Vector2(0f, height * heightMultiplier);
				// Debug.Log(string.Format("linearT: {0} - height: {1} - distance: {2}", linearT, height, distance));

				if (time < duration * 0.99)
				{
					angleRad = Mathf.Atan2(newPoint.y - trans.position.y, newPoint.x - trans.position.x);
					angleDeg = (180 / Mathf.PI) * angleRad;
					trans.rotation = Quaternion.AngleAxis(angleDeg, Vector3.forward);
				}

				transform.position = newPoint;
				
				yield return new WaitForSeconds(0.01f);
			}

			CheckHit();
			
			yield break;
		}

		protected override void CheckHit()
		{
			if (target.Collider.bounds.Contains(trans.position))
			{
				target.Damage(weapon);
				Destroy(gameObject);
			}
			else
			{
				RaiseMissedEvent();
				
				LeanTween.alpha(gameObject, 0f, 2f).setOnComplete(() => Destroy(gameObject));
			}
		}
	}
}