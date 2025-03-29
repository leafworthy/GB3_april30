using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace __SCRIPTS
{
	public class GameLevelCameraTargetGroupManager : MonoBehaviour
	{
		private CinemachineTargetGroup cameraFollowTargetGroup;
	

		private void Start()
		{
		
			Player.OnPlayerSpawned += AddMembersToCameraFollowTargetGroup;
			Player.OnPlayerDies += Player_PlayerDies;
			LevelManager.OnStopLevel += LevelManager_OnStopLevel;
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
			Debug.Log("Adding player to camera follow target group");
			if (cameraFollowTargetGroup == null) cameraFollowTargetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
			if (player.SpawnedPlayerGO != null)
			{
				cameraFollowTargetGroup.AddMember(player.SpawnedPlayerGO.transform, 1, 0);
				var stickTarget = ObjectMaker.I.Make(ASSETS.Players.followStickPrefab).GetComponent<FollowCursor>();
				stickTarget.Init(player);
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
				if (life.player == player) cameraFollowTargetGroup.RemoveMember(t.Object);
			}
		}
	}
}