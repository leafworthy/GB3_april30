using System.Collections.Generic;
using UnityEngine;

public class NadeThrower : MonoBehaviour
{
	private AnimationEvents animationEvents;
	private Vector2 pointA;
	private Vector2 pointB;

	private List<GameObject> _trajectoryMarkersContainer = new List<GameObject>();
	private float _height = 8f;
	private int _numberOfMarkers = 10;
	private int throwTime = 30;

	private GameObject currentArrowHead;
	private float throwDistanceMultiplier = 60;
	private GunAimer aim;
	private UnitStats stats;
	private AmmoInventory ammo;
	private Arms arms;

	private bool IsAiming;
	private Player player;
	private float currentCooldownTime;
	private Vector2 aimRangeMultiplier;
	private float cooldownRate;
	private List<Animator> animators;
	private Body body;
	private Animations anim;
	private string VerbName = "nading";
	private string AimVerbName = "aiming";
	private string AnimationName = "Top-Throw-Nade";
	private Life life;
	private bool isActive;

	private void Start()
	{
		isActive = true;
		anim = GetComponent<Animations>();
		life = GetComponent<Life>();
		life.OnDead += (x)=>Die();
		body = GetComponent<Body>();
		arms = body.arms;
		ammo = GetComponent<AmmoInventory>();
		aim = GetComponent<GunAimer>();
		ListenToPlayer();

		SpawnFX();
		HideAiming();

		Level.OnStop += Die;
		cooldownRate = stats.AttackRate;
		
	}

	private void ListenToPlayer()
	{
		animationEvents = anim.animEvents;
		animationEvents.OnThrow += Anim_Throw;
		animationEvents.OnThrowStop += Anim_ThrowStop;

		stats = GetComponent<UnitStats>();
		player = stats.player;
		player.Controller.AimAxis.OnChange += Player_OnAim;
		player.Controller.Attack2.OnPress += Player_NadePress;
		player.Controller.Attack2.OnRelease += Player_NadeRelease;
	}
	 
	private void StopListeningToPlayer()
	{
		DestroyFX();
		if (player == null) return;
		player.Controller.AimAxis.OnChange -= Player_OnAim;
		player.Controller.Attack2.OnPress -= Player_NadePress;
		player.Controller.Attack2.OnRelease -= Player_NadeRelease;
	}

	private void DestroyFX()
	{
		if (currentArrowHead != null) Maker.Unmake(currentArrowHead);
		foreach (var marker in _trajectoryMarkersContainer) Maker.Unmake(marker);
		_trajectoryMarkersContainer.Clear();
	}

	private void Die(Scene.Type type = Scene.Type.Endscreen)
	{
		isActive = false;
		StopListeningToPlayer();
	}

	private void Anim_ThrowStop()
	{
		if (!isActive) return;
		arms.Stop("NadeThrowing");
		anim.SetBool(Animations.IsShooting, false);
	}

	private void Update()
	{
		if (!isActive) return;
		if (IsAiming)
		{
			ShowAiming();
		}
		else
		{
			HideAiming();
		}
		switch (IsAiming)
		{
			case true when player.isUsingMouse:
				AimAt(CursorManager.GetMousePosition());
				return;
			case true:
				Aim(aim.AimDir);
				break;
		}
	}


