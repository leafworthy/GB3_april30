using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace __SCRIPTS
{
	public class GameLevelCameraTargetGroupManager : MonoBehaviour
	{
		private CinemachineTargetGroup cameraFollowTargetGroup => FindFirstObjectByType<CinemachineTargetGroup>();


		private void Start()
		{

			Services.levelManager.OnLevelSpawnedPlayerFromLevel += AddPlayerFromLevelToCameraFollowTargetGroup;
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

		private void AddPlayerFromLevelToCameraFollowTargetGroup(Player player)
		{
			Debug.Log("Trying to add player to camera follow target group: " + player.name);

			if (player.SpawnedPlayerGO != null)
			{
				if (player.IsMainPlayer())
				{
					cameraFollowTargetGroup.Targets.Clear();
					cameraFollowTargetGroup.AddMember(player.SpawnedPlayerGO.transform, 1, 0);
				}

				Debug.Log("NOT main player, not adding to camera follow target group: " + player.name);

				var stickTarget = Services.objectMaker.Make(Services.assetManager.Players.followStickPrefab).GetComponent<FollowCursor>();
				stickTarget.Init(player);
			}
		}

		private void Player_PlayerDies(Player deadPlayer)
		{

			RemoveFromCameraFollow(deadPlayer);
			if (Services.playerManager.mainPlayer.isDead()) return;
			AddPlayerFromLevelToCameraFollowTargetGroup(Services.playerManager.mainPlayer);
		}


		// Method to remove player from camera target group
		private void RemoveFromCameraFollow(Player player)
		{
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
