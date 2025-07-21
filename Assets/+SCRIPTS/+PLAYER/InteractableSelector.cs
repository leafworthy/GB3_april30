using System.Collections.Generic;
using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public class InteractableSelector : MonoBehaviour, INeedPlayer
{
	public List<PlayerInteractable> interactables = new();
	private PlayerInteractable selectedInteractable;
	private Player player;
	private AimAbility aimAbility;

	private void FixedUpdate()
	{
		SelectClosestInteractable();
	}

	public void AddInteractable(PlayerInteractable interactable)
	{
		if (interactable == null) return;
		if (interactables.Contains(interactable)) return;
		interactables.Add(interactable);
		SelectClosestInteractable();
	}

	private void SelectClosestInteractable()
	{
		if (interactables.Count == 0)
		{
			selectedInteractable = null;
			return;
		}

		var closest = interactables[0];
		if (closest == null) return;
		var closestDistance = Vector2.Distance(closest.GetInteractionPosition(), GetAimPosition());
		foreach (var interactable in interactables)
		{
			var distance = Vector2.Distance(interactable.GetInteractionPosition(), GetAimPosition());

			if (!(distance < closestDistance)) continue;
			closest = interactable;
			closestDistance = distance;
		}

		if (closest == selectedInteractable)
		{
			if (selectedInteractable.isSelected)
				return;
		}

		if (selectedInteractable != null) selectedInteractable.Deselect(player);
		selectedInteractable = closest;
		selectedInteractable.Select(player);
	}

	private Vector2 GetAimPosition()
	{
		if (aimAbility == null) aimAbility = GetComponentInChildren<AimAbility>();
		return aimAbility.GetAimPoint();
	}

	public void RemoveInteractable(PlayerInteractable interactable)
	{
		if (interactable == null) return;
		if (interactable == selectedInteractable)
		{
			interactable.Deselect(player);
			selectedInteractable = null;
		}

		interactables.Remove(interactable);
		SelectClosestInteractable();
	}

	public void SetPlayer(Player _player)
	{
		player = _player;
		aimAbility = GetComponentInChildren<AimAbility>();
	}
}