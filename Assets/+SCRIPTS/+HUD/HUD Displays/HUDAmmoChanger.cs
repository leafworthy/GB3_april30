using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace __SCRIPTS.HUD_Displays
{
	public class HUDAmmoChanger : MonoBehaviour, INeedPlayer
	{
		public AmmoDisplay ammoDisplay;
		public Image AKIcon;
		public Image PistolIcon;
		private Player player;
		private GunAttack gunAttackAkGlock;
		private bool isGlocking;

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			if (player.CurrentCharacter != Character.Bean) return;

			// Cache the component reference once during setup
			if (newPlayer.SpawnedPlayerGO != null)
			{
				gunAttackAkGlock = newPlayer.SpawnedPlayerGO.GetComponent<GunAttack>();

			}
		}

		private void Update()
		{
			if(player == null) return;
			if (player.CurrentCharacter != Character.Bean) return;
			if(player.SpawnedPlayerGO == null) return;

			// Use cached reference instead of GetComponent every frame
			if (gunAttackAkGlock == null) return;

			if(isGlocking == gunAttackAkGlock.IsUsingPrimaryGun) return;
			isGlocking = gunAttackAkGlock.IsUsingPrimaryGun;
			ChangeAmmo(isGlocking);
		}

		private void ChangeAmmo(bool isGlock)
		{
			if (player.CurrentCharacter != Character.Bean) return;
			if (!isGlock)
			{
				AKIcon.gameObject.SetActive(false);
				PistolIcon.gameObject.SetActive(true);
				ammoDisplay.SetAmmo( player.SpawnedPlayerGO.GetComponent<AmmoInventory>().unlimitedAmmo);
			}
			else
			{
				AKIcon.gameObject.SetActive(true);
				PistolIcon.gameObject.SetActive(false);
				ammoDisplay.SetAmmo(player.SpawnedPlayerGO.GetComponent<AmmoInventory>().primaryAmmo);
			}
		}
	}
}
