using UnityEngine;
using VInspector;

public class EnemyPlacer : ServiceUser
{
	public GameObject EnemyPrefab;

	private void Start()
	{
		enemyManager.SpawnNewEnemy(EnemyPrefab, transform.position);
		Destroy(gameObject);
	}

	[Button]
	public void GrabSpriteFromPrefab()
	{
		if (EnemyPrefab == null) return;
		var spriteRenderer = EnemyPrefab.GetComponentInChildren<SpriteRenderer>();
		if (spriteRenderer == null) return;
		var mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
		if (mySpriteRenderer == null) return;
		mySpriteRenderer.sprite = spriteRenderer.sprite;
	}

}
