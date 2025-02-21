using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private AimAbility aim;
  

    void Update()
    {
        if (GlobalManager.IsPaused)
        {
            return;
        }
        transform.position = aim.GetAimPoint();
       
    }

    public void Init(Player player)
    {
        aim = player.SpawnedPlayerGO.GetComponent<AimAbility>();
        if (aim == null) return;
        transform.position = aim.GetAimPoint();
    }
}