using __SCRIPTS.Cursor;
using Sirenix.OdinInspector;
using UnityEngine;


public class EnemyPlacer : MonoBehaviour
{
	public GameObject EnemyPrefab;
	public Camera cam;

	private void Start()
	{
		cam = CursorManager.GetCamera();
	}

	private void Spawn()
	{
		Services.enemyManager.SpawnNewEnemy(EnemyPrefab, transform.position);
		Destroy(gameObject);
	}

	private void Update()
	{
		if (IsOnScreen()) Spawn();
	}

	private bool IsOnScreen()
	{
		Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
		return viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z >= 0;
	}
}
