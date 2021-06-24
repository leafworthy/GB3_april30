using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
namespace _SCRIPTS
{
    public class UnitStats : MonoBehaviour
    {
        public bool isPlayer;
        public UnitStatsData unitData;
        private List<UnitStat> unitStats = new List<UnitStat>();

        private void Awake()
        {
            foreach (var stat in unitData.baseStats)
            {
                unitStats.Add(new UnitStat(stat));
            }
            isPlayer = unitData.isPlayer;
        }

        public float GetStat(StatType type)
        {
            var stat = unitStats.FirstOrDefault(t => t.type == type);
            if (stat is null) return 0;
            return stat.GetValue();
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
            stat.SetValue(value);
        }

        public void ChangeStat(StatType type, float value)
        {
            var stat = unitStats.FirstOrDefault(t => t.type == type);
            if (stat is null) return;
            stat.ChangeValue(value);
        }

        public void ResetStat(StatType type)
        {
            var stat = unitStats.FirstOrDefault(t => t.type == type);
            if(stat is null) return;
            stat.ResetValue();
        }
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
    healthMax,
    aimHeight,
    aggroRange,
    cash
}
