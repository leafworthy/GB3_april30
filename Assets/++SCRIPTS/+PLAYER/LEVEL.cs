using System.Collections.Generic;
using System.Linq;
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
		GameObject o;
		(o = gameObject).SetActive(true);
		isPlaying = true;
		Debug.Log("Level starting" + o.name);
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
		if (prefab == null)
		{
			Debug.Break();
		}
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
			case Character.Bean:
				return ASSETS.CharacterPrefabs.GangstaBeanPlayerPrefab;
			case Character.Brock:
				return ASSETS.CharacterPrefabs.BrockLeePlayerPrefab;
			case Character.Tmato:
				return ASSETS.CharacterPrefabs.BrockLeePlayerPrefab;
		}
		return null;
	}


	public void EndLevel()
	{
		Debug.Log("level deactivated");
		isPlaying = false;
		playerControllersInLevel.Clear();
		var tempTargetsGroup = cameraFollowTargetGroup.m_Targets.ToList();
		foreach (var t in tempTargetsGroup)
		{
			cameraFollowTargetGroup.RemoveMember(t.target);
		}
		MAKER.DestroyAllUnits();
		gameObject.SetActive(false);
	}
}
