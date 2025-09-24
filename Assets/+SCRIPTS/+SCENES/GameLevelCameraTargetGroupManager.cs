using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace __SCRIPTS
{
	public class GameLevelCameraTargetGroupManager : MonoBehaviour
	{
		private CinemachineTargetGroup cameraFollowTargetGroup => _cameraFollowTargetGroup ??= FindFirstObjectByType<CinemachineTargetGroup>();
		private CinemachineTargetGroup _cameraFollowTargetGroup;


		private void Start()
		{

			Services.levelManager.OnLevelSpawnedPlayer += AddPlayerToCameraFollowTargetGroup;
			Services.playerManager.OnPlayerDies += Player_PlayerDies;
			Services.levelManager.OnStopLevel += LevelManager_OnStopLevel;

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

		private void AddPlayerToCameraFollowTargetGroup(Player player)
		{

			if (player.SpawnedPlayerGO != null)
			{
				if(cameraFollowTargetGroup.Targets.Count == 0)
				{
					cameraFollowTargetGroup.AddMember(player.SpawnedPlayerGO.transform, 1, 0);
				}
				var stickTarget = Services.objectMaker.Make(Services.assetManager.Players.followStickPrefab).GetComponent<FollowCursor>();
				stickTarget.Init(player);
			}
		}

		private void Player_PlayerDies(Player deadPlayer)
		{

			RemoveFromCameraFollow(deadPlayer);
			AddPlayerToCameraFollowTargetGroup(Services.playerManager.mainPlayer);
		}


		// Method to remove player from camera target group
		private void RemoveFromCameraFollow(Player player)
		{
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
