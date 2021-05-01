using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace _SCRIPTS
{
	public class LEVEL : Singleton<LEVEL>
	{
		[SerializeField] private List<GameObject> spawnPoints;
		[SerializeField] private CinemachineTargetGroup cameraFollowTargetGroup;
		[SerializeField] public List<PlayerController> playerControllersInLevel;

		public void PlayLevel(List<Player> joiningPlayers)
		{
			Debug.Log("Level starting");
			foreach (var player in joiningPlayers)
			{
				SpawnPlayer(player);
			}

			GAME.OnGameEnd += GAME_OnGameEnd;
			ENEMIES.CollectAllEnemies();
		}

		private void GAME_OnGameEnd()
		{
			GAME.OnGameEnd -= GAME_OnGameEnd;
			Destroy(gameObject);
		}

		private void SpawnPlayer(Player player)
		{
			GameObject prefab = GetPrefabFromCharacter(player);

			var newPlayer = MAKER.Make(prefab, spawnPoints[playerControllersInLevel.Count].transform.position);
			var newPlayerController = newPlayer.GetComponent<PlayerController>();
			newPlayerController.SetPlayer(player);
			playerControllersInLevel.Add(newPlayerController);
			PLAYERS.AddPlayer(newPlayer.GetComponent<DefenceHandler>());
			cameraFollowTargetGroup.AddMember(newPlayer.transform,1,0);
			Debug.Log("Player " + player.playerIndex + "has been spawned as " + player.currentCharacter.ToString());
		}

		private static GameObject GetPrefabFromCharacter(Player player)
		{
			switch (player.currentCharacter)
			{
				case Character.Karrot:
					return ASSETS.players.GangstaBeanPlayerPrefab;
					break;
				case Character.Bean:
					return ASSETS.players.GangstaBeanPlayerPrefab;
					break;
				case Character.Brock:
					return ASSETS.players.BrockLeePlayerPrefab;
					break;
				case Character.Tmato:
					return ASSETS.players.BrockLeePlayerPrefab;
					break;
			}
			return null;
		}

		public void CleanUp()
		{
			Destroy(gameObject);
		}
	}
}
