using System;
using Unity.Cinemachine;
using UnityEngine;

namespace __SCRIPTS
{
	public class CarAnimations : ServiceUser
	{
		private static readonly int Start = Animator.StringToHash("Start");

		public CarAccessInteraction carAccessInteraction;
		private bool carStarted;
		private Animator animator;
		public CinemachineCamera cam;


		public void PlayerEntersCar()
		{
			foreach (var player in playerManager.AllJoinedPlayers) player.SpawnedPlayerGO.SetActive(false);
		}

		private void OnEnable()
		{
			carAccessInteraction.OnCarAccessActionPressed += OnCarAccessActionPressed;
			animator = GetComponentInChildren<Animator>();
		}

		private void OnCarAccessActionPressed(Player player)
		{
			if (carStarted) return;
			carStarted = true;
			animator.SetTrigger(Start);
			var brain = FindFirstObjectByType<CinemachineBrain>();
			brain.DefaultBlend.Time = 2.0f; // Adjust blend time
			cam.Priority = 12;
		}

		private bool pressed;

		public void OnCarEnter()
		{
			sfx.sounds.car_start_sound.PlayRandomAt(transform.position);
			levelManager.StartWinningGame();

		}

		public void OnCarStart()
		{
		}

		public void OnCarSkirtOff()
		{
			levelManager.WinGame();

		}
	}
}
