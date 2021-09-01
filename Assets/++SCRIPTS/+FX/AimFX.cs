using UnityEngine;

public class AimFX : MonoBehaviour
{
	[SerializeField] private GameObject aimTargetGraphic;
	[SerializeField] private Transform aimLightTransform;
	[SerializeField] private Transform aimLightTransform2;
	[SerializeField] private GameObject aimCenter;
	private MovementHandler movementHandler;
	private IAimHandler aimer;
	private IMovementController controller;
	private bool isAiming;
	private AnimationEvents animationEvents;

	private void Start()
	{
		animationEvents = GetComponentInChildren<AnimationEvents>();
		animationEvents.OnRoar += OnRoar;
		aimer = GetComponent<IAimHandler>();
		aimer.OnAim += AimerOnAim;
		aimer.OnAimStop += AimerOnAimStop;
		movementHandler = GetComponent<MovementHandler>();
		controller = GetComponent<IMovementController>();
		if (controller != null)
		{
			controller.OnMovePress += ControllerMoveInDirection;
		}

		aimLightTransform2.gameObject.SetActive(true);
		aimLightTransform.gameObject.SetActive(true);
	}

	private void OnRoar()
	{
		aimLightTransform2.gameObject.SetActive(true);
		aimLightTransform.gameObject.SetActive(true);
	}

	private void AimerOnAimStop()
	{
		isAiming = false;
	}

	private void ControllerMoveInDirection(Vector3 aimDir)
	{
		if (isAiming) return;
		if (aimLightTransform == null) return;
		var lightRotation = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
		aimLightTransform.eulerAngles = new Vector3(0, 0, lightRotation);
		aimLightTransform2.eulerAngles = new Vector3(0, 0, lightRotation);
	}


	private void AimerOnAim(Vector3 aimDir)
	{
		isAiming = true;
		if (aimTargetGraphic != null)
		{
			aimTargetGraphic.transform.position = aimCenter.transform.position;
		}

		if (aimLightTransform == null) return;
		var lightRotation = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
		aimLightTransform.eulerAngles = new Vector3(0, 0, lightRotation);
		aimLightTransform2.eulerAngles = new Vector3(0, 0, lightRotation);

	}
}
