using System;
using UnityEngine;

public interface IAttackHandler
{
	bool CanAttack(Vector3 getPosition);
	event Action OnKillEnemy;
	event Action<AmmoHandler.AmmoType, int> OnUseAmmo;
}
