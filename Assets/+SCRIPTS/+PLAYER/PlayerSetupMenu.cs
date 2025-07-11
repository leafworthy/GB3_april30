using System;
using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlayerSetupMenu : MonoBehaviour
	{
		[SerializeField] private GameObject readyPanel;
		[SerializeField] private GameObject menuPanel;
		public GameObject Visible;
		[SerializeField] private GameObject readyText;

		public CharacterSelectButtons buttons;

		private Player owner;
		private float ignoreInputTime = .5f;
		private bool inputEnabled;



		public event Action<Character> OnCharacterChosen;


		public void StartSetupMenu(Player player)
		{
			owner = player;

			Visible.SetActive(true);
			readyText.SetActive(false);
			ignoreInputTime = Time.time + ignoreInputTime;
			buttons.OnCharacterChosen += Buttons_OnCharacterChosen;
			buttons.Init(player);
		}

		private void Buttons_OnCharacterChosen(Character character)
		{
			if (!inputEnabled) return;
			owner.CurrentCharacter = character;

			readyText.SetActive(true);
			readyPanel.SetActive(true);
			menuPanel.SetActive(false);
			var levelManager = ServiceLocator.Get<LevelManager>();
			levelManager.SpawnPlayerFromInGame(owner);
			OnCharacterChosen?.Invoke(character);
			inputEnabled = false;
			Visible.SetActive(false);
			buttons.OnCharacterChosen -= Buttons_OnCharacterChosen;
		}

		private void Update()
		{
			if (Time.time > ignoreInputTime) inputEnabled = true;
		}
	}
}
