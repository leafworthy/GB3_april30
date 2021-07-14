using System;
using UnityEngine;

public class DashHandler : MonoBehaviour
{
	public event Action OnDashCommand;
	private AnimationEvents animEvents;
	private IMovementController controller;
	private MovementHandler movementHandler;
	private UnitStats stats;
	private PushHandler pushHandler;
	private IAimHandler attackHandler;
	private Vector3 aimDir;
	public float distanceMult = 4;

	private void ControllerDashPress()
	{
		if (!movementHandler.canMove) return;
		movementHandler.DisableMovement();
		OnDashCommand?.Invoke();
	}
	private void Start()
	{
		attackHandler = GetComponent<IAimHandler>();
		attackHandler.OnAim += Aim;
		pushHandler = GetComponent<PushHandler>();
		movementHandler = GetComponent<MovementHandler>();

		stats = GetComponent<UnitStats>();

		controller = GetComponent<IMovementController>();
		controller.OnDashButtonPress += ControllerDashPress;

		animEvents = GetComponentInChildren<AnimationEvents>();
		animEvents.OnDashStart += Anim_DashStart;
		animEvents.OnDash += Anim_Dash;
		animEvents.OnDashStop += Anim_DashStop;
		animEvents.OnTeleport += Anim_Teleport;
	}

	private void Aim(Vector3 obj)
	{
		aimDir = obj;
	}

	private void Anim_Teleport()
	{
		var destination = (Vector2) transform.position + movementHandler.moveDir * stats.GetStatValue(StatType.teleportSpeed);
		movementHandler.MoveObjectTo(destination, true);
	}


	private void Anim_DashStop()
	{
		gameObject.layer = (int) Mathf.Log(ASSETS.LevelAssets.PlayerLayer.value, 2);
		movementHandler.EnableMovement();
	}

	private void Anim_Dash()
	{
		Debug.Log("push dash");
		Debug.Log(movementHandler.moveDir + "moveDir");
		Debug.Log(stats.GetStatValue(StatType.dashSpeed) + "dashSpeed");
		var SpecialAttackDistance = Vector2.Distance(attackHandler.GetAimCenter(),
			Camera.main.ScreenToWorldPoint(Input.mousePosition));
		pushHandler.Push(aimDir, SpecialAttackDistance*distanceMult);
	}

	private void Anim_DashStart()
	{
		movementHandler.DisableMovement();
	}


}
