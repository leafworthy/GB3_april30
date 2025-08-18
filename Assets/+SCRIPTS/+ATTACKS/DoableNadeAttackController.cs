using UnityEngine;

namespace __SCRIPTS
{
	[DisallowMultipleComponent, RequireComponent(typeof(NadeAttack))]
	public class DoableNadeAttackController : AbilityController
	{
		private AnimationEvents animationEvents;
		private Vector2 startPoint;
		private Vector2 endPoint;
		private NadeAttack ability => _ability ??= GetComponent<NadeAttack>();
		private NadeAttack _ability;


		private void Player_OnAim(Vector2 vector2)
		{
			ability.Aim();
		}

		private void Anim_Throw()
		{
			ability.Throw();
		}

		private void Anim_ThrowStop()
		{
			ability.StopActivity();
		}

		protected override void ListenToPlayer()
		{
			animEvents.OnThrow += Anim_Throw;
			animEvents.OnThrowStop += Anim_ThrowStop;
			player.Controller.OnAimAxis_Change += Player_OnAim;
			player.Controller.OnAttack2_Pressed += Player_NadePress;
			player.Controller.OnAttack2_Released += Player_NadeRelease;
		}

		protected override void StopListeningToPlayer()
		{
			animEvents.OnThrow -= Anim_Throw;
			animEvents.OnThrowStop -= Anim_ThrowStop;
			player.Controller.OnAimAxis_Change -= Player_OnAim;
			player.Controller.OnAttack2_Pressed -= Player_NadePress;
			player.Controller.OnAttack2_Released -= Player_NadeRelease;
		}

		private void Player_NadePress(NewControlButton newControlButton)
		{
			ability.DoSafely();
		}

		private void Player_NadeRelease(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;
			ability.ReleaseNade();

		}
	}
}
