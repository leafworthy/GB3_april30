using System;
using __SCRIPTS.Plugins._ISOSORT;
using __SCRIPTS.Plugins.AstarPathfindingProject.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
	public class DoorInteraction : TimedInteraction
	{
		public event Action<Player> OnBreak;

		public Animator DoorAnimator;
		public bool isOpen;
		public bool isBroken;

		[FormerlySerializedAs("doorColliderDisabledOnOpen")]
		public Collider2D ColliderDisabledOnOpen;
		public Collider2D ShadowCollider;

		[SerializeField] private Life life;

		private Bounds bounds;
		private static readonly int IsOpen = Animator.StringToHash("IsOpen");
		private static readonly int IsBroken = Animator.StringToHash("IsBroken");

		public IsoSpriteSorting closedSorting;
		private SortingPoints openSortingPoints;
		private Vector3 closedSortingSorterPositionOffset;
		private Vector3 closedSortingSorterPositionOffset2;

		protected override void OnEnable()
		{
			base.OnEnable();
			if(life == null) life = GetComponentInChildren<Life>();
			life.OnDying += BreakDoor;
			OnPlayerExits += PlayerExits;
			OnTimeComplete += Repair;
			Init();
		}

		protected override void OnDisable()
		{
			if (life == null) return;
			life.OnDying += BreakDoor;
			OnPlayerExits += PlayerExits;
			OnTimeComplete += Repair;
			base.OnDisable();
				 
			 
		}

		private void Init()
		{
			bounds = GetComponent<Collider2D>().bounds;
			isBroken = false;
			SetDoorOpen(false);
			if(closedSorting != null)
			{
				closedSortingSorterPositionOffset = closedSorting.SorterPositionOffset;
				closedSortingSorterPositionOffset2 = closedSorting.SorterPositionOffset2;
			}
			if (life == null) life = GetComponent<Life>();
			openSortingPoints = GetComponentInChildren<SortingPoints>();
			life.Resurrect();
		}

		private void PlayerExits(Player obj)
		{
			//Close the door behind you
			if (isBroken) return;
			SetDoorOpen(false);
		}
	
		protected override void InteractableOnActionPress(Player player)
		{
			if (isBroken)
			{
				//start normal loading bar
				base.InteractableOnActionPress(player);
				SFX.I.sounds.door_repair_sound.PlayRandomAt(transform.position);
				return;
			}

			SetDoorOpen(!isOpen);
		}

		private void SetDoorOpen(bool open)
		{
			if (isOpen == open) return;
			isOpen = open;
			DoorAnimator.SetBool(IsOpen, open);
			if (isOpen)
			{
				SFX.I.sounds.door_open_sound.PlayRandomAt(transform.position);
			}
			else
			{
				SFX.I.sounds.door_close_sound.PlayRandomAt(transform.position);
			}
			UpdateGraph();
		}

		public void Animation_OnOpenDoor(bool open)
		{
			if (ColliderDisabledOnOpen != null) ColliderDisabledOnOpen.gameObject.SetActive(!open);
			if (ShadowCollider != null) ShadowCollider.gameObject.SetActive(!open);
			if (closedSorting == null) return;

			if (open)
			{
				closedSorting.SorterPositionOffset = openSortingPoints.openSortingPoint1;
				closedSorting.SorterPositionOffset2 = openSortingPoints.openSortingPoint2;
			}
			else
			{
				closedSorting.SorterPositionOffset = closedSortingSorterPositionOffset;
				closedSorting.SorterPositionOffset2 = closedSortingSorterPositionOffset2;
			}
		}

		private void BreakDoor(Player player, Life life1)
		{
			if (isBroken) return;
			isBroken = true;
			DoorAnimator.SetBool(IsBroken, true);


		
			SFX.I.sounds.door_break_sound.PlayRandomAt(life1.transform.position);
			SFX.I.sounds.door_break_sound.PlayRandomAt(life1.transform.position);

			SetCollidersEnabled(false);
			OnBreak?.Invoke(player);
		}

		private void SetCollidersEnabled(bool collidersEnabled)
		{
			if (ColliderDisabledOnOpen != null) ColliderDisabledOnOpen.enabled = collidersEnabled;
			if (ShadowCollider != null) ShadowCollider.gameObject.SetActive(collidersEnabled);
		}

		private void UpdateGraph()
		{
			var guo = new GraphUpdateObject(bounds);
			guo.updatePhysics = true;
			AstarPath.active.UpdateGraphs(guo);
		}

		public void Repair(Player player)
		{
			if (!isBroken) return;
			isBroken = false;
			DoorAnimator.SetBool(IsBroken, false);
			SetCollidersEnabled(true);
			UpdateGraph();
			life.Resurrect();

			ListenToPlayerActionButton(player);
		}
	}
}