using __SCRIPTS._ABILITIES;
using __SCRIPTS._CAMERA;
using __SCRIPTS._COMMON;
using __SCRIPTS._MANAGERS;
using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace __SCRIPTS._FX
{
	public class AimAbility_FX : MonoBehaviour
	{
		private AimAbility aimAbility;
		private Body body;
		private Light2D flashlightLight;
		private Life life;

		private void Start()
		{
			life = GetComponent<Life>();
			if(life == null) return;
			aimAbility = GetComponent<AimAbility>();
			body = GetComponent<Body>();
			flashlightLight = GetComponentInChildren<Light2D>();
		}

		private void Update()
		{
			if (GlobalManager.IsPaused) return;
			AimFlashlight();
		
		}

		private void AimFlashlight()
		{
			var hitPoint = aimAbility.CheckRaycastHit(aimAbility.GetAimDir());
			if (hitPoint.collider != null)
			{
				Debug.DrawLine(body.FootPoint.transform.position, hitPoint.point, Color.red);
				var length = Vector2.Distance(body.FootPoint.transform.position, hitPoint.point);
				flashlightLight.pointLightOuterRadius = 60 * length / life.AttackRange + 10;
			}
			else
			{
				Debug.DrawLine(body.FootPoint.transform.position, CursorManager.GetMousePosition(), Color.green);
				flashlightLight.pointLightOuterRadius = 60;
			}
		}
	}
}