using UnityEngine;

namespace __SCRIPTS
{
	public class FollowCursor : MonoBehaviour
	{
		IAimAbility cachedAimAbility;

		public void Init(Player player)
		{
			if (player?.SpawnedPlayerGO == null) return;
			cachedAimAbility = player.SpawnedPlayerGO.GetComponentInChildren<IAimAbility>();

			if (cachedAimAbility == null) return;

			transform.position = cachedAimAbility.GetAimPoint();
		}

		void Update()
		{
			// Use cached reference instead of GetComponent calls
			if (cachedAimAbility == null) return;
			transform.position = cachedAimAbility.GetAimPoint();
		}
	}
}
