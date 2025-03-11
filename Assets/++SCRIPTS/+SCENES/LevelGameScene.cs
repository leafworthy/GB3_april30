using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class LevelGameScene : GameScene
{
	public static LevelGameScene CurrentLevelGameScene;

	[Header("Player Spawning"), SerializeField]
	
	private List<GameObject> playerSpawnPoints = new();
	[SerializeField] private CinemachineTargetGroup cameraFollowTargetGroup;
	private int currentSpawnPointNumber;

	public static event Action<SceneDefinition> OnStop;
	public static event Action<SceneDefinition> OnStart;
	public static event Action<Player> OnPlayerSpawned;


	protected void Start()
	{
		gameObject.SetActive(true);
		StartLevel();

		// Register this level with SceneLoader
		if (SceneLoader.I != null) SceneLoader.I.SetCurrentScene(sceneDefinition);
	}

	

	private void StartLevel()
	{
		Debug.Log("start level");
		Players.OnAllJoinedPlayersDead += RestartLevel;
		Player.OnPlayerDies += Player_PlayerDies;

		CurrentLevelGameScene = this;
		GlobalManager.IsInLevel = true;
		OnStart?.Invoke(sceneDefinition);

		// Let Players singleton handle character persistence if needed
		if (Players.ShouldPersistCharacters)
		{
			// Players will handle character persistence
			Debug.Log("Players will handle character persistence");
		}
		else
		{
			// Normal flow - spawn new characters
			ActivateLevelAndSpawnPlayers();
		}

		Players.SetActionMaps(Players.PlayerActionMap);
	}

	private void Player_PlayerDies(Player deadPlayer)
	{
		RemoveFromCameraFollow(deadPlayer);
	}

	// Method to remove player from camera target group
	public void RemoveFromCameraFollow(Player player)
	{
		var tempTargetsGroup = cameraFollowTargetGroup.Targets.ToList();
		foreach (var t in tempTargetsGroup)
		{
			var life = t.Object.GetComponent<Life>();
			if (life == null) continue;
			if (life.player == player) cameraFollowTargetGroup.RemoveMember(t.Object);
		}
	}

	// Method to add player to camera target group (made public for PlayerManager)
	public void AddToCameraFollow(Player player)
	{
		AddMembersToCameraFollowTargetGroup(player);
	}

	private void StopLevel(SceneDefinition sceneDefinition)
	{
		Players.OnAllJoinedPlayersDead -= RestartLevel;

		var tempTargetsGroup = cameraFollowTargetGroup.Targets.ToList();
		foreach (var t in tempTargetsGroup)
		{
			cameraFollowTargetGroup.RemoveMember(t.Object);
		}

		OnStop?.Invoke(sceneDefinition);

		GlobalManager.IsInLevel = false;
		ASSETS.GoToScene(sceneDefinition);
		CurrentLevelGameScene = null;
	}

	public void RestartLevel()
	{
		StopAndPlayLevel();
	}

	// Overload to specify exact spawn position
	public void SpawnPlayer(Player player, bool firstTime, Vector2 specificPosition)
	{
		Debug.Log($"Spawning player {player.playerIndex} at specific position {specificPosition}");

		// Spawn the player at the specified position
		player.Spawn(specificPosition);

		// Complete setup
		AddMembersToCameraFollowTargetGroup(player);
		Players.SetActionMaps(Players.PlayerActionMap);
		OnPlayerSpawned?.Invoke(player);
	}

	// Original method with default spawn points
	public void SpawnPlayer(Player player, bool firstTime = false)
	{
		Debug.Log($"Spawning player {player.playerIndex}");
		Vector2 spawnPosition;

		// Determine spawn position
		if (firstTime)
		{
			// Check if we have multiple entry points in the scene
			var entryPoints = new List<SpawnPointData>();

			if (SceneLoader.I != null)
			{
				// Try to get the connected entry point
				var connectedId = SceneLoader.I.GetLastConnectedId();

				if (!string.IsNullOrEmpty(connectedId) && !SceneLoader.I.IsFirstLoad())
				{
					// Find the specific entry point if there is one
					foreach (var point in FindObjectsOfType<SpawnPoint>())
					{
						if (point.id == connectedId && (point.pointType == SpawnPointType.Entry || point.pointType == SpawnPointType.Both))
						{
							// Get spawn positions distributed around this entry point
							var positions = point.GetSpawnPositionsForPlayers(Players.AllJoinedPlayers.Count);
							var playerIndex = Players.AllJoinedPlayers.IndexOf(player);

							if (playerIndex >= 0 && playerIndex < positions.Count)
							{
								// Spawn at the calculated position
								player.Spawn(positions[playerIndex]);
								AddMembersToCameraFollowTargetGroup(player);
								Players.SetActionMaps(Players.PlayerActionMap);
								OnPlayerSpawned?.Invoke(player);
								return;
							}
						}
					}
				}

				// If no specific entry point found or it's first load, try to find any entry point
				if (SceneLoader.I != null)
				{
					// Use scene definition if available
					if (sceneDefinition != null) entryPoints = SceneLoader.I.GetEntryPoints(sceneDefinition);
				}
			}

			// If we found entry points in the scene
			if (entryPoints.Count > 0 && SceneLoader.I != null)
			{
				// Get positions distributed around the first entry point (for simplicity)
				List<Vector2> spawnPositions;

				// Try with scene definition first
				if (sceneDefinition == null) return;

				spawnPositions = SceneLoader.I.GetSpawnPositions(sceneDefinition, Players.AllJoinedPlayers.Count);

				var playerIndex = Players.AllJoinedPlayers.IndexOf(player);

				if (playerIndex >= 0 && playerIndex < spawnPositions.Count)
				{
					// Spawn at the calculated position
					spawnPosition = spawnPositions[playerIndex];
					player.Spawn(spawnPosition);
					AddMembersToCameraFollowTargetGroup(player);
					Players.SetActionMaps(Players.PlayerActionMap);
					OnPlayerSpawned?.Invoke(player);
					return;
				}
			}

			// Otherwise use default spawn points from the scene
			if (currentSpawnPointNumber < playerSpawnPoints.Count)
			{
				var point = playerSpawnPoints[currentSpawnPointNumber];
				currentSpawnPointNumber++;
				player.Spawn(point.transform.position);
			}
			else
			{
				// Fallback if out of spawn points
				player.Spawn(transform.position);
			}
		}
		else
		{
			// Not first time (respawn)
			player.Spawn(CursorManager.GetCamera().transform.position);
		}

		AddMembersToCameraFollowTargetGroup(player);
		Players.SetActionMaps(Players.PlayerActionMap);
		OnPlayerSpawned?.Invoke(player);
	}

	private void StopAndPlayLevel()
	{
		LevelManager.I.RestartCurrentLevel();
	}

	private void ActivateLevelAndSpawnPlayers()
	{
		foreach (var player in Players.AllJoinedPlayers)
		{
			Debug.Log("Player being prepared for spawn: " + player.name, player);
		}

		Debug.Log("Spawning players");
		gameObject.SetActive(true);
		SpawnPlayers(Players.AllJoinedPlayers, true);
	}

	private void SpawnPlayers(List<Player> joiningPlayers, bool firstTime = false)
	{
		foreach (var player in joiningPlayers)
		{
			Debug.Log("Spawning player: " + player.name, player);
			if (player.state != Player.State.Unjoined) SpawnPlayer(player, firstTime);
		}
	}

	private void AddMembersToCameraFollowTargetGroup(Player player)
	{
		if (player.SpawnedPlayerGO != null)
		{
			cameraFollowTargetGroup.AddMember(player.SpawnedPlayerGO.transform, 1, 0);
			var stickTarget = ObjectMaker.Make(ASSETS.Players.followStickPrefab).GetComponent<FollowCursor>();
			stickTarget.Init(player);
		}
	}

	public void ExitToMainMenu()
	{
		StopLevel(ASSETS.Scenes.mainMenu);
	}
}