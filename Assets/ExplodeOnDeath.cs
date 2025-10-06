using System;
using __SCRIPTS;
using UnityEngine;

public class ExplodeOnDeath : MonoBehaviour
{
    private Life life => _life ??= GetComponent<Life>();
	private Life _life;

	private Life_FX lifeFX => _lifeFX ??= GetComponent<Life_FX>();
	private Life_FX _lifeFX;

	public float explosionSize = 3f;

	public GameObject transformToDestroy;
	public event Action OnExplode;

	private void OnEnable()
	{
		life.OnDying += Life_OnDying;
	}

	private void OnDisable()
	{
		life.OnDying -= Life_OnDying;
	}

	private void Life_OnDying(Attack obj)
	{
		Debug.Log("boom");
		lifeFX.ExplodeDebreeEverywhere(explosionSize);
		 life.OnDying -= Life_OnDying;
		 AttackUtilities.ExplosionFX(transformToDestroy.transform.position, explosionSize/5);
		 OnExplode?.Invoke();
		 Services.objectMaker.Unmake(transformToDestroy);
	}
}
