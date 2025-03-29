using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class PrimaryAttack_Kunai : MonoBehaviour
	{
		private AnimationEvents animationEvents;

		private Life life;
		private AimAbility aim;
		private Animations anim;
		private Body body;
		private AmmoInventory ammo;
		private string KunaiVerbName = "throwing kunai";
		private bool isPressing;
		public event Action<Vector3, Vector3, float, Life, bool> OnThrow;

		private void Start()
		{
			Init();
		}

		private void Init()
		{
			ammo = GetComponent<AmmoInventory>();
			body = GetComponent<Body>();
			anim = GetComponent<Animations>();
			aim = GetComponent<AimAbility>();
			life = GetComponent<Life>();

			life.player.Controller.Attack1RightTrigger.OnPress += StartPress;
			life.player.Controller.Attack1RightTrigger.OnRelease += StopPressing;
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
			life.player.Controller.Attack1RightTrigger.OnPress -= StartPress;
			life.player.Controller.Attack1RightTrigger.OnRelease -= StopPressing;
			animationEvents.OnThrow -= Anim_Throw;
			animationEvents.OnThrowStop -= Anim_ThrowStop;
	
		}

		private void Anim_Throw()
		{
			if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.primaryAmmo)) return;
			body.arms.currentActivity = KunaiVerbName;
			ammo.UseAmmo(AmmoInventory.AmmoType.primaryAmmo, 1);
	
			var throwHeight = body.ThrowPoint.transform.position.y - transform.position.y;
			OnThrow?.Invoke(aim.AimDir, transform.position, throwHeight, life, false);
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
			if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.primaryAmmo))
			{
				Debug.Log("no kunai ammo");
				return;
			}
		
			anim.SetTrigger(Animations.ThrowTrigger);
			
		}


	}
}