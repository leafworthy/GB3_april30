using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
    public enum DebrisType
    {
        blood,
        glass,
        wood,
        metal,
        wall,
        none
    }
    [CreateAssetMenu(menuName = "My Assets/DebreeAssets")]
    public class FXAssets : ScriptableObject
    {

        [Header("Hit FX")]
        public List<GameObject> blood_debree = new List<GameObject>();
        public List<GameObject> bloodspray = new List<GameObject>();
        [Space]
        public List<GameObject> fires = new List<GameObject>();
        public GameObject fire1_small;
        public GameObject fire2_medium;

        [Space]
        public GameObject dust1_ground;
        public GameObject dust2_jump;

        [Space]
        public List<GameObject> hits = new List<GameObject>();
        public GameObject hit1_starhit;
        public GameObject hit2_biglongflash;
        public GameObject hit3_shield;
        public GameObject hit4_star_particles;
        public GameObject hit5_xstrike;
        public GameObject hit5_line_burst;

        [Space]
        public List<GameObject> explosions = new List<GameObject>();
        public GameObject explosion1_nade;
        public GameObject explosion2_oval;
        public GameObject explosion3_huge;

        [Space]
        public List<GameObject> glass_debree = new List<GameObject>();
        public List<GameObject> wood_debree = new List<GameObject>();
        public List<GameObject> cushion_debree = new List<GameObject>();
        public List<GameObject> wall_debree = new List<GameObject>();
        public List<GameObject> metal_debree = new List<GameObject>();

        public List<GameObject> bulletholes = new List<GameObject>();

        [Space,Header("Weapon FX")]
        public GameObject BulletPrefab;
        public GameObject AKbulletPrefab;
        public GameObject GlockBulletPrefab;
        public GameObject bulletHitAnimPrefab;
        public GameObject muzzleFlashPrefab;
        public GameObject bulletShellPrefab;
        public GameObject nadePrefab;
        public GameObject minePrefab;
        public GameObject kunaiPrefab;

        [Space,Header("Pickup FX")]
        public GameObject pickupEffectPrefab;
        public GameObject healthPickupPrefab;
        public GameObject speedPickupPrefab;
        public GameObject ammoPickupPrefab;
        public GameObject damagePickupPrefab;
        public GameObject nadesPickupPrefab;
        public GameObject cashPickupPrefab;
        public GameObject gasPickupPrefab;

        public GameObject trajectoryMarkerPrefab;
        public GameObject nadeTargetPrefab;

        public GameObject loadingBarPrefab;

        public GameObject indicatorPrefab;
        public GameObject directionArrowPrefab;
        [FormerlySerializedAs("RisingTextPrefab")] public GameObject risingTextPrefab;

        public List<GameObject> GetBulletHits(DebrisType debrisType)
        {

            switch (debrisType)
            {
                case DebrisType.blood:
                    return null;
                case DebrisType.glass:
                case DebrisType.wood:
                case DebrisType.metal:
                case DebrisType.wall:
                    return bulletholes;
            }

            return null;
        }
        public GameObject GetDebree(DebrisType debrisType)
        {
            switch (debrisType)
            {
                case DebrisType.blood:
                    return blood_debree.GetRandom();
                case DebrisType.glass:
                    return glass_debree.GetRandom();
                case DebrisType.wood:
                    return wood_debree.GetRandom();
                case DebrisType.metal:
                    return metal_debree.GetRandom();
                case DebrisType.wall:
                    return wall_debree.GetRandom();
            }

            return null;
        }


    }
}
