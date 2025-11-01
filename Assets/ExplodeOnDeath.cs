using System;
using System.Collections.Generic;
using __SCRIPTS;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IExplodeOnDeath
{
	void ExplodeDebreeEverywhere(float explosionSize, int min = 5, int max = 10);

}
public class ExplodeOnDeath : MonoBehaviour
{
	public int amount = 5;
    private IGetAttacked life => _life ??= GetComponent<IGetAttacked>();
	private IGetAttacked _life;
	private IExplodeOnDeath lifeFX => _lifeFX ??= GetComponent<IExplodeOnDeath>();
	private IExplodeOnDeath _lifeFX;

	public float explosionSize = 3f;

	public GameObject transformToDestroy;
	public event Action OnExplode;
	public List<LootType> lootTypes = new List<LootType>();
	private LootTable lootTable => _lootTable ?? ServiceLocator.Get<LootTable>();
	private LootTable _lootTable;
	private void OnEnable()
	{
		life.OnDead += LifeOnDead;
	}

	private void OnDisable()
	{
		life.OnDead -= LifeOnDead;
	}

	private void LifeOnDead(Attack obj)
	{
		Debug.Log("boom");
		lifeFX.ExplodeDebreeEverywhere(explosionSize);
		 life.OnDead -= LifeOnDead;
		 AttackUtilities.ExplosionFX(transformToDestroy.transform.position, explosionSize/5);
		 OnExplode?.Invoke();
		 DropLoot();
		 Services.objectMaker.Unmake(transformToDestroy);
	}

	private void DropLoot()
	{
		for (int i = 0; i < amount; i++)
		{
			var lootType = lootTypes[Random.Range(0, lootTypes.Count)];
			lootTable.DropLoot(transform.position, lootType);
		}
	}
}
