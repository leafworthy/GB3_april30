using System;
using System.Collections.Generic;
using UnityEngine;

namespace _SCRIPTS
{
    public class CapturePlayerTrigger2D : MonoBehaviour {

        public event Action<Collider2D> OnCapturedTriggerEnter2D;
        public event Action<Collider2D> OnCapturedTriggerExit2D;
        public event Action<PlayerController> OnPlayerTriggerEnter2D;
        public event Action<PlayerController> OnPlayerTriggerExit2D;

        public List<PlayerController> playersInside = new List<PlayerController>();

        private void OnTriggerEnter2D(Collider2D collider) {
            OnCapturedTriggerEnter2D?.Invoke(collider);

            PlayerController playerMain = collider.GetComponent<PlayerController>();
            if (playerMain != null) {
                OnPlayerTriggerEnter2D?.Invoke(playerMain);
                if (!playersInside.Contains(playerMain))
                {
                    playersInside.Add(playerMain);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            OnCapturedTriggerExit2D?.Invoke(collider);
            PlayerController playerMain = collider.GetComponent<PlayerController>();
            if (playerMain != null)
            {
                if (playersInside.Contains(playerMain))
                {
                    playersInside.Remove(playerMain);
                }
                OnPlayerTriggerExit2D?.Invoke(playerMain);
            }
        }

        public bool ContainsPlayer()
        {
            return playersInside.Count > 0;
        }
    }
}
