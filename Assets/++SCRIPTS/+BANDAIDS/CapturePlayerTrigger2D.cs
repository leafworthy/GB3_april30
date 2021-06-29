using System;
using System.Collections.Generic;
using UnityEngine;

public class CapturePlayerTrigger2D : MonoBehaviour {

    public event Action<Collider2D> OnCapturedTriggerEnter2D;
    public event Action<Collider2D> OnCapturedTriggerExit2D;
    public event Action<IPlayerController> OnPlayerTriggerEnter2D;
    public event Action<IPlayerController> OnPlayerTriggerExit2D;

    public List<IPlayerController> playersInside = new List<IPlayerController>();

    private void OnTriggerEnter2D(Collider2D collider) {
        OnCapturedTriggerEnter2D?.Invoke(collider);

        IPlayerController playerRemoteMain = collider.GetComponent<IPlayerController>();
        if (playerRemoteMain != null) {
            OnPlayerTriggerEnter2D?.Invoke(playerRemoteMain);
            if (!playersInside.Contains(playerRemoteMain))
            {
                playersInside.Add(playerRemoteMain);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        OnCapturedTriggerExit2D?.Invoke(collider);
        IPlayerController playerRemoteMain = collider.GetComponent<IPlayerController>();
        if (playerRemoteMain != null)
        {
            if (playersInside.Contains(playerRemoteMain))
            {
                playersInside.Remove(playerRemoteMain);
            }
            OnPlayerTriggerExit2D?.Invoke(playerRemoteMain);
        }
    }

    public bool ContainsPlayer()
    {
        return playersInside.Count > 0;
    }
}
