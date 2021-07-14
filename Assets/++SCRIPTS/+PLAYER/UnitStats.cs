using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public bool isPlayer;
    public Player player;
    public UnitStatsData unitData;
    [SerializeField]private List<UnitStat> unitStats = new List<UnitStat>();
    public Action<UnitStat> OnStatChange;

    private void Awake()
    {
        foreach (var stat in unitData.baseStats)
        {
            unitStats.Add(new UnitStat(stat));
        }
        isPlayer = unitData.isPlayer;
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
    }

    public float GetStatValue(StatType type)
    {
        var stat = unitStats.FirstOrDefault(t => t.type == type);
        if (stat is null) return 0;
        return stat.GetValue();
    }

    public UnitStat GetStat(StatType type)
    {
        var stat = unitStats.FirstOrDefault(t => t.type == type);
        return stat;
    }

    public float GetBaseStat(StatType type)
    {
        var stat = unitStats.FirstOrDefault(t => t.type == type);
        return stat.GetBaseValue();
    }

    public void SetStat(StatType type,float value)
    {
        var stat = unitStats.FirstOrDefault(t => t.type == type);
        if (stat is null) return;
       // Debug.Log("setting stat " + stat.type + " from "+stat.value+" to " + value);
        stat.SetValue(value);
        OnStatChange?.Invoke(stat);
    }

    public void ChangeStat(StatType type, float value)
    {
        var stat = unitStats.FirstOrDefault(t => t.type == type);
        if (stat is null) return;
//        Debug.Log("changing stat " + stat.type + " "+stat.value+" by " + value);
        stat.ChangeValue(value);
        OnStatChange?.Invoke(stat);
    }

    public void ResetStat(StatType type)
    {
        var stat = unitStats.FirstOrDefault(t => t.type == type);
        if(stat is null) return;
      // Debug.Log("resetting stat " + stat.type + " " + stat.value + " to " + stat.GetBaseValue());
        stat.ResetValue();
        OnStatChange?.Invoke(stat);
    }
}

public enum StatType
{
    attackDamage,
    attackRange,
    attackRate,
    moveSpeed,
    dashSpeed,
    dashMultiplier,
    activeRange,
    health,
    aimHeight,
    aggroRange,
    cash,
    teleportSpeed
}
