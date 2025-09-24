using __SCRIPTS.Cursor;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyPlacer : MonoBehaviour
{
	public GameObject EnemyPrefab;
	public Camera cam;
	public bool isBehindSpawner; // true = spawns when spawner is beyond left side, false = spawns when spawner enters from right
	private float margin = 0.2f; // Margin for spawn trigger (0.1 = 10% of screen width)
	public int EnemyTier = 0;


	private SpriteRenderer spriteRenderer => _spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
	private SpriteRenderer _spriteRenderer;
	private void Start()
	{
		cam = CursorManager.GetCamera();
		spriteRenderer.enabled = false;
	}

	[Button]
	private void SetTier()
	{
		var paletteSwapper = GetComponent<PalletteSwapper>();
		paletteSwapper.SetPallette(EnemyTier);

	}
	private void Spawn()
	{
		Services.enemyManager.SpawnNewEnemy(EnemyPrefab, transform.position, EnemyTier);

		Destroy(gameObject);
	}

	private void Update()
	{
		if (ShouldSpawn()) Spawn();
	}

	private bool ShouldSpawn()
	{
		Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
		bool inVerticalBounds = viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z >= 0;

		if (!inVerticalBounds) return false;

		if (isBehindSpawner)
		{
			// Spawn when spawner is beyond the left side of the screen (with margin)
			return viewportPos.x < -margin;
		}
		else
		{
			// Spawn when spawner enters from the right side (with margin)
			return viewportPos.x >= -margin && viewportPos.x <= 1 + margin;
		}
	}
}
