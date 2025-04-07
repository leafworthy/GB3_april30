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
		private GunAttack gunAttack;
		private bool isGlocking;
	
		public void SetPlayer(Player _player)
		{
			player = _player;
			if (player.CurrentCharacter != Character.Bean) return;
			gunAttack = _player.SpawnedPlayerGO.GetComponent<GunAttack>();
		}

		private void Update()
		{
			if(player == null) return;
			if (player.CurrentCharacter != Character.Bean) return;
			if(player.SpawnedPlayerGO == null) return;
			gunAttack = player.SpawnedPlayerGO.GetComponent<GunAttack>();
			if(isGlocking == gunAttack.isGlocking) return;
			isGlocking = gunAttack.isGlocking;
			ChangeAmmo(isGlocking);
		}

		private void ChangeAmmo(bool isGlock)
		{
			if (player.CurrentCharacter != Character.Bean) return;
			if (isGlock)
			{
				AKIcon.enabled = false;
				PistolIcon.enabled = true;
				ammoDisplay.SetAmmo( player.SpawnedPlayerGO.GetComponent<AmmoInventory>().unlimitedAmmo);
			}
			else
			{
				AKIcon.enabled = true;
				PistolIcon.enabled = false;
				ammoDisplay.SetAmmo(player.SpawnedPlayerGO.GetComponent<AmmoInventory>().primaryAmmo);
			}
		}
	}
}