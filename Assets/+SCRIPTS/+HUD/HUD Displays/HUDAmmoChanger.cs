using GangstaBean.Core;
using UnityEngine;
using UnityEngine.UI;

namespace __SCRIPTS.HUD_Displays
{
	public class HUDAmmoChanger : MonoBehaviour, INeedPlayer
	{
		public AmmoDisplay ammoDisplay;
		public Image AKIcon;
		public Image PistolIcon;
		Player player;
		GunAttack gunAttackAkGlock;
		bool isGlocking;

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			if (player.CurrentCharacter != Character.Bean) return;
			if (newPlayer.SpawnedPlayerGO == null) return;
			gunAttackAkGlock = newPlayer.SpawnedPlayerGO.GetComponent<GunAttack>();
			gunAttackAkGlock.OnSwitchGun += ChangeAmmo;
			ChangeAmmo(false);
		}


		void ChangeAmmo(bool isPrimary)
		{
			if (player.CurrentCharacter != Character.Bean) return;
			if (!gunAttackAkGlock.IsUsingPrimaryGun)
			{
				AKIcon.gameObject.SetActive(false);
				PistolIcon.gameObject.SetActive(true);
				ammoDisplay.SetAmmo(player.SpawnedPlayerGO.GetComponent<AmmoInventory>().unlimitedAmmo);
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
