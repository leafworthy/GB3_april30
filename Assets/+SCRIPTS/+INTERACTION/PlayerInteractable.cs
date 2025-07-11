using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlayerInteractable : ServiceUser
	{
		public event Action<Player> OnActionPress;
		public event Action<Player> OnActionRelease;
		public event Action<Player> OnPlayerEnters;
		public event Action<Player> OnPlayerExits;
		public event Action<Player> OnPlayerFinishes;

		private List<Player> playersListening = new();
		public bool isSelected;
		protected bool isFinished;
		public event Action<Player> OnSelected;
		public event Action<Player> OnDeselected;
		public InteractionIndicator interactionIndicator=> GetComponentInChildren<InteractionIndicator>();
		public Vector2 GetInteractionPosition() => interactionIndicator.indicatorOffset + transform.position;
		private Vector2 interactionPosition => interactionIndicator.interactionPoint + transform.position;



		protected virtual void FinishInteraction(Player player)
		{
			OnPlayerFinishes?.Invoke(player);
			isFinished = true;
		}
		protected virtual void OnTriggerEnter2D(Collider2D other)
		{
			if(isFinished) return;
			if (other.gameObject == gameObject) return;

			ListenToPlayerActionButton(other);
		}

		private void ListenToPlayerActionButton(Collider2D other)
		{
			var life = other.GetComponent<Life>();
			if (life == null)
			{
				life = other.GetComponentInParent<Life>();
				if (life == null) return;
			}

			ListenToPlayerActionButton(life.player);
		}

		protected void ListenToPlayerActionButton(Player player)
		{
			if (!canEnter(player)) return;
			if (playersListening.Contains(player)) return;
			if (!player.IsPlayer()) return;
			playersListening.Add(player);


			player.Controller.InteractRightShoulder.OnPress += PlayerOnInteractButton;
			player.Controller.InteractRightShoulder.OnRelease += Player_OnRelease;

			OnPlayerEnters?.Invoke(player);
		}

		private void Player_OnRelease(NewControlButton newControlButton)
		{
			if (isFinished) return;
			if (!isSelected) return;
			OnActionRelease?.Invoke(newControlButton.owner);
			if (!canInteract(newControlButton.owner)) StopListeningToPlayer(newControlButton.owner);
		}

		private bool buildingIsInTheWay(Vector2 playerPosition)
		{
			if (interactionIndicator == null) return true;

			var hits = Physics2D.LinecastAll(playerPosition, interactionPosition, assets.LevelAssets.BuildingLayer);
			//Debug.DrawLine( playerPosition, interactionPosition, Color.blue, 1f);

			if(hits.Length == 0)
			{
				//MyDebugUtilities.DrawX(interactionPosition, 1, Color.magenta);
				return false;
			}
			foreach (var h in hits)
			{
				var playerInteraction = h.collider.GetComponentInParent<PlayerInteractable>();
				if (playerInteraction == null)
				{
					//MyDebugUtilities.DrawX(h.point, 1, Color.red);
					return true;

				}
				else
				{
					//MyDebugUtilities.DrawX(interactionPosition, 1, Color.green);

				}



			}

			return false;


		}
		private void PlayerOnInteractButton(NewControlButton newControlButton)
		{
			if (isFinished) return;
			if (!isSelected) return;
			if (!canInteract(newControlButton.owner))
			{
				StopListeningToPlayer(newControlButton.owner);
				return;
			}
			OnActionPress?.Invoke(newControlButton.owner);
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (isFinished) return;

			if (other.gameObject == gameObject) return;
			if (other == null) return;
			var life = other.GetComponentInChildren<Life>();
			if (life == null) return;
			if (!life.IsPlayer) return;
			if (!buildingIsInTheWay(life.transform.position))
			{
				ListenToPlayerActionButton(life.player);
			}

		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (isFinished) return;
			if (other.gameObject == gameObject) return;
			var life = other.GetComponentInChildren<Life>();
			if (life == null) return;
			StopListeningToPlayer(life.player);
		}

		protected void StopListeningToPlayer(Player player)
		{
			if (!playersListening.Contains(player)) return;
			playersListening.Remove(player);
			player.Controller.InteractRightShoulder.OnPress -= PlayerOnInteractButton;
			player.Controller.InteractRightShoulder.OnRelease -= Player_OnRelease;
			OnPlayerExits?.Invoke(player);
		}

		protected virtual bool canInteract(Player player) => !isFinished;

		protected virtual bool canEnter(Player player)  {
			if(isFinished) return false;
			if(player == null || player.SpawnedPlayerGO == null) return false;
			return !buildingIsInTheWay(player.SpawnedPlayerGO.transform.position);
		}

		public void Select(Player player)
		{
			if (isSelected) return;
			isSelected = true;
			OnSelected?.Invoke(player);
		}

		public void Deselect(Player player)
		{

			if (!isSelected) return;
			isSelected = false;
			OnDeselected?.Invoke(player);
		}

	}
}
