using UnityEngine;

namespace __SCRIPTS
{
	[DisallowMultipleComponent, RequireComponent(typeof(DoableNadeAttackAbility))]
	public class DoableNadeAttackController : AbilityController
	{
		private AnimationEvents animationEvents;
		private Vector2 startPoint;
		private Vector2 endPoint;
		private DoableNadeAttackAbility ability => _ability ??= GetComponent<DoableNadeAttackAbility>();
		private DoableNadeAttackAbility _ability;
		private PauseManager pauseManager  => _pauseManager ??= ServiceLocator.Get<PauseManager>();
		private PauseManager _pauseManager;

		private void Player_OnAim(NewInputAxis arg1, Vector2 arg2)
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
			player.Controller.AimAxis.OnChange += Player_OnAim;
			player.Controller.Attack2LeftTrigger.OnPress += Player_NadePress;
			player.Controller.Attack2LeftTrigger.OnRelease += Player_NadeRelease;
		}

		protected override void StopListeningToPlayer()
		{
			animEvents.OnThrow -= Anim_Throw;
			animEvents.OnThrowStop -= Anim_ThrowStop;
			player.Controller.AimAxis.OnChange -= Player_OnAim;
			player.Controller.Attack2LeftTrigger.OnPress -= Player_NadePress;
			player.Controller.Attack2LeftTrigger.OnRelease -= Player_NadeRelease;
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
