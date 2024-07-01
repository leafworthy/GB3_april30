using __SCRIPTS._ABILITIES;
using __SCRIPTS._PLAYER;
using UnityEngine;

namespace __SCRIPTS._BANDAIDS
{
	public class FollowStick : MonoBehaviour
	{
		private GunAimAbility aim;

		// Start is called before the first frame update
		public void Init(Player player)
		{
			aim = player.SpawnedPlayerGO.GetComponent<GunAimAbility>();
			if (aim == null) return;
			transform.position = aim.GetAimPoint();
		}

		private void Update()
		{
			transform.position = aim.GetAimPoint();
		}
	}
}