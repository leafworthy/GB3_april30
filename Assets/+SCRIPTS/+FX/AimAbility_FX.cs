using __SCRIPTS.Cursor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace __SCRIPTS
{
	public class AimAbility_FX : MonoBehaviour
	{
		private IAimAbility aimAbility;
		private Body body;
		public Light2D flashlightLight;
		private Life life;

		private void Start()
		{
			life = GetComponent<Life>();
			if(life == null) return;
			aimAbility = GetComponent<IAimAbility>();
			body = GetComponent<Body>();
		}

		private void Update()
		{
			if (Services.pauseManager.IsPaused) return;
			if(aimAbility.hasEnoughMagnitude())
			{
				AimFlashlight();
			}

		}

		private RaycastHit2D CheckRaycastHit(Vector3 targetDirection, LayerMask layer)
		{
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, life.PrimaryAttackRange, layer);
			return raycastHit;
		}

		private void AimFlashlight()
		{

			var hitPoint = CheckRaycastHit(aimAbility.AimDir, Services.assetManager.LevelAssets.BuildingLayer);
			if (hitPoint.collider != null)
			{
				Debug.DrawLine(body.FootPoint.transform.position, hitPoint.point, Color.white);
				var length = Vector2.Distance(body.FootPoint.transform.position, hitPoint.point);
				flashlightLight.pointLightOuterRadius = 60 * length / life.PrimaryAttackRange + 10;
			}
			else
			{
				Debug.DrawLine(body.FootPoint.transform.position, CursorManager.GetMousePosition(), Color.green);
				flashlightLight.pointLightOuterRadius = 60;
			}
		}
	}
}
