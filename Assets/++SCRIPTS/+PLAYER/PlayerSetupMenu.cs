using System;
using GangstaBean.UI.HUD;
using UnityEngine;

namespace GangstaBean.Player
{
	public class PlayerSetupMenu : MonoBehaviour, INeedPlayer
	{
		[SerializeField] private GameObject readyPanel;
		[SerializeField] private GameObject menuPanel;
		public GameObject Visible;
		[SerializeField] private GameObject readyText;

		public CharacterSelectButtons buttons;

		private Player owner;
		private float ignoreInputTime = .5f;
		private bool inputEnabled;

		private void Start()
		{
			Debug.Log("player setup menu start", this);
			//Visible.SetActive(false);
		}

		private void OnEnable()
		{
			Debug.Log("player setup menu enabled", this);
			LevelManager.I.OnStopLevel += LevelGameSceneOnStopLevel;
		}

		private void OnDisable()
		{
			LevelManager.I.OnStopLevel -= LevelGameSceneOnStopLevel;
		}

		private void LevelGameSceneOnStopLevel(GameLevel gameLevel)
		{
			Unsetup();
		}

		private void Unsetup()
		{
			owner = null;
			Visible.SetActive(false);
		}

		public event Action<Character> OnCharacterChosen;

		public void SetPlayer(Player _player)
		{
			owner = _player;
		}

		public void StartSetupMenu(Player player)
		{
			SetPlayer(player);
			Debug.Log(  player.name + " is setting up");
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
			Debug.Log("Character chosen: " + character);
			readyText.SetActive(true);
			readyPanel.SetActive(true);
			menuPanel.SetActive(false);
			LevelManager.I.SpawnPlayerFromInGame(owner);
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