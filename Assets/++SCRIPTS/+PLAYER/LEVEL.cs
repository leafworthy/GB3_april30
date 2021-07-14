using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LEVEL : MonoBehaviour
{
	[SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();
	[SerializeField] private CinemachineTargetGroup cameraFollowTargetGroup;
	[SerializeField] public List<IPlayerController> playerControllersInLevel = new List<IPlayerController>();
	private bool isPlaying;

	public void PlayLevel(List<Player> joiningPlayers)
	{
		if (isPlaying) return;
		isPlaying = true;
		Debug.Log("Level starting" + gameObject.name);
		foreach (var player in joiningPlayers)
		{
			SpawnPlayer(player);
		}
		HUD.SetPlayers(joiningPlayers);

		ENEMIES.CollectAllEnemies();
	}

	private void SpawnPlayer(Player player)
	{
		GameObject prefab = GetPrefabFromCharacter(player);
		PLAYERS.AddPlayer(player);
		var spawnedPlayerGO = MAKER.Make(prefab, spawnPoints[playerControllersInLevel.Count].transform.position);
		player.SetSpawnedPlayerGO(spawnedPlayerGO);
		cameraFollowTargetGroup.AddMember(spawnedPlayerGO.transform,1,0);

		IPlayerController newPlayerController;
		if (player.isUsingKeyboard)
			newPlayerController = spawnedPlayerGO.AddComponent<PlayerKeyboardMouseController>();
		else
			newPlayerController = spawnedPlayerGO.AddComponent<PlayerRemoteController>();

		newPlayerController.SetPlayer(player);
		playerControllersInLevel.Add(newPlayerController);

		Debug.Log("Player " + player.playerIndex + "has been spawned as " + player.currentCharacter.ToString());


	}

	private static GameObject GetPrefabFromCharacter(Player player)
	{
		switch (player.currentCharacter)
		{
			case Character.Karrot:
				return ASSETS.CharacterPrefabs.GangstaBeanPlayerPrefab;
				break;
			case Character.Bean:
				return ASSETS.CharacterPrefabs.GangstaBeanPlayerPrefab;
				break;
			case Character.Brock:
				return ASSETS.CharacterPrefabs.BrockLeePlayerPrefab;
				break;
			case Character.Tmato:
				return ASSETS.CharacterPrefabs.BrockLeePlayerPrefab;
				break;
		}
		return null;
	}


	public void EndLevel()
	{
		Debug.Log("level destroy");
		isPlaying = false;
		MAKER.DestroyAllUnits();
		Destroy(gameObject);
	}
}
