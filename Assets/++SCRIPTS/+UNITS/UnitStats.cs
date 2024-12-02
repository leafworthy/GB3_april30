using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [HideInInspector] public Player player;
    public UnitStatsData unitData;
    

    protected virtual void OnValidate()
    {
        if (unitData != null) return;
        unitData = ASSETS.LevelAssets.DefaultUnitData;
    }


    public float AttackHeight => unitData.AttackHeight;
    public float AttackDamage => unitData.AttackDamage;
    public float AttackRange => unitData.AttackRange;
    public float AttackRate => unitData.AttackRate;

    public float Attack2Damage => unitData.AttackDamage;
    public float Attack2Rate => unitData.AttackRate;

    
    public float HealthMax => unitData.HealthMax;
    public float MoveSpeed => unitData.MoveSpeed;
    public float DashSpeed => unitData.DashSpeed;
    
    public bool IsPlayer => IsThisAPlayer();

    public bool isEnemyOf(Life otherLife) => IsPlayer != otherLife.IsPlayer;

    private bool IsThisAPlayer() => player != null && player.IsPlayer();

    public bool IsObstacle => unitData.isObstacle;

    public DebrisType DebrisType => unitData.debrisType;
    public float JumpSpeed => unitData.JumpSpeed;
    public float AggroRange => unitData.AggroRange;

    public void SetPlayer(Player _player)
    {
        player = _player;
    }
}