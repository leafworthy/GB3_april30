using UnityEngine;

namespace __SCRIPTS
{
	public class FollowCursor : MonoBehaviour
	{
		private IAimAbility cachedAimAbility;

		// Optimized initialization using cached component reference
		public void Init(Player player)
		{
			if (player?.SpawnedPlayerGO != null)
			{
				// Cache the AimAbility component reference to avoid GetComponent calls
				cachedAimAbility = player.SpawnedPlayerGO.GetComponentInChildren<IAimAbility>();

				if (cachedAimAbility == null) return;

				// Initialize position
				transform.position = cachedAimAbility.GetAimPoint();
			}
		}

		private void Update()
		{
			// Use cached reference instead of GetComponent calls
			if (cachedAimAbility == null) return;
			transform.position = cachedAimAbility.GetAimPoint();
		}
	}
}
