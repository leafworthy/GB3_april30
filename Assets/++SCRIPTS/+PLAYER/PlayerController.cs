using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
        
    private Player owner;
    private PlayerControls controls;
    public NewInputAxis AimAxis;
    public NewInputAxis MoveAxis;
    public NewInputAxis UIAxis;
    public NewControlButton Select;
    public NewControlButton Cancel;
    public NewControlButton Jump;
    public NewControlButton Dash;
    public NewControlButton Attack1;
    public NewControlButton Attack2;
    public NewControlButton Attack3;
    public NewControlButton Reload1;
    public NewControlButton ActionButton;
    public NewControlButton Pause;
    public NewControlButton Unpause;

    public PlayerInput input;

    private bool initialized;

    private void Start()
    {
        SetAxes();
    }

      
    public void InitializeAndLinkToPlayer(Player player)
    {
        if (initialized) return;
        initialized = true;
        input = GetComponent<PlayerInput>();
        owner = player; 
        controls = new PlayerControls();
          
            
            
        SetAxes();

        Select = new NewInputButton(controls.UI.Select, owner);
        Cancel = new NewInputButton(controls.UI.Cancel, owner);
        Unpause = new NewInputButton(controls.UI.Unpause, owner);
        Select = new NewInputButton(controls.UI.Select, owner);
            
        Pause = new NewInputButton(controls.PlayerMovement.Pause, owner);
        Jump = new NewInputButton(controls.PlayerMovement.Jump, owner);
        Dash = new NewInputButton(controls.PlayerMovement.Dash, owner);
        Attack1 = new NewInputButton(controls.PlayerMovement.Attack1, owner);
        Attack2 = new NewInputButton(controls.PlayerMovement.Attack2, owner);
        Attack3 = new NewInputButton(controls.PlayerMovement.Attack3, owner);
        Reload1 = new NewInputButton(controls.PlayerMovement.Reload1, owner);
        ActionButton = new NewInputButton(controls.PlayerMovement.Reload2, owner);
          
    }

    private void FixedUpdate()
    {
        if (!initialized) return;
        MoveAxis?.update();
    }

    private void SetAxes()
    {
        AimAxis = owner.isUsingMouse ? new NewInputAxis(controls.PlayerMovement.MousePosition, owner) : new NewInputAxis(controls.PlayerMovement.StickAiming, owner);

        UIAxis = new NewInputAxis(controls.UI.Movement, owner);
        MoveAxis = new NewInputAxis(controls.PlayerMovement.Movement, owner);
    }
}