using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class LevelGameScene : GameScene
{
	public static LevelGameScene CurrentLevelGameScene;

	[SerializeField] private List<GameObject> playerSpawnPoints = new();
	[SerializeField] private CinemachineTargetGroup cameraFollowTargetGroup;
	private int currentSpawnPointNumber;

	public static event Action<Type> OnStop;
	public static event Action OnStart;
	public static event Action<Player> OnPlayerSpawned;
	
	public static bool DefenceStyle = true;


	protected void Start()
	{
		gameObject.SetActive(true);
		StartLevel();
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
		ActivateLevelAndSpawnPlayers();
		Players.SetActionMaps(Players.PlayerActionMap);
	}

	private void Player_PlayerDies(Player deadPlayer)
	{
		var tempTargetsGroup = cameraFollowTargetGroup.Targets.ToList();
		foreach (var t in tempTargetsGroup)
		{
			var life = t.Object.GetComponent<Life>();
			if (life == null) continue;
			if (life.player == deadPlayer) cameraFollowTargetGroup.RemoveMember(t.Object);
		}
	}

	private void StopLevel(Type sceneType)
	{
		Players.OnAllJoinedPlayersDead -= RestartLevel;

		var tempTargetsGroup = cameraFollowTargetGroup.Targets.ToList();
		foreach (var t in tempTargetsGroup) cameraFollowTargetGroup.RemoveMember(t.Object);
		
		OnStop?.Invoke(Type.Endscreen);
		GlobalManager.IsInLevel = false;
		CurrentLevelGameScene.GoToScene(sceneType);
		CurrentLevelGameScene = null;
	}

	public void RestartLevel()
	{
		StopAndPlayLevel();
	}

	public void SpawnPlayer(Player player, bool firstTime = false)
	{
		Debug.Log("spawn player");
		var point = playerSpawnPoints[currentSpawnPointNumber];
		if (firstTime)
		{
			currentSpawnPointNumber++;
			player.Spawn(point.transform.position);
		}
		else
			player.Spawn(CursorManager.GetCamera().transform.position);

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
			Debug.Log("player being spawned " + player.name, player);
		}

		Debug.Log("players spawned");
		gameObject.SetActive(true);
		SpawnPlayers(Players.AllJoinedPlayers, true);
	}

	private void SpawnPlayers(List<Player> joiningPlayers, bool firstTime = false)
	{
		foreach (var player in joiningPlayers)
		{
			Debug.Log("player spawned:" + player.name, player);
			if(player.state != Player.State.Unjoined) SpawnPlayer(player, firstTime);
		}
	}

	private void AddMembersToCameraFollowTargetGroup(Player player)
	{
		cameraFollowTargetGroup.AddMember(player.SpawnedPlayerGO.transform, 1, 0);
		var stickTarget = ObjectMaker.Make(ASSETS.Players.followStickPrefab).GetComponent<FollowCursor>();
		stickTarget.Init(player);
	}

	public void ExitToMainMenu()
	{
		OnStop?.Invoke(Type.MainMenu);
		GoToScene(Type.MainMenu);
	}
}