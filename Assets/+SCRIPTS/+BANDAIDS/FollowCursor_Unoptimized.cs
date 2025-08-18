using UnityEngine;

namespace __SCRIPTS
{
    // UNOPTIMIZED VERSION - for baseline testing
    public class FollowCursor_Unoptimized : MonoBehaviour
    {
        public void Init(Player player)
        {
            if (player?.SpawnedPlayerGO != null)
            {
                // GetComponent on initialization - this is fine
                var aimAbility = player.SpawnedPlayerGO.GetComponentInChildren<AimAbility>();
                if (aimAbility != null)
                {
                    transform.position = aimAbility.GetAimPoint();
                }
            }
        }

        private void Update()
        {
            // PERFORMANCE KILLER: GetComponent calls every frame!
            var players = FindObjectsOfType<Player>();
            foreach (var player in players)
            {
                if (player?.SpawnedPlayerGO != null)
                {
                    var aimAbility = player.SpawnedPlayerGO.GetComponentInChildren<AimAbility>();
                    if (aimAbility != null)
                    {
                        transform.position = aimAbility.GetAimPoint();
                        break;
                    }
                }
            }
        }
    }
}