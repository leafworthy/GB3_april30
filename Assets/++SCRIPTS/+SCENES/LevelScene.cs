using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __SCRIPTS._BANDAIDS;
using __SCRIPTS._COMMON;
using __SCRIPTS._MANAGERS;
using __SCRIPTS._PLAYER;
using __SCRIPTS._UI;
using Cinemachine;
using FunkyCode;
using UnityEngine;

namespace __SCRIPTS._SCENES
{
	public class LevelScene : Scene
	{
		private static bool isPlaying;
		private static LevelDrops _levelDrops;
		private static LightingManager2D _lightingManager;
		public static LevelScene CurrentLevelScene;
	
		[SerializeField] private List<GameObject> spawnPoints = new();
		[SerializeField] private CinemachineTargetGroup cameraFollowTargetGroup;
		[SerializeField] private Camera mainCamera;
		private int currentSpawnPointNumber;

		public static event Action<Type> OnStop;
		public static event Action OnStart;

		protected void Start()
		{
			gameObject.SetActive(true);
			StartLevel();
			ActivateLevelAndSpawnPlayers();
			Players.SetActionMaps(Players.PlayerActionMap);
			Players.OnPlayerJoins += PlayerOnJoins;
		}

		private void PlayerOnJoins(Player obj)
		{
			//SpawnPlayer(obj);
		}

		private void StartLevel()
		{
			if (isPlaying) return;
			_levelDrops = gameObject.AddComponent<LevelDrops>();
			CurrentLevelScene = this;
			OnStart?.Invoke();
		}

		public void StopLevel()
		{
			if (!isPlaying) return;
			isPlaying = false;
		
			CurrentLevelScene = null;
			Players.OnAllPlayersDead -= RestartLevel;

			var tempTargetsGroup = cameraFollowTargetGroup.m_Targets.ToList();
			foreach (var t in tempTargetsGroup) cameraFollowTargetGroup.RemoveMember(t.target);
			Maker.DestroyAllUnits();
			OnStop?.Invoke(Scene.Type.Endscreen);

		}
		public void RestartLevel()
		{
			isPlaying = false;
			StopAndPlayLevel();
		}

		public void SpawnPlayer(Player player)
		{
			currentSpawnPointNumber++;
			player.Spawn(spawnPoints[currentSpawnPointNumber].transform.position);
			AddMembersToCameraFollowTargetGroup(player);
			Players.SetActionMaps(Players.PlayerActionMap);
		}

		public Camera GetCamera()
		{
			return mainCamera;
		}

		private IEnumerator RestartLevelAfterSeconds(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			StopAndPlayLevel();
		}

		private void StopAndPlayLevel()
		{
			StopLevel();
			StartLevel();
		}

		private void ActivateLevelAndSpawnPlayers()
		{
			if (isPlaying) return;
			isPlaying = true;
			gameObject.SetActive(true);
			SpawnPlayers(Players.AllJoinedPlayers);
		}

		private void SpawnPlayers(List<Player> joiningPlayers)
		{
			for (var index = 0; index < joiningPlayers.Count; index++)
			{
				var player = joiningPlayers[index];
				SpawnPlayer(player);
			}
		}

		private void AddMembersToCameraFollowTargetGroup(Player player)
		{
			cameraFollowTargetGroup.AddMember(player.SpawnedPlayerGO.transform, 1, 0);
			if (!player.isUsingMouse)
			{
				var stickTarget = Maker.Make(ASSETS.Players.followStickPrefab).GetComponent<FollowStick>();
				stickTarget.Init(player);
				cameraFollowTargetGroup.AddMember(stickTarget.transform, 1, 0);
				return;
			}
			var mouseTarget = Maker.Make(ASSETS.Players.followMousePrefab).transform;
			cameraFollowTargetGroup.AddMember(mouseTarget, 1, 0);
		}


		public void WaitThenRestartLevel()
		{
			isPlaying = false;
			StartCoroutine(RestartLevelAfterSeconds(1f));
		}

		public void ExitToMainMenu()
		{
			OnStop?.Invoke(Scene.Type.MainMenu);
		}
	}
}
