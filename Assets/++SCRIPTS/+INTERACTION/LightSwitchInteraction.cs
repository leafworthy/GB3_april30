using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;
using UnityEngine;

namespace __SCRIPTS._INTERACTION
{
    public class LightSwitchInteraction : PlayerInteractable
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
}
