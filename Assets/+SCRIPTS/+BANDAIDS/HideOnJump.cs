using UnityEngine;

namespace __SCRIPTS
{
	public class HideOnJump : MonoBehaviour
	{
		public GameObject ObjectToHide;

		private JumpAbility jumps;
		private AnimationEvents events;

		private bool hasRecovered;

		private void Start()
		{
			jumps = GetComponent<JumpAbility>();
			if (jumps == null) return;
			jumps.OnJump += Jumps_OnJump;
			jumps.OnLand += Jumps_OnLand;

			hasRecovered = true;
			events = GetComponentInChildren<AnimationEvents>();
			events.OnRecovered += Events_OnRecovered;
			ObjectToHide.SetActive(false);
		}

		private void OnDisable()
		{
			if (events != null)
			{
				events.OnRecovered -= Events_OnRecovered;
			}
			if (jumps != null)
			{
				jumps.OnJump -= Jumps_OnJump;
				jumps.OnLand -= Jumps_OnLand;
			}
			var life = GetComponent<Life>();
			if (life != null)
			{
				life.OnWounded -= Life_OnWounded;
			}
		}

		private void Events_OnRecovered()
		{
			hasRecovered = true;
		}

		private void Life_OnWounded(Attack obj)
		{
			ObjectToHide.SetActive(false);
			hasRecovered = false;
		}

		private void Jumps_OnLand(Vector2 vector2)
		{
			if (!hasRecovered) return;
			ObjectToHide.SetActive(true);
		}

		private void Jumps_OnJump(Vector2 vector2)
		{
			ObjectToHide.SetActive(false);
		}
	}
}
