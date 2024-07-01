using __SCRIPTS._MANAGERS;
using __SCRIPTS._PLAYER;
using UnityEngine;

namespace __SCRIPTS._INTERACTION
{
	public class DropPickupTimedInteraction:MonoBehaviour
	{
		private TimedInteraction interaction;

		private void Start()
		{
			interaction = GetComponent<TimedInteraction>();
			interaction.OnTimeComplete += Interaction_OnTimeComplete;
		}

		private void Interaction_OnTimeComplete(Player player)
		{
			LevelDrops.DropLoot(transform.position, LootType.Cash);
		}
	}
}