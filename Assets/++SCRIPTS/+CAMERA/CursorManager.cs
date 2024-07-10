using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{

	[SerializeField]private GameObject currentCursor;
	private Vector2 inGameCursorScale = new Vector2(5, 5);
	private Vector2 inMenuCursorScale = new Vector2(.1f, .1f);

	private void Start()
	{
		InitCursor();
		LevelGameScene.OnStart += LevelStarts;
	}

	private void LevelStarts()
	{
		currentCursor.SetActive(true);
	}


	private void InitCursor()
	{
		UnityEngine.Cursor.visible = false;
		//currentCursor = Maker.Make(ASSETS.ui.CursorPrefab);
		currentCursor.transform.localScale = GlobalManager.IsInLevel ? inGameCursorScale : inMenuCursorScale;
		DontDestroyOnLoad(currentCursor);

		LevelGameScene.OnStart += () =>
		{
			currentCursor.gameObject.SetActive(true);
			currentCursor.transform.localScale = inGameCursorScale ;
		};
		PauseManager.OnPause += (x) =>
		{
			currentCursor.gameObject.SetActive(false);
		};
		PauseManager.OnUnpause += (x) =>
		{
			currentCursor.gameObject.SetActive(true);
		};
		LevelGameScene.OnStop += (t) =>
		{
			currentCursor.transform.localScale =  inMenuCursorScale;
			currentCursor.gameObject.SetActive(false);
		};
	}

	private void Update()
	{
		currentCursor.transform.position = Mouse.current.position.ReadValue();
	}

	public static Vector3 GetMousePosition()
	{
		var cam = GetCamera();
		var vec = cam.ScreenToWorldPoint((Vector2)Mouse.current.position.ReadValue());
		vec.z = 0;
		return vec;
	}

	public static Camera GetCamera()
	{
		var cam = Camera.main;
		if (cam == null)
		{
			cam = FindObjectOfType<Camera>();

		}

		return cam;
	}
}