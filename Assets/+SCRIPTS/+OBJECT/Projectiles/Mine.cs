using System;
using __SCRIPTS;
using UnityEngine;

public class Mine : ServiceUser
{
	private bool hasLaunched;
	public bool isProximityMine;
	private Player player;
	public Action<Mine> OnSelfDetonate;

	public void Launch(Vector2 _start, Player _player)
	{
		player = _player;
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

		if (otherLife.IsEnemyOf(player.spawnedPlayerLife))
		{
			OnSelfDetonate?.Invoke(this);
			Explode();
		}
	}

	private void Explode()
	{
		Explosion_FX.Explode(transform.position, player.spawnedPlayerLife.SecondaryAttackRange,
			player.spawnedPlayerLife.SecondaryAttackDamageWithExtra, player);
		objectMaker.Unmake(gameObject);

	}

	public void Detonate()
	{
		Explode();
	}
}
