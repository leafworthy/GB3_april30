using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace __SCRIPTS.Cursor
{
	public class CursorManager : MonoBehaviour
	{
		[SerializeField] private GameObject player1cursor;
		[SerializeField] private GameObject player2cursor;
		[SerializeField] private GameObject player3cursor;
		[SerializeField] private GameObject player4cursor;
		private Vector2 inGameCursorScale = new(5, 5);
		private Vector2 inMenuCursorScale = new(0, 0);
		private PlayerCursor currentCursor;
		[SerializeField] private List<PlayerCursor> currentCursors = new();

		private bool isActive;
		private static Camera cam;

		private void Start()
		{
			UnityEngine.Cursor.visible = false;
			LevelManager.I.OnStartLevel += LevelStartsLevel;
			LevelManager.I.OnPlayerSpawned += InitCursor;
		}

		private void LevelStartsLevel(GameLevel level)
		{
			isActive = true;
			foreach (var player in Players.I.AllJoinedPlayers)
			{
				InitCursor(player);
			}

		}

		private void OnDisable()
		{
			LevelManager.I.OnStartLevel -= LevelStartsLevel;
			LevelManager.I.OnPlayerSpawned -= InitCursor;
			currentCursors.Clear();
		}

		private void InitCursor(Player player)
		{
			//if (!player.isUsingMouse) return;

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

			if (currentCursor == null)
			{
				return;
			}
			currentCursor.owner = player;
			currentCursor.transform.localScale = inGameCursorScale;
			//DontDestroyOnLoad(currentCursor);
			var image = currentCursor.GetComponentInChildren<Image>();
			if (image != null) image.color = player.playerColor;
			LevelManager.I.OnStartLevel += (t) => { SetCursorsActive(true); };
			PauseManager.I.OnPause += x =>
			{
				SetCursorsActive(false);
			};
			PauseManager.I.OnUnpause += x => { SetCursorsActive(true); };
			LevelManager.I.OnStopLevel += t => { SetCursorsActive(false); };
			currentCursor.gameObject.SetActive(true);
			currentCursors.Add(currentCursor);
			SetCursorsActive(true);
			if (Players.I.AllJoinedPlayers.Count < 4)
			{
				player4cursor.SetActive(false);
			}

			if (Players.I.AllJoinedPlayers.Count < 3)
			{
				player3cursor.SetActive(false);
			}

			if (Players.I.AllJoinedPlayers.Count < 2)
			{
				player2cursor.SetActive(false);
			}
		}



		private void SetCursorsActive(bool active)
		{
			foreach (var cursor in currentCursors)
			{
				if (cursor == null) continue;
				if (cursor.owner == null) continue;
				cursor.gameObject.SetActive(active);
			}

			isActive = active;
		}

		private void Update()
		{
			UnityEngine.Cursor.visible = false;
			if (!isActive) return;
			foreach (var cursor in currentCursors)
			{
				UpdateCursor(cursor);
			}
		}

		private void UpdateCursor(PlayerCursor cursor)
		{
			if(cursor == null) return;
			if (cursor.owner == null) return;
			if (cursor.owner.SpawnedPlayerGO == null) return;
			var aim = cursor.aimAbility;
			if (aim == null) return;
			if(cursor.aimAbility.hasEnoughMagnitude() )
			{
				cursor.gameObject.SetActive(true);
				cursor.gameObject.transform.position = cursor.aimAbility.GetAimPoint();
			}
			else
			{
				cursor.gameObject.SetActive(false);
			}
		}

		public static Vector3 GetMousePosition()
		{
			var cam = GetCamera();
			if (cam == null) return Vector3.zero;
			var vec = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			vec.z = 0;
			return vec;
		}

		public static Camera GetCamera()
		{
			if(cam != null) return cam;
			cam = Camera.main;
			if (cam == null) cam = FindFirstObjectByType<Camera>();

			return cam;
		}
	}
}
