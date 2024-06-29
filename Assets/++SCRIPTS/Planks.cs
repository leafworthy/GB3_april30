using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Planks : Defence
{
	public List<GameObject> planks;
	private TimedInteraction repairInteraction;

	private void Start()
	{
		defence = 0;
		repairInteraction.OnTimeComplete += OnRepairComplete;
		repairInteraction.OnPlayerEnters += OnPlayerEnters;
		repairInteraction.OnPlayerExits += OnPlayerExits;
	}

	public override void Defend(Attack attack)
	{
		defence--;
		if (Refresh()) return;
		if (defence <= 0) return;
		base.Defend(attack);
	}

	[Button]
	private bool Refresh()
	{
		foreach (var go in planks) go.SetActive(false);

		if (defence >  planks.Count) return true;
		for (var i = 0; i < defence; i++) planks[i].SetActive(true);
		return false;
	}

	private void OnPlayerEnters(Player obj)
	{
		obj.Say("E - Barricade", 0);
	}

	private void OnPlayerExits(Player player)
	{
		player.StopSaying();
	}

	private void OnRepairComplete(Player player)
	{
		defence++;
	}
}