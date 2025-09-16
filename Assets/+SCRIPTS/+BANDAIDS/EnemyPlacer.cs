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


}
