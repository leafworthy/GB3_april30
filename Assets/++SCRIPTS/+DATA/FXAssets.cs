using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/DebreeAssets")]
public class FXAssets : ScriptableObject
{
    public enum DebreeType
    {
        blood,
        glass,
        wood,
        cushion
    }
    public List<GameObject> blood_debree = new List<GameObject>();

    public List<GameObject> glass_debree = new List<GameObject>();
    public List<GameObject> wood_debree = new List<GameObject>();
    public List<GameObject> cushion_debree = new List<GameObject>();

    public List<GameObject> glass_bulletholes_front = new List<GameObject>();
    public List<GameObject> wood_bulletholes_front = new List<GameObject>();
    public List<GameObject> wood_bulletholes_side = new List<GameObject>();

    public GameObject AKbulletPrefab;
    public GameObject GlockBulletPrefab;
    public GameObject bulletHitAnimPrefab;
    public GameObject muzzleFlashPrefab;
    public GameObject bulletShellPrefab;
    public GameObject explosionPrefab;
    public GameObject nadePrefab;

    public GameObject pickupEffectPrefab;
    public GameObject healthPickupPrefab;
    public GameObject speedPickupPrefab;
    public GameObject ammoPickupPrefab;
    public GameObject damagePickupPrefab;
    public GameObject nadesPickupPrefab;
    public GameObject cashPickupPrefab;
    public GameObject nadeTargetPrefab;
    public GameObject trajectoryMarkerPrefab;

    public Texture2D AKbulletTexture2D;
    public Texture2D cursorTexture2D;
    public Texture2D GlockBulletTexture2D;

    public List<GameObject> GetBulletHits(FXAssets.DebreeType debreeType)
    {
        switch (debreeType)
        {
            case DebreeType.blood:
                return blood_debree;
            case DebreeType.glass:
                return glass_bulletholes_front;
            case DebreeType.wood:
                return wood_bulletholes_front;
            case DebreeType.cushion:
                return wood_bulletholes_front;
        }

        return null;
    }
    public List<GameObject> GetDebree(DebreeType debreeType)
    {
        switch (debreeType)
        {
            case DebreeType.blood:
                return blood_debree;
            case DebreeType.glass:
                return glass_debree;
            case DebreeType.wood:
                return wood_debree;
            case DebreeType.cushion:
                return cushion_debree;
        }

        return null;
    }
}
