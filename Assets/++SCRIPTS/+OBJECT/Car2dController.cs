using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Serialization;

public class Car2dController : MonoBehaviour
{
	public GameObject exitPosition;
	public GameObject carBody;

	private float rotationForce = 60;
	private float accelerationForce = 5;
	private float inirtia = .98f;

	private UnitStats stats;
	public Player owner;
	[FormerlySerializedAs("interact")] public CarAccessInteraction carAccessInteraction;
	private Rigidbody2D rb;
	private Rotator rotator;
	private GameObject rotationObject;

	private Vector2 dir;
	public float currentTurnDegrees;
	private float speed;
	private bool hasDriver;
	private Vector3 currentVelocity;
	private MoveAbility currentMoveAbility;
	private Animations currentAnimations;
	private Body currentBody;
	private GameObject currentGO;
	private float stoppingSpeed = .1f;
	private Vector3 SorterPositionCar = new(0, -20, 0);
	private IsoSpriteSorting currentSorter;
	private float originalTurnDegrees;
	private bool playerHasEntered;
	private void Start()
	{
		Init();
	}

	[Button()]
	private void Init()
	{
		originalTurnDegrees = currentTurnDegrees;
		rotator = carBody.GetComponent<Rotator>();
		rb = carBody.GetComponent<Rigidbody2D>();
		CreateRotationObject();
	}


	private void CreateRotationObject()
	{
		rotationObject = new GameObject();
		rotationObject.transform.SetParent(carBody.transform);
		rotationObject.transform.rotation = Quaternion.Euler(0, 0, currentTurnDegrees);
		rotator.SetRotation(rotationObject.transform.up * .001f);
	}

	private void FixedUpdate()
	{
		if (Game_GlobalVariables.IsPaused) return;
		SetVelocity();
		if (hasDriver) SetRotation();
	}

	private void OnMoveChange(NewInputAxis arg1, Vector2 _dir)
	{
		if (Game_GlobalVariables.IsPaused) return;
		dir = _dir;
	}

	private void CarAccessOnCarAccessActionPressed(Player player)
	{
		if (Game_GlobalVariables.IsPaused) return;
		if (!hasDriver)
		{
			hasDriver = true;
			PlayerEnters(player);
		}
		else
		{
			hasDriver = false;
			PlayerExits(player);
		}

	}

	private void PlayerExits(Player player)
	{
		ResetLayers();
		currentGO.SetActive(true);
		ExitPlayer(player);
		ResetCurrentThings();
	}

	private void ResetCurrentThings()
	{
		
		if(currentSorter != null)
		{
			currentSorter.SorterPositionOffset = Vector3.zero;
		}
		if(currentMoveAbility != null)
		{
			currentMoveAbility.SetRBKinematic(true);
			currentMoveAbility.ActivateMovement();
			currentMoveAbility.enabled = true;
		}
		if(currentBody != null)
		{
			currentBody.legs.Stop("driving");
		}
	}

	private void ExitPlayer(Player player)
	{
		player.SpawnedPlayerGO.transform.position = exitPosition.transform.position;
		player.Controller.MoveAxis.OnChange -= OnMoveChange;
		owner.Controller.ActionButton.OnPress -= Owner_ActionPress;
	}

	private void PlayerEnters(Player player)
	{
		if (!player.hasKey) return;
		currentGO = player.SpawnedPlayerGO;
		playerHasEntered = true;
		//currentGO.transform.SetParent(carBody.transform);
		currentGO.SetActive(false);
		currentGO.transform.position = rotator.currentCarRotation.DriverPosition.transform.position;
		currentSorter = currentGO.GetComponent<IsoSpriteSorting>();
		currentSorter.SorterPositionOffset = SorterPositionCar;

		currentAnimations = currentGO.GetComponent<Animations>();
		currentMoveAbility = currentGO.transform.GetComponent<MoveAbility>();
		currentMoveAbility.StopMoving();
		currentMoveAbility.SetRBKinematic(false);
		currentMoveAbility.enabled = false;
		currentBody = currentGO.GetComponent<Body>();
		currentBody.legs.Do("driving");
		SetLayers();
		owner = player;
		owner.Controller.MoveAxis.OnChange += OnMoveChange;
		owner.Controller.ActionButton.OnPress += Owner_ActionPress;
	}

	private void Owner_ActionPress(NewControlButton obj)
	{
		CarAccessOnCarAccessActionPressed(owner);
	}

	private void SetLayers()
	{
		carBody.layer = (int)Mathf.Log(ASSETS.LevelAssets.ShootableNotWalkableLayer.value, 2);
		currentBody.FootPoint.layer = (int)Mathf.Log(ASSETS.LevelAssets.JumpingLayer.value, 2);
	}

	private void ResetLayers()
	{
		carBody.layer = (int)Mathf.Log(ASSETS.LevelAssets.JumpableLayer.value, 2);

		currentBody.FootPoint.layer = (int)Mathf.Log(ASSETS.LevelAssets.PlayerLayer.value, 2);
	}

	private void SetVelocity()
	{
		if (hasDriver) speed += dir.y * accelerationForce;
		speed *= inirtia;
		if (rotationObject == null) return;
		currentVelocity = rotationObject.transform.up * speed * Time.fixedDeltaTime;
		rb.MovePosition(rb.position + (Vector2)currentVelocity);
	}

	private void SetRotation()
	{
		rotator = carBody.GetComponent<Rotator>();

		var inReverse = speed < 0;
		if (Mathf.Abs(speed) <= stoppingSpeed) return;
		if (inReverse)
			rotator.SetRotation(-currentVelocity);
		else
			rotator.SetRotation(currentVelocity);

		currentTurnDegrees = Mathf.Lerp(currentTurnDegrees, currentTurnDegrees - dir.x * rotationForce,
			Time.fixedDeltaTime);
		rotationObject.transform.rotation = Quaternion.Euler(0, 0, currentTurnDegrees);
		if (currentGO == null) return;
		currentGO.transform.position = rotator.currentCarRotation.DriverPosition.transform.position;
	}
}