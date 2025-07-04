﻿using GangstaBean.Core;
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
		private GunAttack_AK_Glock gunAttackAkGlock;
		private bool isGlocking;
	
		public void SetPlayer(Player _player)
		{
			player = _player;
			if (player.CurrentCharacter != Character.Bean) return;
			
			// Cache the component reference once during setup
			if (_player.SpawnedPlayerGO != null)
			{
				gunAttackAkGlock = _player.SpawnedPlayerGO.GetComponent<GunAttack_AK_Glock>();
				if (gunAttackAkGlock == null)
				{
					Debug.LogWarning($"GunAttack_AK_Glock not found on {_player.SpawnedPlayerGO.name}", this);
				}
			}
		}

		private void Update()
		{
			if(player == null) return;
			if (player.CurrentCharacter != Character.Bean) return;
			if(player.SpawnedPlayerGO == null) return;
			
			// Use cached reference instead of GetComponent every frame
			if (gunAttackAkGlock == null) return;
			
			if(isGlocking == gunAttackAkGlock.isGlocking) return;
			isGlocking = gunAttackAkGlock.isGlocking;
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