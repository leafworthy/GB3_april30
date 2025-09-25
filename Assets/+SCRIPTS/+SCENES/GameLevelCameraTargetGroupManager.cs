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

			_cameraFollowTargetGroup = null;
		}

		private void AddPlayerToCameraFollowTargetGroup(Player player)
		{
			Debug.Log("Trying to add player to camera follow target group: " + player.name);

			if (player.SpawnedPlayerGO != null)
			{
				if (player.IsMainPlayer())
				{
					Debug.Log("adding the main character to camera",this);
					cameraFollowTargetGroup.Targets.Clear();
					cameraFollowTargetGroup.AddMember(player.SpawnedPlayerGO.transform, 1, 0);
				}
				else
				{
					Debug.Log("this was not the main character",this);
				}

				var stickTarget = Services.objectMaker.Make(Services.assetManager.Players.followStickPrefab).GetComponent<FollowCursor>();
				stickTarget.Init(player);
			}
		}

		private void Player_PlayerDies(Player deadPlayer)
		{

			RemoveFromCameraFollow(deadPlayer);
			if (Services.playerManager.mainPlayer.isDead()) return;
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
