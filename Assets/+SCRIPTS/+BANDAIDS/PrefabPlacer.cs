using __SCRIPTS.Cursor;
using Sirenix.OdinInspector;
using UnityEngine;

public class PrefabPlacer : MonoBehaviour
{
	public bool SpawnsNPCs;
	public GameObject Prefab;
	private Camera cam;
	public bool isBehindSpawner;
	public float margin = -0.2f; // Margin for spawn trigger (0.1 = 10% of screen width)
	public int EnemyTier;
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
		if (paletteSwapper == null) return;
		paletteSwapper.SetPallette(EnemyTier);
	}

	private void Spawn()
	{
		if (SpawnsNPCs) Services.enemyManager.SpawnNewNPC(Prefab, transform.position);
		else Services.enemyManager.SpawnNewEnemy(Prefab, transform.position, EnemyTier);
		Destroy(gameObject);
	}

	private void Update()
	{
		if (ShouldSpawn()) Spawn();
	}

	private bool ShouldSpawn()
	{
		var viewportPos = cam.WorldToViewportPoint(transform.position);
		var inVerticalBounds = viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z >= 0;

		if (!inVerticalBounds) return false;

		if (isBehindSpawner) return viewportPos.x < -margin;

		return viewportPos.x >= -margin && viewportPos.x <= 1 + margin;
	}
}
