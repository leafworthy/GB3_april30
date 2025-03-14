﻿using System;
using TMPro;
using UnityEngine;

public class PlayerSetupMenu : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private GameObject readyPanel;
	[SerializeField] private GameObject menuPanel;

	[SerializeField] private GameObject readyText;
		

	public CharacterSelectButtons buttons;

	private Player owner;
	private float ignoreInputTime = .5f;
	private bool inputEnabled;

	private void OnEnable()
	{
		LevelGameScene.OnStop += LevelGameScene_OnStop;
	}

	private void LevelGameScene_OnStop(SceneDefinition sceneDefinition)
	{
		Unsetup();
	}

	private void Unsetup()
	{
		owner = null;
		gameObject.SetActive(false);
		readyText.SetActive(false);
		readyPanel.SetActive(false);
		menuPanel.SetActive(true);
	}

	public event Action<Player> OnCharacterChosen;
	public void Setup(Player player, HUDSlot hudSlot)
	{
		Debug.Log("made it in menu", this);
		owner = player;
		gameObject.SetActive(true);
		readyText.SetActive(false);
		ignoreInputTime = Time.time + ignoreInputTime;
		buttons.OnCharacterChosen += Buttons_OnCharacterChosen;
		buttons.Init(player);
	}

	private void Buttons_OnCharacterChosen(Character character)
	{
		if (!inputEnabled) return;
		owner.CurrentCharacter = character;
		Debug.Log("Character chosen: " + character);
		readyText.SetActive(true);
		readyPanel.SetActive(true);
		menuPanel.SetActive(false);
		LevelGameScene.CurrentLevelGameScene.SpawnPlayer(owner);
		OnCharacterChosen?.Invoke(owner);
		inputEnabled = false;
		gameObject.SetActive(false);
		buttons.OnCharacterChosen -= Buttons_OnCharacterChosen;
	}

	private void Update()
	{
		if (Time.time > ignoreInputTime) inputEnabled = true;
	}

}