	private void Player_NadePress(NewControlButton newControlButton)
	{
		if (Game_GlobalVariables.IsPaused) return;
		if (!isActive) return;
		if (!arms.Do(AimVerbName))
		{
			return;
		}

		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades))
		{
			return;
		}
		IsAiming = true;
		ShowAiming();
	}

	private void Player_NadeRelease(NewControlButton newControlButton)
	{
		if (Game_GlobalVariables.IsPaused) return;
		if (!IsAiming) return;
		if (!isActive) return;
		arms.Stop(AimVerbName);
		IsAiming = false;
		NadeWithCooldown(aim.AimDir);
		HideAiming();
		anim.SetBool(Animations.IsShooting, false);
	}

	private void NadeWithCooldown(Vector3 target)
	{
		if (!isActive) return;
		if (!(Time.time >= currentCooldownTime))
		{
			return;
		}

		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades))
		{
			return;
		}

		if (!arms.Do(VerbName)) return;
		ammo.UseAmmo(AmmoInventory.AmmoType.nades, 1);
		anim.Play(AnimationName,1,0);
		currentCooldownTime = Time.time + cooldownRate;
	}

	private void Anim_Throw()
	{
		if (!isActive) return;
		var newProjectile = Maker.Make(ASSETS.FX.nadePrefab, body.AimCenter.transform.position);
		var nadeThrower = newProjectile.GetComponent<Nade>();
		ASSETS.sounds.bean_nade_throw_sounds.PlayRandom();
		pointB = aim.GetAimPoint();
		pointA = body.AimCenter.transform.position;
		var velocity = new Vector3((pointB.x - pointA.x) / throwTime, (pointB.y - pointA.y) / throwTime);
		nadeThrower.Launch(pointA, velocity, throwTime, stats.player);
	}

	private void SpawnFX()
	{
		if (!isActive) return;
		currentArrowHead = Maker.Make(ASSETS.FX.nadeTargetPrefab, pointB);
		currentArrowHead.SetActive(false);
		for (var i = 0; i < _numberOfMarkers; i++)
			_trajectoryMarkersContainer.Add(Maker.Make(ASSETS.FX.trajectoryMarkerPrefab));
	}

	private void ShowAiming()
	{
		if (!isActive) return;
		currentArrowHead.SetActive(true);
		if (_trajectoryMarkersContainer.Count <= 0) return;
		foreach (var marker in _trajectoryMarkersContainer) marker.SetActive(true);
	}

	private  void HideAiming()
	{
		if (!isActive) return;
		if (currentArrowHead == null) return;
		currentArrowHead.SetActive(false);
		if (_trajectoryMarkersContainer.Count <= 0) return;
		foreach (var marker in _trajectoryMarkersContainer) marker.SetActive(false);
	}



	private void Player_OnAim(IControlAxis controlAxis, Vector2 aimDir)
	{
		if (!isActive) return;
		Aim(controlAxis.GetCurrentAngle());
	}

	private void Aim(Vector2 aimDir)
	{
		if (!isActive) return;
		if (body == null) return;
		pointA = body.AimCenter.transform.position;
		pointB = (Vector2)body.AimCenter.transform.position + (Vector2)aimDir * throwDistanceMultiplier;
		PlaceMarkers();
	}

	private void AimAt(Vector2 aimPos)
	{
		pointA = body.AimCenter.transform.position;
		pointB = aimPos;
		PlaceMarkers();
	}

	private void PlaceMarkers()
	{
		currentArrowHead.transform.position = pointB;
		ShowAiming();
		for (float i = 1; i <= _numberOfMarkers; i++)
		{
			var currentPosition = SampleParabola(pointA, pointB, _height, i / (float)_numberOfMarkers);
			_trajectoryMarkersContainer[(int)i - 1].transform.position =
				new Vector3(currentPosition.x, currentPosition.y, 0);

			var nextPosition = SampleParabola(pointA, pointB, _height, (i + 1) / (float)_numberOfMarkers);
			var angleInR = Mathf.Atan2(nextPosition.y - currentPosition.y, nextPosition.x - currentPosition.x);
			_trajectoryMarkersContainer[(int)i - 1].transform.eulerAngles =
				new Vector3(0, 0, Mathf.Rad2Deg * angleInR - 90);
		}
	}


	private static Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
	{
		var parabolicT = t * 2 - 1;
		if (Mathf.Abs(start.y - end.y) < 0.1f)
		{
			var travelDirection = end - start;
			var result = start + t * travelDirection;
			result.y += (-parabolicT * parabolicT + 1) * height;
			return result;
		}
		else
		{
			var travelDirection = end - start;
			var levelDirection = end - new Vector3(start.x, end.y, start.z);
			var right = Vector3.Cross(travelDirection, levelDirection);
			var up = Vector3.Cross(right, levelDirection);
			if (end.y > start.y) up = -up;
			var result = start + t * travelDirection;
			result += (-parabolicT * parabolicT + 1) * height * up.normalized;
			return result;
		}
	}
}
