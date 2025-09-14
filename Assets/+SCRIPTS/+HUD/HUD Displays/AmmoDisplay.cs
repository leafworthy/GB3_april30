using GangstaBean.Core;
using TMPro;
using UnityEngine;

namespace __SCRIPTS.HUD_Displays
{
	public class AmmoDisplay : MonoBehaviour, INeedPlayer
	{
		private Bar_FX barFX;
		public TMP_Text ammoText;
		public TMP_Text totalText;
		private Ammo ammoToDisplay;
		public CanvasGroup ammoDisplayCanvas;

		public bool greys;
		private bool init;

		protected virtual void UpdateDisplay(bool shake = false)
		{
			if (ammoText != null) ammoText.text = ammoToDisplay.reloads ? ammoToDisplay.AmmoInClip.ToString() : ammoToDisplay.reserveAmmo.ToString();

			if (totalText != null)
			{
				if (ammoToDisplay.hasSlash) totalText.text = "/" + ammoToDisplay.reserveAmmo;
				else totalText.text = "/" + ammoToDisplay.maxReserveAmmo;
			}

			if (!ammoToDisplay.hasAmmoInClip() && ammoToDisplay.reloads)
				GreyOut();
			else if (!ammoToDisplay.hasReserveAmmo() && !ammoToDisplay.reloads)
				GreyOut();
			else
				Ungrey();

			if (barFX == null) return;
			barFX.UpdateBar(ammoToDisplay.reserveAmmo, ammoToDisplay.maxReserveAmmo);
		}

		private void GreyOut()
		{
			if (greys) ammoDisplayCanvas.alpha = .25f;
		}

		private void Ungrey()
		{
			if (greys) ammoDisplayCanvas.alpha = 1;
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

		private void CleanUp(GameLevel gameLevel)
		{
			if (!init) return;
			init = false;
			ammoToDisplay.OnAmmoUsed -= AmmoUsedUpdateDisplay;
			ammoToDisplay.OnAmmoGained -= AmmoGainedUpdateDisplay;
		}

		private void AmmoUsedUpdateDisplay()
		{
			UpdateDisplay(true);
		}

		private void AmmoGainedUpdateDisplay()
		{
			UpdateDisplay();
		}

		public void SetPlayer(Player _player)
		{
			Services.levelManager.OnStopLevel += CleanUp;
			barFX = GetComponentInChildren<Bar_FX>();
			if (barFX.fastBarImage != null) barFX.fastBarImage.color = _player.playerColor;
		}
	}
}
