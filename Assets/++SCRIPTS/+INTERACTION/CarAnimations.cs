using System;
using Unity.Cinemachine;
using UnityEngine;

namespace GangstaBean.Interaction
{
	public class CarAnimations : MonoBehaviour
	{
		private static readonly int Start = Animator.StringToHash("Start");

		public CarAccessInteraction carAccessInteraction;
		private bool carStarted;
		private Animator animator;
		public CinemachineCamera cam;


		public void PlayerEntersCar()
		{
			foreach (var player in Players.AllJoinedPlayers) player.SpawnedPlayerGO.SetActive(false);
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
			SFX.I.sounds.car_start_sound.PlayRandomAt(transform.position);
			LevelManager.I.StartWinningGame();
			Debug.Log("heyo");
		}

		public void OnCarStart()
		{
		}

		public void OnCarSkirtOff()
		{
			LevelManager.I.WinGame();
			Debug.Log("win");
		}
	}
}