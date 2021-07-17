using System;
using UnityEngine;

public interface IAttackHandler
{
	bool CanAttack(Vector3 getPosition);

}

public interface IPlayerAttackHandler : IAttackHandler
{
	Player GetPlayer();
	event Action OnKillEnemy;
	event Action<AmmoHandler.AmmoType, int> OnUseAmmo;
	bool isBusy();
}
