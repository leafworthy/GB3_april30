using System;
using Unity.Cinemachine;
using UnityEngine;

public class CarAnimations : MonoBehaviour
{
    private static readonly int Start = Animator.StringToHash("Start");

    public CarAccessInteraction carAccessInteraction;
    private bool carStarted;
    private Animator animator;
    private Player currentPlayer;
    public CinemachineCamera cam;

    public static event Action OnCarSequenceStart;

    public void PlayerEntersCar()
    {
        foreach (var player in Players.AllJoinedPlayers)
        {

            player.SpawnedPlayerGO.SetActive(false);
        }
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
        var brain = FindFirstObjectByType<CinemachineBrain>( );
        brain.DefaultBlend.Time = 2.0f; // Adjust blend time
        cam.Priority = 12;
        currentPlayer = player;
    }

    private bool pressed;

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
        //LevelGameScene.WinGame();
        Debug.Log("win");
    }
}
