using GangstaBean.Core;
using TMPro;
using UnityEngine;

namespace __SCRIPTS.HUD_Displays
{
	public class AmmoDisplay : MonoBehaviour, INeedPlayer
	{
		LineBar barFX => _barFX ??= GetComponentInChildren<LineBar>();
		LineBar _barFX;
		public TMP_Text ammoText;
		public TMP_Text totalText;
		Ammo ammoToDisplay;
		public CanvasGroup ammoDisplayCanvas;
		WeaponButton weaponButton => _weaponButton ??= GetComponentInChildren<WeaponButton>();
		WeaponButton _weaponButton;

		public bool greys;
		bool init;
		Player player;

		void UpdateDisplay()
		{
			if (ammoText != null) ammoText.text = ammoToDisplay.reloads ? ammoToDisplay.AmmoInClip.ToString() : ammoToDisplay.reserveAmmo.ToString();

			if (totalText != null)
			{
				if (ammoToDisplay.hasSlash) totalText.text = "/" + ammoToDisplay.reserveAmmo;
				else totalText.text = "/" + ammoToDisplay.maxReserveAmmo;
			}

			if (!ammoToDisplay.hasAmmoInClip() && ammoToDisplay.reloads || !ammoToDisplay.hasReserveAmmo() && !ammoToDisplay.reloads)
				GreyOut();
			else
				Ungrey();

			var fraction = ammoToDisplay.totalAmmo() / ammoToDisplay.maxReserveAmmo;
			if(player != null)
			{
				if(ammoToDisplay.whiteWhenFull) barFX.FastBar.color = fraction == 1 ? Color.white : player.playerColor;
			}
			barFX.gameObject.SetActive(true);
			barFX.UpdateBar(fraction);
		}

		void GreyOut()
		{
			if (greys) ammoDisplayCanvas.alpha = .25f;
		}

		void Ungrey()
		{
			if (greys) ammoDisplayCanvas.alpha = 1;
		}

		public void SetButton(WeaponButton.buttons button)
		{
			weaponButton.Set(button);
		}

		public void SetAmmo(Ammo newAmmo)
		{
			if (ammoToDisplay != null)
			{
				ammoToDisplay.OnAmmoUsed -= AmmoUsedUpdateDisplay;
				ammoToDisplay.OnAmmoGained -= AmmoGainedUpdateDisplay;
			}

			ammoToDisplay = newAmmo;
			ammoToDisplay.OnAmmoUsed += AmmoUsedUpdateDisplay;
			ammoToDisplay.OnAmmoGained += AmmoGainedUpdateDisplay;

			init = true;
			UpdateDisplay();
		}

		void CleanUp(GameLevel gameLevel)
		{
			if (!init) return;
			init = false;
			ammoToDisplay.OnAmmoUsed -= AmmoUsedUpdateDisplay;
			ammoToDisplay.OnAmmoGained -= AmmoGainedUpdateDisplay;
		}

		void AmmoUsedUpdateDisplay()
		{
			UpdateDisplay();
		}

		void AmmoGainedUpdateDisplay()
		{
			UpdateDisplay();
		}

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			Services.levelManager.OnStopLevel += CleanUp;
			if (barFX.FastBar != null) barFX.FastBar.color = newPlayer.playerColor;
			barFX.useGradientColor = false;
		}
	}
}
