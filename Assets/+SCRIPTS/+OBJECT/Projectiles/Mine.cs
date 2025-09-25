using System;
using __SCRIPTS;
using UnityEngine;

public class Mine : MonoBehaviour
{
	private bool hasLaunched;
	public bool isProximityMine;
	private Life life;
	public Action<Mine> OnSelfDetonate;

	public void Launch(Vector2 _start, Life _mineLayerLife)
	{
		life = _mineLayerLife;
		transform.position = _start;
		hasLaunched = true;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!hasLaunched || !isProximityMine) return;
		if (other.transform == transform) return;
		var otherLife = other.GetComponent<Life>();
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
		AttackUtilities.Explode(transform.position, life.SecondaryAttackRange,
			life.SecondaryAttackDamageWithExtra, life);
		Services.objectMaker.Unmake(gameObject);

	}

	public void Detonate()
	{
		Explode();
	}
}
