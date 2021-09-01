using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
	[SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();
	[SerializeField] private CinemachineTargetGroup cameraFollowTargetGroup;
	private bool isPlaying;
	public Action OnLevelStop;
	public Action<List<Player>> OnLevelStart;


	private List<Player> currentPlayers;

	public void PlayLevel(List<Player> joiningPlayers)
	{
		if (isPlaying) return;
		Debug.Log("play level LEVELS");
		currentPlayers = joiningPlayers;
		gameObject.SetActive(true);
		StartLevel(currentPlayers);
		OnLevelStart?.Invoke(currentPlayers);
	}

	private void RestartLevel()
	{
		isPlaying = false;
		StartCoroutine(WaitAndRestartLevel(currentPlayers));
	}

	private IEnumerator WaitAndRestartLevel(List<Player> joiningPlayers)
	{
		yield return new WaitForSeconds(1f);
		Debug.Log("restarting level...");
		StopLevel();
		PlayLevel(joiningPlayers);
	}

	private void StartLevel(List<Player> joiningPlayers)
	{
		if (isPlaying) return;
		isPlaying = true;
		PLAYERS.OnAllPlayersDead += RestartLevel;
		gameObject.SetActive(true);
		Debug.Log("Level starting" + gameObject.name);
		SpawnPlayers(joiningPlayers);
	}

	private void SpawnPlayers(List<Player> joiningPlayers)
	{
		foreach (var player in joiningPlayers) SpawnPlayer(player);
	}

	private void SpawnPlayer(Player player)
	{
		player.Spawn(spawnPoints[PLAYERS.GetJoinedPlayers().Count].transform.position);
		cameraFollowTargetGroup.AddMember(player.SpawnedPlayerGO.transform, 1, 0);
	}


	public void StopLevel()
	{
		isPlaying = false;
		var tempTargetsGroup = cameraFollowTargetGroup.m_Targets.ToList();
		foreach (var t in tempTargetsGroup) cameraFollowTargetGroup.RemoveMember(t.target);
		MAKER.DestroyAllUnits();
		gameObject.SetActive(false);
		OnLevelStop?.Invoke();
	}
}
