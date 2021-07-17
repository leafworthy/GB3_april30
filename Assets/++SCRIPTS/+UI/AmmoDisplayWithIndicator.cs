using UnityEngine;

public class AmmoDisplayWithIndicator : AmmoDisplay
{
	public GameObject IndicatorShine;

	protected override void UpdateDisplay(bool shake = false)
	{
		bar.UpdateBar(ammoToDisplay.reserveAmmo, ammoToDisplay.maxReserveAmmo);
		if (ammoToDisplay.reserveAmmo == ammoToDisplay.maxReserveAmmo)
		{
			IndicatorShine.SetActive(true);
		}
		else
		{
			IndicatorShine.SetActive(false);
		}
		if (shake)
		{
			ShakeObject();
		}
	}
}
