using UnityEngine;

namespace __SCRIPTS
{
	public class DoorRepair : TimedInteraction
	{
		private DoorInteraction doorInteraction;
		private Life life;
		protected override void Start()
		{
			base.Start();
			doorInteraction = GetComponent<DoorInteraction>();
			doorInteraction.OnBreak += BreakDoorInteraction;
			OnTimeComplete += RepairDoor;
		}

		private void BreakDoorInteraction(Player player)
		{
			ListenToPlayerActionButton(player);
		}

		protected override void InteractableOnActionPress(Player player)
		{
			if (!doorInteraction.isBroken) return;
			base.InteractableOnActionPress(player);
		}

		protected override void OnTriggerEnter2D(Collider2D other)
		{
			if (!doorInteraction.isBroken) return;
			base.OnTriggerEnter2D(other);
		}

	
		private void RepairDoor(Player player)
		{
			doorInteraction.Repair(player);
			StopListeningToPlayer(player);
		}
	}
}