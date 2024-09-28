using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using FunkyCode;
using UnityEngine;

public class LevelGameScene : GameScene
{
	private static bool isPlaying;
	private static LevelDrops _levelDrops;
	private static LightingManager2D _lightingManager;
	public static LevelGameScene CurrentLevelGameScene;
	
	[SerializeField] private List<GameObject> spawnPoints = new();
	[SerializeField] private CinemachineTargetGroup cameraFollowTargetGroup;
	[SerializeField] private Camera mainCamera;
	private int currentSpawnPointNumber;

	public static event Action<Type> OnStop;
	public static event Action OnStart;
	public static event Action<Player> OnPlayerSpawned;

	protected void Start()
	{
		gameObject.SetActive(true);
		StartLevel();
	}


	private void StartLevel()
	{
		Debug.Log("level start");
		Players.OnAllPlayersDead += RestartLevel;
		_levelDrops = gameObject.AddComponent<LevelDrops>();
		CurrentLevelGameScene = this;
		GlobalManager.IsInLevel = true;
		OnStart?.Invoke();
		ActivateLevelAndSpawnPlayers();
		Players.SetActionMaps(Players.PlayerActionMap);
	}

	private void StopLevel()
	{
		
		Players.OnAllPlayersDead -= RestartLevel;

		var tempTargetsGroup = cameraFollowTargetGroup.m_Targets.ToList();
		foreach (var t in tempTargetsGroup) cameraFollowTargetGroup.RemoveMember(t.target);
		Maker.DestroyAllUnits();
		OnStop?.Invoke(GameScene.Type.Endscreen);
		CurrentLevelGameScene = null;
		GlobalManager.IsInLevel = false;

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

	private IEnumerator RestartLevelAfterSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		StopAndPlayLevel();
	}

	private void StopAndPlayLevel()
	{
		//StopLevel();
		GoToScene(Type.RestartLevel);
	}

	private void ActivateLevelAndSpawnPlayers()
	{
	
		gameObject.SetActive(true);
		SpawnPlayers(Players.AllJoinedPlayers);
	}

	private void SpawnPlayers(List<Player> joiningPlayers)
	{
		for (var index = 0; index < joiningPlayers.Count; index++)
		{
			var player = joiningPlayers[index];
			SpawnPlayer(player);
			Debug.Log("player spawned");
			OnPlayerSpawned?.Invoke(player);
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
		OnStop?.Invoke(GameScene.Type.MainMenu);
		GoToScene(GameScene.Type.MainMenu);
	}
}