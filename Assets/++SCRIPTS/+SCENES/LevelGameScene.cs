using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class LevelGameScene : GameScene
{
	public static LevelGameScene CurrentLevelGameScene;

	[Header("Player Spawning")]
	[SerializeField] private List<GameObject> playerSpawnPoints = new();
	[SerializeField] private CinemachineTargetGroup cameraFollowTargetGroup;
	private int currentSpawnPointNumber;

	public static event Action<Type> OnStop;
	public static event Action OnStart;
	public static event Action<Player> OnPlayerSpawned;
	
	public static bool DefenceStyle = true;
	
	// The scene type property (for spawn point system)
	public override Type SceneType => Type.InLevel;

	protected void Start()
	{
		gameObject.SetActive(true);
		StartLevel();
		
		// Register this level with LevelTransition
		if (LevelTransition.I != null)
		{
			LevelTransition.I.SetCurrentScene(SceneType);
		}
	}

	public static void WinGame()
	{
		CurrentLevelGameScene.StopLevel(Type.Endscreen);
	}

	private void StartLevel()
	{
		Debug.Log("start level");
		Players.OnAllJoinedPlayersDead += RestartLevel;
		Player.OnPlayerDies += Player_PlayerDies;
		
		CurrentLevelGameScene = this;
		GlobalManager.IsInLevel = true;
		OnStart?.Invoke();
		
		// Let PlayerManager handle character persistence if needed
		if (PlayerManager.I != null && PlayerManager.ShouldPersistCharacters)
		{
			// PlayerManager will handle persistence
			Debug.Log("PlayerManager will handle character persistence");
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
			if (life.player == player) 
			{
				cameraFollowTargetGroup.RemoveMember(t.Object);
			}
		}
	}
	
	// Method to add player to camera target group (made public for PlayerManager)
	public void AddToCameraFollow(Player player)
	{
		AddMembersToCameraFollowTargetGroup(player);
	}

	private void StopLevel(Type sceneType)
	{
		Players.OnAllJoinedPlayersDead -= RestartLevel;

		var tempTargetsGroup = cameraFollowTargetGroup.Targets.ToList();
		foreach (var t in tempTargetsGroup) cameraFollowTargetGroup.RemoveMember(t.Object);
		
		OnStop?.Invoke(sceneType);
		GlobalManager.IsInLevel = false;
		CurrentLevelGameScene.GoToScene(sceneType);
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
			List<SpawnPointData> entryPoints = new List<SpawnPointData>();
			
			if (LevelTransition.I != null)
			{
				// Try to get the connected entry point
				string connectedId = LevelTransition.I.GetLastConnectedId();
				
				if (!string.IsNullOrEmpty(connectedId) && !LevelTransition.I.IsFirstLoad())
				{
					// Find the specific entry point if there is one
					foreach (var point in FindObjectsOfType<SpawnPoint>())
					{
						if (point.id == connectedId && 
							(point.pointType == SpawnPointType.Entry || point.pointType == SpawnPointType.Both))
						{
							// Get spawn positions distributed around this entry point
							var positions = point.GetSpawnPositionsForPlayers(Players.AllJoinedPlayers.Count);
							int playerIndex = Players.AllJoinedPlayers.IndexOf(player);
							
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
				if (LevelTransition.I != null)
				{
					entryPoints = LevelTransition.I.GetEntryPoints(SceneType);
				}
			}
			
			// If we found entry points in the scene
			if (entryPoints.Count > 0 && LevelTransition.I != null)
			{
				// Get positions distributed around the first entry point (for simplicity)
				var spawnPositions = LevelTransition.I.GetSpawnPositions(SceneType, Players.AllJoinedPlayers.Count);
				int playerIndex = Players.AllJoinedPlayers.IndexOf(player);
				
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
		GoToScene(Type.RestartLevel);
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
			if(player.state != Player.State.Unjoined) 
			{
				SpawnPlayer(player, firstTime);
			}
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
		OnStop?.Invoke(Type.MainMenu);
		GoToScene(Type.MainMenu);
	}
}