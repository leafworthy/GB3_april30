using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace __SCRIPTS
{
	public class GameLevelCameraTargetGroupManager : ServiceUser
	{
		private CinemachineTargetGroup cameraFollowTargetGroup;


		private void Start()
		{

			levelManager.OnLevelSpawnedPlayer += AddMembersToCameraFollowTargetGroup;
			playerManager.OnPlayerDies += Player_PlayerDies;
			levelManager.OnStopLevel += LevelManager_OnStopLevel;

		}

		private void LevelManager_OnStopLevel(GameLevel level)
		{
			clearCameraFollowTargetGroup();
		}

		private void clearCameraFollowTargetGroup()
		{
			if (cameraFollowTargetGroup == null) return;
			var tempTargetsGroup = cameraFollowTargetGroup.Targets.ToList();
			foreach (var t in tempTargetsGroup)
			{
				cameraFollowTargetGroup.RemoveMember(t.Object);
			}
		}

		private void AddMembersToCameraFollowTargetGroup(Player player)
		{

			if (cameraFollowTargetGroup == null) cameraFollowTargetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
			if (player.SpawnedPlayerGO != null)
			{
				cameraFollowTargetGroup.AddMember(player.SpawnedPlayerGO.transform, 1, 0);
				var stickTarget = objectMaker.Make( assetManager.Players.followStickPrefab).GetComponent<FollowCursor>();
				stickTarget.Init(player);
			}
			else
			{

			}
		}

		private void Player_PlayerDies(Player deadPlayer)
		{
			RemoveFromCameraFollow(deadPlayer);
		}

		// Method to remove player from camera target group
		private void RemoveFromCameraFollow(Player player)
		{
			if (cameraFollowTargetGroup == null) cameraFollowTargetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
			var tempTargetsGroup = cameraFollowTargetGroup.Targets.ToList();
			foreach (var t in tempTargetsGroup)
			{
				var life = t.Object.GetComponent<Life>();
				if (life == null) continue;
				if (life.Player == player) cameraFollowTargetGroup.RemoveMember(t.Object);
			}
		}
	}
}
