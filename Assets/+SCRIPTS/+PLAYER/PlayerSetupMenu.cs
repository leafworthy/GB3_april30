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

		void Start()
		{
			Visible.SetActive(false);
		}

		public void StartSetupMenu(Player player)
		{
			owner = player;

			Visible.SetActive(true);
			ignoreInputTime = Time.time + ignoreInputTime;
			buttons.OnCharacterChosen += Buttons_OnCharacterChosen;
			buttons.Init(player);
		}

		private void StopSetupMenu()
		{
			Visible.SetActive(false);
			buttons.OnCharacterChosen -= Buttons_OnCharacterChosen;
			buttons.CleanUp();
			owner = null;
			inputEnabled = false;
		}

		private void Buttons_OnCharacterChosen(Character character)
		{
			if (!inputEnabled) return;
			owner.CurrentCharacter = character;
			Services.levelManager.SpawnPlayerFromPlayerSetupMenu(owner);
			OnCharacterChosen?.Invoke(character);
			StopSetupMenu();
		}

		private void Update()
		{
			if (Time.time > ignoreInputTime) inputEnabled = true;
		}


	}
}
