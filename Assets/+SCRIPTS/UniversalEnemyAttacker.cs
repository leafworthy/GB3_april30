using System;
using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public class UniversalEnemyAttacker : MonoBehaviour, ICanAttack
{
	public Player player => _player ??= Services.playerManager.NPCPlayer;
	private Player _player;
	public LayerMask EnemyLayer => Services.assetManager.LevelAssets.AllLivingBeingsLayer;

	public IHaveUnitStats stats => _stats ??= GetComponentInChildren<IHaveUnitStats>();
	IHaveUnitStats _stats;

	public bool IsEnemyOf(IGetAttacked targetLife) => true;

	public event Action<IGetAttacked> OnAttack;
}
