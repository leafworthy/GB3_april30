using System.Collections;
using System.Collections.Generic;
using _SCRIPTS;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : Singleton<AmmoDisplay>
{
	static int bulletsInClip;
    static int clipSize;
    static int totalAmmo;

    static Text ammoText;


    // Update is called once per frame
    public static void UpdateDisplay(BeanAttackHandler bean)
    {
	    ammoText = I.GetComponent<Text>();
	    ammoText.text = "Ammo: " + bean.totalAmmo + "\n" +
	                    "Clip: " + bean.bulletsInClip + " / " + bean.clipSize + "\n" +
	                    "Nades: " + bean.nades;
    }
}
