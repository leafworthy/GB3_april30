using System;
using __SCRIPTS;
using UnityEngine;

public class Mine : MonoBehaviour
{
	private bool hasLaunched;
	public bool isProximityMine;
	private ICanAttack life;
	public Action<Mine> OnSelfDetonate;

	public void Launch(Vector2 _start, ICanAttack _mineLayerLife)
	{
		life = _mineLayerLife;
		transform.position = _start;
		hasLaunched = true;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!hasLaunched || !isProximityMine) return;
		if (other.transform == transform) return;
		var otherLife = other.GetComponent<IGetAttacked>();
		if (otherLife == null)
		{
			otherLife = other.GetComponentInParent<Life>();
			if (otherLife == null) return;
		}

		if (otherLife.IsEnemyOf(life))
		{
			OnSelfDetonate?.Invoke(this);
			Explode();
		}
	}

	private void Explode()
	{
		AttackUtilities.Explode(transform.position, life.Stats.SecondaryAttackRange,
			life.Stats.SecondaryAttackDamageWithExtra, life);
		Services.objectMaker.Unmake(gameObject);

	}

	public void Detonate()
	{
		Explode();
	}
}
