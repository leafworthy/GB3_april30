
using __SCRIPTS._ABILITIES;
using __SCRIPTS._ATTACKS;
using __SCRIPTS._UNITS;
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
			hasRecovered = true;
			events = GetComponentInChildren<AnimationEvents>();
			events.OnRecovered += Events_OnRecovered;
			var life = GetComponent<Life>();
			life.OnDamaged += Life_OnDamaged;

			jumps = GetComponent<JumpAbility>();
			jumps.OnJump += Jumps_OnJump;
			jumps.OnLand += Jumps_OnLand;
			ObjectToHide.SetActive(false);
		}

		private void Events_OnRecovered()
		{
			hasRecovered = true;
		}

		private void Life_OnDamaged(Attack obj)
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