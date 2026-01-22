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
		ThreeWeaponSwitchAbility gunAttackAkGlock;
		bool isGlocking;

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			if (player.CurrentCharacter != Character.Bean) return;
			if (newPlayer.SpawnedPlayerGO == null) return;
			gunAttackAkGlock = newPlayer.SpawnedPlayerGO.GetComponent<ThreeWeaponSwitchAbility>();
			gunAttackAkGlock.OnSwitchWeapon += ChangeAmmo;
		}

		void ChangeAmmo(WeaponAbility weaponAbility)
		{
			if (player.CurrentCharacter != Character.Bean) return;

			if (weaponAbility is not GunAttackSingle gunAbility) return;
			if (gunAbility.CurrentGun is PrimaryGun)
			{
				AKIcon.gameObject.SetActive(true);
				PistolIcon.gameObject.SetActive(false);
				ammoDisplay.SetAmmo(player.SpawnedPlayerGO.GetComponent<AmmoInventory>().primaryAmmo);

			}
			else
			{
				AKIcon.gameObject.SetActive(false);
				PistolIcon.gameObject.SetActive(true);
				ammoDisplay.SetAmmo(player.SpawnedPlayerGO.GetComponent<AmmoInventory>().unlimitedAmmo);
			}




		}

	}
}
