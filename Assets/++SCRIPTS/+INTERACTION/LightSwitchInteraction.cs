using UnityEngine;

namespace __SCRIPTS
{
    public class LightSwitchInteraction : PlayerInteractable
    {

        public bool isOn;
        public GameObject LightObject;
        private bool originalState;
        private Life life;
        private bool isBroken;
        public GameObject transformToDestroy;

        protected void OnEnable()
        {
           // life = GetComponentInChildren<Life>();
            //life.OnDead += Life_Dead;
            originalState = isOn;
            OnActionPress += Interactable_OnInteract;
            SetLightActive(originalState);
        }

        private void OnDisable()
        {
                //ife.OnDead -= Life_Dead;
                 OnActionPress -= Interactable_OnInteract;
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
            SFX.sounds.light_switch_sound.PlayRandomAt(transform.position);
        }


        private void SetLightActive(bool isActive)
        {
            isOn = isActive;
            LightObject.SetActive(isActive);
        }


    }
}