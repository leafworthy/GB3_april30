using UnityEngine;

namespace __SCRIPTS
{
	public class HideOnJump : MonoBehaviour
	{
		public GameObject ObjectToHide;

		private CharacterJumpAbility jumps;
		private AnimationEvents events;

		private bool hasRecovered;

		private void Start()
		{
			jumps = GetComponent<CharacterJumpAbility>();
			if (jumps == null) return;
			jumps.OnJump += JumpsOnJump;
			jumps.OnLand += JumpsOnLand;

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

			if (jumps == null) return;
			jumps.OnJump -= JumpsOnJump;
			jumps.OnLand -= JumpsOnLand;
		}

		private void Events_OnRecovered()
		{
			hasRecovered = true;
		}
		private void JumpsOnLand(Vector2 vector2)
		{
			if (!hasRecovered) return;
			ObjectToHide.SetActive(true);
		}

		private void JumpsOnJump(Vector2 vector2)
		{
			ObjectToHide.SetActive(false);
		}
	}
}
