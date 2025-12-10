using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace __SCRIPTS.Cursor
{
	public class CursorManager : MonoBehaviour
	{
		[SerializeField] GameObject player1cursor;
		[SerializeField] GameObject player2cursor;
		[SerializeField] GameObject player3cursor;
		[SerializeField] GameObject player4cursor;
		Vector2 inGameCursorScale = new(5, 5);
		PlayerCursor currentCursor;
		[SerializeField] List<PlayerCursor> currentCursors = new();

		bool isActive;
		static Camera cam;

		[RuntimeInitializeOnLoadMethod]
		static void ResetStatics()
		{
			cam = null;
		}

		void Start()
		{
			UnityEngine.Cursor.visible = false;
			Services.levelManager.OnStartLevel += LevelStartsLevel;
			Services.levelManager.OnLevelSpawnedPlayerFromLevel += InitCursor;
		}

		void LevelStartsLevel(GameLevel level)
		{
			isActive = true;
			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				InitCursor(player);
			}
		}

		void OnDisable()
		{
			Services.levelManager.OnStartLevel -= LevelStartsLevel;
			Services.levelManager.OnStartLevel -= Level_OnStartLevel;
			Services.pauseManager.OnPause -= Pause_OnPause;
			Services.pauseManager.OnUnpause -= Pause_OnUnPause;
			Services.levelManager.OnStopLevel -= Level_OnStopLevel;
			Services.levelManager.OnLevelSpawnedPlayerFromLevel -= InitCursor;
			currentCursors.Clear();
		}

		void InitCursor(Player player)
		{
			player.OnPlayerDies += Player_OnPlayerDies;

			switch (player.playerIndex)
			{
				case 0:
					currentCursor = player1cursor.GetComponent<PlayerCursor>();
					break;
				case 1:
					currentCursor = player2cursor.GetComponent<PlayerCursor>();
					break;
				case 2:
					currentCursor = player3cursor.GetComponent<PlayerCursor>();
					break;
				case 3:
					currentCursor = player4cursor.GetComponent<PlayerCursor>();
					break;
			}

			if (currentCursor == null) return;
			currentCursor.owner = player;
			currentCursor.transform.localScale = inGameCursorScale;
			var image = currentCursor.GetComponentInChildren<Image>();
			if (image != null) image.color = player.playerColor;
			Services.levelManager.OnStartLevel += Level_OnStartLevel;
			Services.pauseManager.OnPause += Pause_OnPause;
			Services.pauseManager.OnUnpause += Pause_OnUnPause;
			Services.levelManager.OnStopLevel += Level_OnStopLevel;
			currentCursor.gameObject.SetActive(true);
			currentCursors.Add(currentCursor);
			SetCursorsActive(true);
			if (Services.playerManager.AllJoinedPlayers.Count < 4) player4cursor.SetActive(false);

			if (Services.playerManager.AllJoinedPlayers.Count < 3) player3cursor.SetActive(false);

			if (Services.playerManager.AllJoinedPlayers.Count < 2) player2cursor.SetActive(false);
		}

		void Level_OnStopLevel(GameLevel obj)
		{
			SetCursorsActive(false);
		}

		void Pause_OnUnPause(Player obj)
		{
			SetCursorsActive(true);
		}

		void Level_OnStartLevel(GameLevel obj)
		{
			SetCursorsActive(true);
		}

		void Pause_OnPause(Player player)
		{
			SetCursorsActive(false);
		}


		void Player_OnPlayerDies(Player deadPlayer, bool b)
		{
			foreach (var cursor in currentCursors)
			{
				if (cursor == null) continue;
				if (cursor.owner == deadPlayer)
				{
					cursor.gameObject.SetActive(false);
					cursor.owner = null;
					deadPlayer.OnPlayerDies -= Player_OnPlayerDies;
					return;
				}
			}
		}

		void SetCursorsActive(bool active)
		{
			foreach (var cursor in currentCursors)
			{
				if (cursor == null) continue;
				if (cursor.owner == null) continue;
				cursor.gameObject.SetActive(active);
			}

			isActive = active;
		}

		void Update()
		{
			UnityEngine.Cursor.visible = false;
			if (!isActive) return;
			foreach (var cursor in currentCursors)
			{
				UpdateCursor(cursor);
			}
		}

		void UpdateCursor(PlayerCursor cursor)
		{
			if (cursor == null) return;
			if (cursor.owner == null) return;
			if (cursor.owner.SpawnedPlayerGO == null) return;
			var aim = cursor.aimAbility;
			if (aim == null) return;
			if (cursor.aimAbility.hasEnoughMagnitude())
			{
				cursor.gameObject.SetActive(true);
				cursor.gameObject.transform.position = cursor.aimAbility.GetAimPoint();
			}
			else
				cursor.gameObject.SetActive(false);
		}

		public static Vector3 GetMousePosition()
		{
			cam = GetCamera();
			if (cam == null) return Vector3.zero;
			var vec = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			vec.z = 0;
			return vec;
		}

		public static Camera GetCamera()
		{
			if (cam != null) return cam;
			cam = Camera.main;
			if (cam == null) cam = FindFirstObjectByType<Camera>();

			return cam;
		}
	}
}
