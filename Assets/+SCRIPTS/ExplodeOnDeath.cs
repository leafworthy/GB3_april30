using System;
using System.Collections.Generic;
using __SCRIPTS;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;


public class ExplodeOnDeath : MonoBehaviour
{
    private Life life => _life ??= GetComponent<Life>();
	private Life _life;

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
		 MyAttackUtilities.ExplosionFX(transformToDestroy.transform.position, explosionSize/5);
		 Services.objectMaker.Unmake(transformToDestroy);
	}


}
