using System.Collections.Generic;
using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

[RequireComponent(typeof(UniversalEnemyAttacker))]
public class ExplodeOnDeath : MonoBehaviour
{
    private Life life => _life ??= GetComponentInChildren<Life>();
	private Life _life;
	ICanAttack attack  => _attack ??= GetComponent<ICanAttack>();
	private ICanAttack _attack;
	public float explosiveDamage = 5f;

	public float explosionSize = 3f;

	public GameObject transformToDestroy;
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
		 life.OnDead -= LifeOnDead;
		 MyAttackUtilities.ExplodeDebreeEverywhere(explosionSize  , transformToDestroy.transform.position  , life.DebrisType, life.Stats.DebrisTint);
		 MyAttackUtilities.Explode(transformToDestroy.transform.position, explosionSize, explosiveDamage, attack);
		 MyAttackUtilities.ExplosionFX(transformToDestroy.transform.position, explosionSize/5);
		 Services.objectMaker.Unmake(transformToDestroy);
	}


}
