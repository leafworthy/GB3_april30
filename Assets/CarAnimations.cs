using System;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarAnimations : MonoBehaviour
{
    private static readonly int Start = Animator.StringToHash("Start");

    public CarAccessInteraction carAccessInteraction;
    private bool carStarted;
    private Animator animator;
    private Player currentPlayer;
    public CinemachineVirtualCamera cam;

    public static event Action OnCarSequenceStart;

    public void PlayerEntersCar()
    {
        currentPlayer.SpawnedPlayerGO.SetActive(false);
        Debug.Log("here we gooooo");
    }

    private void OnEnable()
    {
        carAccessInteraction.OnCarAccessActionPressed += OnCarAccessActionPressed;
        animator = GetComponent<Animator>();
    }

    private void OnCarAccessActionPressed(Player player)
    {
        if (carStarted) return;
        carStarted = true;
        animator.SetTrigger(Start);
        OnCarSequenceStart?.Invoke();
        var brain = FindObjectsByType<CinemachineBrain>( FindObjectsSortMode.None).FirstOrDefault();
        brain.m_DefaultBlend.m_Time = 2.0f; // Adjust blend time
        cam.Priority = 12;
        currentPlayer = player;
    }

    private bool pressed;
    private void Update()
    {
        if(pressed) return;
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            OnCarAccessActionPressed(null);
        }
    }

    public void OnCarEnter()
    {
     SFX.sounds.car_start_sound.PlayRandomAt(transform.position);
     Debug.Log("heyo");
    }

    public void OnCarStart()
    {
        
    }

    public void OnCarSkirtOff()
    {
        
    }
}
