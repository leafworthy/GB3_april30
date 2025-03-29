using TMPro;
using UnityEngine;

namespace __SCRIPTS.HUD_Displays
{
	public class AmmoDisplay : MonoBehaviour
	{
		private Bar_FX barFX;
		public TMP_Text ammoText;
		public TMP_Text totalText;
		protected Ammo ammoToDisplay;
		public GameObject shakeObject;
		public CanvasGroup ammoDisplayCanvas;
		public bool hasSlash;

		public bool greys;
		private bool init;


		private void Start()
		{
			LevelManager.OnStopLevel += CleanUp;
			barFX = GetComponentInChildren<Bar_FX>();
		}

	
		protected virtual void UpdateDisplay(bool shake = false)
		{
			if (ammoText != null)
			{
				ammoText.text = ammoToDisplay.reloads ? ammoToDisplay.AmmoInClip.ToString() : ammoToDisplay.reserveAmmo.ToString();
			}

			if (totalText != null)
			{
				if(hasSlash) totalText.text = "/"+ammoToDisplay.maxReserveAmmo.ToString();
				else totalText.text = ammoToDisplay.reserveAmmo.ToString();
			}

			if (!ammoToDisplay.hasAmmoInClip() && ammoToDisplay.reloads)
			{
				GreyOut();
			}
			else if(!ammoToDisplay.hasReserveAmmo() && !ammoToDisplay.reloads)
			{
				GreyOut();
			}
			else
			{
				Ungrey();
			}

			if (barFX == null) return;
			barFX.UpdateBar(ammoToDisplay.reserveAmmo, ammoToDisplay.maxReserveAmmo);
			if (shake)
			{
				ShakeObject();
			}
		}

		private void GreyOut()
		{
			if(greys) ammoDisplayCanvas.alpha = .25f;
		}

		private void Ungrey()
		{
			if (greys) ammoDisplayCanvas.alpha = 1;
		}

		protected void ShakeObject()
		{
			var shaker = shakeObject.GetComponent<ObjectShaker>();
			if (shaker == null) shaker = shakeObject.AddComponent<ObjectShaker>();
			shaker.Shake(ObjectShaker.ShakeIntensityType.low);
		}

		public void SetAmmo(Ammo newAmmo)
		{
			if (ammoToDisplay != null)
			{
				ammoToDisplay.OnAmmoGained -= AmmoUsedUpdateDisplay;
				ammoToDisplay.OnAmmoGained -= AmmoGainedUpdateDisplay;
			}
			ammoToDisplay = newAmmo;
			ammoToDisplay.OnAmmoUsed += AmmoUsedUpdateDisplay;
			ammoToDisplay.OnAmmoGained += AmmoGainedUpdateDisplay;
	
			init = true;
			UpdateDisplay(false);
		}



		private void CleanUp(GameLevel gameLevel)
		{
			if (!init) return;
			init = false;
			ammoToDisplay.OnAmmoGained -= AmmoUsedUpdateDisplay;
			ammoToDisplay.OnAmmoGained -= AmmoGainedUpdateDisplay;
		}

		private void AmmoUsedUpdateDisplay()
		{
			UpdateDisplay(true);
		}
		private void AmmoGainedUpdateDisplay()
		{
			UpdateDisplay(false);
		}

	}
}