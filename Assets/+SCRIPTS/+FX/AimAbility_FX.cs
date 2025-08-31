using __SCRIPTS.Cursor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace __SCRIPTS
{
	public class AimAbility_FX : ServiceUser
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
			if (pauseManager.IsPaused) return;
			if(aimAbility.hasEnoughMagnitude())
			{
				AimFlashlight();
			}

		}

		private void AimFlashlight()
		{

			var hitPoint = aimAbility.CheckRaycastHit(aimAbility.AimDir);
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
