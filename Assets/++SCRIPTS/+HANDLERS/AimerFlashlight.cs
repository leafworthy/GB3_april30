using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AimerFlashlight : MonoBehaviour
{
	private Aimer aimer;
	private Body body;
	private Light2D flashlightLight;
	private Life life;

	private void Start()
	{
		life = GetComponent<Life>();
		if(life == null) return;
		aimer = GetComponent<Aimer>();
		body = GetComponent<Body>();
		flashlightLight = GetComponentInChildren<Light2D>();
	}

	private void Update()
	{
		if (Game_GlobalVariables.IsPaused) return;
		AimFlashlight();
		
	}

	private void AimFlashlight()
	{
		var hitPoint = aimer.CheckRaycastHit(aimer.GetAimDir());
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