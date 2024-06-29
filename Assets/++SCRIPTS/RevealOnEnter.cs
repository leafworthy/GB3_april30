using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealOnEnter : PlayerInteractable
{
    // Start is called before the first frame update
    private HideRevealObjects hideRevealObjects;
    protected void Start()
    {
	    hideRevealObjects = GetComponent<HideRevealObjects>();
	    OnPlayerEnters += Reveal;
	    OnPlayerExits += Hide;
    }

    private void Hide(Player obj)
    {
	    hideRevealObjects.Set(1);
    }

    private void Reveal(Player obj)
    {
	    hideRevealObjects.Set(0);
    }

}
