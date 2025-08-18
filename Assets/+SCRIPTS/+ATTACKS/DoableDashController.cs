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
			player.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
			animEvents.OnTeleport += Anim_Teleport;
		}

		private void Anim_Teleport()
		{
			ability.Teleport();
		}

		protected override void StopListeningToPlayer()
		{
			if (player?.Controller != null)
				player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;

			if (animEvents != null)
				animEvents.OnTeleport -= Anim_Teleport;
		}

		private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
		{
			ability.DoSafely();
		}
	}
}
