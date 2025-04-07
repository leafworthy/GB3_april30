using System;
using __SCRIPTS.HUD_Displays;
using __SCRIPTS.Projectiles;
using UnityEngine;

namespace __SCRIPTS
{
	public class PrimaryAttack_Kunai : MonoBehaviour, INeedPlayer
	{
		private AnimationEvents animationEvents;

		private AimAbility aim;
		private Animations anim;
		private Body body;
		private AmmoInventory ammoInventory;
		private string KunaiVerbName = "throwing kunai";
		private bool isPressing;
		private Player player;
		public event Action<Vector3, Vector3, float, Life, bool> OnThrow;

		private void OnEnable()
		{
			ammoInventory = GetComponent<AmmoInventory>();
			body = GetComponent<Body>();
			anim = GetComponent<Animations>();
			aim = GetComponent<AimAbility>();

			
		}

		public void SetPlayer(Player _player)
		{
			if (_player == null) return;
			player = _player;
			player.Controller.Attack1RightTrigger.OnPress += StartPress;
			player.Controller.Attack1RightTrigger.OnRelease += StopPressing;
			animationEvents = anim.animEvents;
			animationEvents.OnThrow += Anim_Throw;
			animationEvents.OnThrowStop += Anim_ThrowStop;
		}
		private void StopPressing(NewControlButton obj)
		{
			if (!isPressing) return;
			isPressing = false;
			body.arms.Stop(KunaiVerbName);
			anim.ResetTrigger(Animations.ThrowTrigger);
		}

		private void OnDisable()
		{ 
			player.Controller.Attack1RightTrigger.OnPress -= StartPress;
			player.Controller.Attack1RightTrigger.OnRelease -= StopPressing;
			animationEvents.OnThrow -= Anim_Throw;
			animationEvents.OnThrowStop -= Anim_ThrowStop;
	
		}

		private void Anim_Throw()
		{
			if (!ammoInventory.primaryAmmo.hasReserveAmmo()) return;
			body.arms.currentActivity = KunaiVerbName;
			ammoInventory.primaryAmmo.UseAmmo( 1);
	
			var throwHeight = body.ThrowPoint.transform.position.y - transform.position.y;
			var newProjectile = ObjectMaker.I.Make(ASSETS.FX.kunaiPrefab, transform.position);
			var kunaiScript = newProjectile.GetComponent<Kunai>();
			kunaiScript.Throw(aim.AimDir, transform.position, throwHeight, player.spawnedPlayerDefence);
			OnThrow?.Invoke(aim.AimDir, transform.position, throwHeight, player.spawnedPlayerDefence, false);
		}

		private void Anim_ThrowStop()
		{
			body.arms.Stop(KunaiVerbName);
			if (isPressing)
			{
				StartAttack();
			}
			else
			{
				Debug.Log("not pressing");
			}
		}


		private void StartPress(NewControlButton newControlButton)
		{
			if (isPressing) return;
			isPressing = true;
			StartAttack();
		}

		private void StartAttack()
		{
			if (!ammoInventory.primaryAmmo.hasReserveAmmo())
			{
				Debug.Log("no kunai ammo");
				return;
			}
		
			anim.SetTrigger(Animations.ThrowTrigger);
			
		}


	}
}