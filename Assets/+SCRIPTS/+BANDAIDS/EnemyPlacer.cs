using Sirenix.OdinInspector;
using UnityEngine;


public class EnemyPlacer : MonoBehaviour
{
	public GameObject EnemyPrefab;

	private void Start()
	{
		Services.enemyManager.SpawnNewEnemy(EnemyPrefab, transform.position);
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
