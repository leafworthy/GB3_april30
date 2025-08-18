using UnityEngine;

namespace __SCRIPTS
{
	[DisallowMultipleComponent, RequireComponent(typeof(DoableDashAbility))]
	public class DoableDashController : AbilityController
	{
		private Vector2 startPoint;
		private Vector2 endPoint;
		private DoableDashAbility ability => _ability ??= GetComponent<DoableDashAbility>();
		private DoableDashAbility _ability;

		protected override void ListenToPlayer()
		{
			player.Controller.OnDash_Pressed += ControllerDashPress;
			animEvents.OnTeleport += Anim_Teleport;
		}

		private void Anim_Teleport()
		{
			ability.Teleport();
		}

		protected override void StopListeningToPlayer()
		{
			if (player?.Controller != null)
				player.Controller.OnDash_Pressed -= ControllerDashPress;

			if (animEvents != null)
				animEvents.OnTeleport -= Anim_Teleport;
		}

		private void ControllerDashPress(NewControlButton newControlButton)
		{
			ability.DoSafely();
		}
	}
}
