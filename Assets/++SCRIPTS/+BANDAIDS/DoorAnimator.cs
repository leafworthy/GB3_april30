using __SCRIPTS._INTERACTION;
using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;
using Pathfinding;
using UnityEngine;

namespace __SCRIPTS._BANDAIDS
{
	public class DoorAnimator : TimedInteraction
	{
		public GameObject Door_Open;
		public GameObject Door_Closed;
		public GameObject Door_Broken;

	
		public bool isOpen;
		public bool isBroken;
		private bool hasReleased;

		[SerializeField] private Life life;
		private static bool HasOpenedADoor;
		private Bounds bounds;
		[SerializeField]private HideRevealObjects damageStates;
		[SerializeField] private DoorDefence doorDefence;
		public GameObject targetPosition;
		private bool HasBarricadedADoor;

		protected override void Start()
		{
			base.Start();
			life.OnDead += BreakDoor;
			life.OnFractionChanged += SetDamageState;
			life.OnResurrected += FixDoor;

		
		
			OnPlayerEnters += DoorInteractionPlayerEnters;
			OnPlayerExits += DoorInteractionPlayerExits;
			OnActionPress += DoorInteractionPlayerPress;
			OnActionRelease += DoorInteractionPlayerRelease;
			OnTimeComplete += DoorInteractionTimeComplete;

			bounds = GetComponent<Collider2D>().bounds;
			SetDamageState();
		}

		private void DoorInteractionPlayerPress(Player obj)
		{
			hasReleased = false;
		}

		private void SetDamageState(float f = 0)
		{
			if (damageStates.objectsToReveal.Count <= 0) return;
			var index =  damageStates.objectsToReveal.Count;
			var currentDamageLevel = Mathf.FloorToInt((1-life.GetFraction())* index);
			if (life.Health >= life.HealthMax)
			{
				damageStates.Set(0);
			}
			else
			{
				damageStates.Set(currentDamageLevel);
			}
		}

		private void DoorInteractionTimeComplete(Player player)
		{
			hasReleased = true;
			if (life.GetFraction() < 1)
			{
				HasBarricadedADoor = true;
				life.AddHealth(life.HealthMax / 6);
			}
			else
			{
				if (doorDefence == null) return;
				doorDefence.Repair();
			}
		}
		private void DoorInteractionPlayerRelease(Player player)
		{
			if (hasReleased) return;
			if (isBroken) return;
			hasReleased = true;
			OpenOrCloseDoor();
		}

		private void OpenOrCloseDoor()
		{
			if (isOpen) CloseDoor();
			else OpenDoor();
		}

		private void DoorInteractionPlayerEnters(Player player)
		{
			if (isBroken) return;
			if (HasOpenedADoor)
			{
				if (HasBarricadedADoor) return;
				player.Say(isOpen ?  "E - Close Door":"Hold E - Barricade", 0);
				return;
			}
			player.Say(isOpen ?   "E - Close door": "E - to Open door", 0);
		}

		private void DoorInteractionPlayerExits(Player player)
		{
			player.StopSaying();
		}

		private void CleanUp()
		{
			isBroken = false;
			Door_Broken.SetActive(false);
			Door_Closed.SetActive(true);
			Door_Open.SetActive(false);
			isOpen = false;
			UpdateGraph();
		}


		private void OpenDoor()
		{
			if (isBroken) return;
			if (isOpen) return;
			HasOpenedADoor = true;
			Door_Closed.SetActive(false);
			Door_Open.SetActive(true);
			isOpen = true;
			UpdateGraph();
		}

		private void CloseDoor()
		{
			if (isBroken) return;
			if (!isOpen) return;
			Door_Closed.SetActive(true);
			Door_Open.SetActive(false);
			isOpen = false;
			UpdateGraph();
		}

		private void BreakDoor(Player player)
		{
			if (isBroken) return;
			isBroken = true;
			Door_Broken.SetActive(true);
			Door_Closed.SetActive(false);
			Door_Open.SetActive(false);
			isOpen = true;
			UpdateGraph();
		}

		private void FixDoor(Player player)
		{
			Door_Closed.SetActive(true);
			Door_Open.SetActive(false);
			isBroken = false;
			isOpen = false;
			UpdateGraph();
		}

		private void UpdateGraph()
		{
			var guo = new GraphUpdateObject(bounds)
			{
				updatePhysics = true
			};

			AstarPath.active.UpdateGraphs(guo);
		}

	}
}
