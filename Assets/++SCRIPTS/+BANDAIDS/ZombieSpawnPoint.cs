using UnityEngine;

public class ZombieSpawnPoint : MonoBehaviour
{
	public GameObject ZombiePrefab;
	private void Spawn()
	{
		var newZombie = Maker.Make(ZombiePrefab, transform.position);
		var newLife = newZombie.GetComponent<Life>();
		newLife.SetPlayer(Players.EnemyPlayer);
		var newAI = newZombie.GetComponent<EnemyAI>();
		newAI.SetWanderPoint(transform.position);
		EnemyManager.CollectEnemy(newZombie);
	}

	private void Start()
	{
		Spawn();
	}
}