using __SCRIPTS.Cursor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace __SCRIPTS
{
	public class AimAbility_FX : MonoBehaviour
	{
		IAimAbility aimAbility;
		Body body;
		public Light2D flashlightLight;
		Life life;

		void Start()
		{
			life = GetComponent<Life>();
			if (life == null) return;
			aimAbility = GetComponent<IAimAbility>();
			body = GetComponent<Body>();
		}

		void Update()
		{
			if (Services.pauseManager.IsPaused) return;
			if (aimAbility.hasEnoughMagnitude()) AimFlashlight();
		}

		RaycastHit2D CheckRaycastHit(Vector3 targetDirection, LayerMask layer)
		{
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, life.Stats.Range(1), layer);
			return raycastHit;
		}

		void AimFlashlight()
		{
			var hitPoint = CheckRaycastHit(aimAbility.AimDir, Services.assetManager.LevelAssets.BuildingLayer);
			if (hitPoint.collider != null)
			{
				Debug.DrawLine(body.FootPoint.transform.position, hitPoint.point, Color.white);
				var length = Vector2.Distance(body.FootPoint.transform.position, hitPoint.point);
				flashlightLight.pointLightOuterRadius = 60 * length / life.Stats.Range(1) + 10;
			}
			else
			{
				Debug.DrawLine(body.FootPoint.transform.position, CursorManager.GetMouseWorldPosition(), Color.green);
				flashlightLight.pointLightOuterRadius = 60;
			}
		}
	}
}
