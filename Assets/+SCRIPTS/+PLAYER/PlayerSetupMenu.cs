using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlayerSetupMenu : MonoBehaviour
	{
		[SerializeField] private GameObject menuPanel;
		public GameObject Visible;

		public CharacterSelectButtons buttons;

		private Player owner;
		private float ignoreInputTime = .5f;
		private bool inputEnabled;



		public event Action<Character> OnCharacterChosen;


		public void StartSetupMenu(Player player)
		{
			owner = player;

			Visible.SetActive(true);
			ignoreInputTime = Time.time + ignoreInputTime;
			buttons.OnCharacterChosen += Buttons_OnCharacterChosen;
			buttons.Init(player);
		}

		public void StopSetupMenu()
		{
			Visible.SetActive(false);
			buttons.OnCharacterChosen -= Buttons_OnCharacterChosen;
			buttons.CleanUp();
			owner = null;
			inputEnabled = false;
			menuPanel.SetActive(false);
		}

		private void Buttons_OnCharacterChosen(Character character)
		{
			if (!inputEnabled) return;
			owner.CurrentCharacter = character;

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
