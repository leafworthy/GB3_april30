using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : PlayerInteractable
{

    public bool isOn;
    public GameObject LightObject;
    private bool originalState;
    private Life life;
    private bool isBroken;
    public GameObject transformToDestroy;

    protected void Start()
    {
        life = GetComponentInChildren<Life>();
        life.OnDead += Life_Dead;
        originalState = isOn;
        OnActionPress += Interactable_OnInteract;
        SetLightActive(originalState);
    }

    private void Life_Dead(Player player)
    {
        SetLightActive(false);
        isBroken = true;
        Destroy(transformToDestroy);
    }

 

    private void Interactable_OnInteract(Player player)
    {
        if (isBroken) return;
        SetLightActive(!isOn);
    }


    private void SetLightActive(bool isActive)
    {
        isOn = isActive;
        LightObject.SetActive(isActive);
    }


}
