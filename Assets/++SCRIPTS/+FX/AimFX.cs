using UnityEngine;

public class AimFX : MonoBehaviour
{
	[SerializeField] private GameObject aimTargetGraphic;
	[SerializeField] private Transform aimLightTransform;
	[SerializeField] private GameObject aimCenter;
	private MovementHandler movementHandler;
	private IAimHandler aimer;
	private IMovementController controller;
	private bool isAiming;
	private AnimationEvents animationEvents;

	private void Awake()
	{
		animationEvents = GetComponentInChildren<AnimationEvents>();
		animationEvents.OnRoar += OnRoar;
		aimer = GetComponent<IAimHandler>();
		aimer.OnAim += AimerOnAim;
		aimer.OnAimStop += AimerOnAimStop;
		movementHandler = GetComponent<MovementHandler>();
		controller = GetComponent<IMovementController>();
		controller.OnMovePress += ControllerMoveInDirection;
		aimLightTransform.gameObject.SetActive(false);
	}

	private void OnRoar()
	{
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
		if (aimDir.x >= 0)
		{
			movementHandler.FaceDir(true);
		}
		else
		{
			movementHandler.FaceDir(false);

		}
	}
}
