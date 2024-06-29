using UnityEngine;

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