using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/UnitStats")]
public class UnitStatsData : ScriptableObject
{
	public DebreeType DebreeType;
	
	public bool isObstacle;

	public float AttackHeight;
	
	public float AttackDamage;
	public float AttackRange;
	public float AttackRate;
	
	public float HealthMax;
	public float MoveSpeed;
	public float DashSpeed;
	public float JumpSpeed;

	public float AggroRange;
}