using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
	public event Action<Player> OnActionPress;
	public event Action<Player> OnActionRelease;
	public event Action<Player> OnPlayerEnters;
	public event Action<Player> OnPlayerExits;

	private List<Player> playersListening = new();
	public bool isSelected;
	public event Action<Player> OnSelected;
	public event Action<Player> OnDeselected;

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == gameObject) return;

		ListenToPlayerActionButton(other);
	}

	private void ListenToPlayerActionButton(Collider2D other)
	{
		var stats = other.GetComponent<Life>();
		if (stats == null)
		{
			stats = other.GetComponentInParent<Life>();
			if (stats == null) return;
		}

		ListenToPlayerActionButton(stats.player);
	}

	protected void ListenToPlayerActionButton(Player player)
	{
		if (!canEnter(player)) return;
		if (playersListening.Contains(player)) return;
		playersListening.Add(player);

		if (!player.IsPlayer()) return;

		player.Controller.ActionButton.OnPress += Player_OnActionButton;
		player.Controller.ActionButton.OnRelease += Player_OnRelease;

		OnPlayerEnters?.Invoke(player);
	}

	private void Player_OnRelease(NewControlButton newControlButton)
	{
		if (!isSelected) return;
		OnActionRelease?.Invoke(newControlButton.owner);
		if (!canInteract(newControlButton.owner)) StopListeningToPlayer(newControlButton.owner);
	}

	private void Player_OnActionButton(NewControlButton newControlButton)
	{
		if (!isSelected) return;
		OnActionPress?.Invoke(newControlButton.owner);
		if (!canInteract(newControlButton.owner)) StopListeningToPlayer(newControlButton.owner);
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject == gameObject) return;
		var life = other.GetComponentInChildren<Life>();
		StopListeningToPlayer(life.player);
	}

	protected void StopListeningToPlayer(Player player)
	{
		if (!playersListening.Contains(player)) return;
		playersListening.Remove(player);
		player.Controller.ActionButton.OnPress -= Player_OnActionButton;
		player.Controller.ActionButton.OnRelease -= Player_OnRelease;
		OnPlayerExits?.Invoke(player);
	}

	protected virtual bool canInteract(Player player) => true;

	protected virtual bool canEnter(Player player) => true;

	public void Select(Player player)
	{
		if (isSelected) return;
		isSelected = true;
		Debug.Log("Select: " + name);
		OnSelected?.Invoke(player);
	}

	public void Deselect(Player player)
	{
		if (!isSelected) return;
		isSelected = false;
		Debug.Log("Deselect: " + name);
		OnDeselected?.Invoke(player);
	}

	public Vector2 GetInteractionPosition()
	{
		var interactableIndicator = GetComponentInChildren<InteractionIndicator>();
		return interactableIndicator.indicatorOffset + transform.position;
	}
}