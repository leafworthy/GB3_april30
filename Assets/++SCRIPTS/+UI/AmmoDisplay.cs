using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AmmoDisplay : MonoBehaviour
{
	[FormerlySerializedAs("healthBar"),FormerlySerializedAs("bar")] public AmmoLifeBar lifeBar;
	public TMP_Text ammoText;
	public TMP_Text totalText;
	protected Ammo ammoToDisplay;
	public GameObject shakeObject;
	public CanvasGroup ammoDisplayCanvas;
	private bool init;


	protected virtual void UpdateDisplay(bool shake = false)
	{
		if (ammoText != null)
		{
			ammoText.text = ammoToDisplay.reloads ? ammoToDisplay.AmmoInClip.ToString() : ammoToDisplay.reserveAmmo.ToString();
		}

		if (totalText != null)
		{
			totalText.text = ammoToDisplay.reserveAmmo.ToString();
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

		if (lifeBar == null) return;
		lifeBar.UpdateBar(ammoToDisplay.reserveAmmo, ammoToDisplay.maxReserveAmmo);
		if (shake)
		{
			ShakeObject();
		}
	}

	private void GreyOut()
	{
		ammoDisplayCanvas.alpha = .25f;
	}

	private void Ungrey()
	{
		ammoDisplayCanvas.alpha = 1;
	}

	protected void ShakeObject()
	{
		var shaker = shakeObject.gameObject.AddComponent<ObjectShaker>();
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
		Level.OnStop += CleanUp;
		init = true;
		UpdateDisplay(false);
	}



	private void CleanUp(Scene.Type type)
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
