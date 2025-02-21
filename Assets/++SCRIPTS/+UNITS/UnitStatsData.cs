using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/UnitStats")]
public class UnitStatsData : ScriptableObject
{
	public DebrisType debrisType;
	
	public bool isObstacle;
	public bool isPlayerAttackable = true;

	public float AttackHeight;
	
	public float AttackDamage;
	public float AttackRate;
	public float Attack2Damage;
	public float Attack2Rate;
	public float AttackRange;
	
	public float HealthMax;
	public float MoveSpeed;
	public float DashSpeed;
	public float JumpSpeed;

	public float AggroRange;

	public bool ShowLifeBar = false;
}