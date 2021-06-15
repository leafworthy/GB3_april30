using UnityEngine;

namespace _SCRIPTS
{
    public class UnitStats : MonoBehaviour {

        public float attackDamage = 1f;
        public float attackRange = 150f;
        public float aggroRange = 50;
        public float healthMax = 100;
        public float attackRate = 2;
        public float moveSpeed = 10;
        public float height;
        public bool isPlayer;
        public float dashMultiplier = 1;
        public float dashSpeed = 15;
        public float hitPushMultiplier = 1;
        public float reloadTime = .3f;
        public float activeRange= 200;
    }
}
