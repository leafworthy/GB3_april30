using _SCRIPTS;
using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
	public AmmoBar bar;
	public TMP_Text ammoText;
	protected Ammo ammoToDisplay;
	public GameObject shakeObject;


	protected virtual void UpdateDisplay(bool shake = false)
	{
		if (ammoText != null)
		{
			ammoText.text = ammoToDisplay.AmmoInClip.ToString();
		}

		bar.UpdateBar(ammoToDisplay.reserveAmmo + ammoToDisplay.AmmoInClip,
			ammoToDisplay.maxReserveAmmo + ammoToDisplay.clipSize);
		if (shake)
		{
			ShakeObject();
		}
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
		UpdateDisplay(false);
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
