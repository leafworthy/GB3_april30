using UnityEngine;

namespace __SCRIPTS
{
	public class HideOnJump : MonoBehaviour
	{
		public GameObject ObjectToHide;
		JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		JumpAbility _jumps;

		void Start()
		{
			if (jumps == null) return;
			jumps.OnJump += JumpsOnJump;
			jumps.OnLand += JumpsOnLand;

			ObjectToHide.SetActive(false);
		}

		void OnDisable()
		{
			if (jumps == null) return;
			jumps.OnJump -= JumpsOnJump;
			jumps.OnLand -= JumpsOnLand;
		}

		void JumpsOnLand(Vector2 vector2)
		{
			ObjectToHide.SetActive(true);
		}

		void JumpsOnJump(Vector2 vector2)
		{
			ObjectToHide.SetActive(false);
		}
	}
}
