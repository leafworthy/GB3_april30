using UnityEngine;

namespace __SCRIPTS
{
    /// <summary>
    /// Improved PlayerController using simplified UI navigation system
    /// Replaces the complex input axis system with reliable, straightforward navigation
    /// </summary>
    public class ImprovedPlayerController : MonoBehaviour
    {
        private Player owner;
        private PlayerControls controls;
        
        // Simplified components
        public SimpleUINavigation UINavigation { get; private set; }
        public NewInputAxis AimAxis;
        public NewInputAxis MoveAxis;
        
        // Button controls (unchanged)
        public NewControlButton Jump;
        public NewControlButton DashRightShoulder;
        public NewControlButton Attack1RightTrigger;
        public NewControlButton Attack2LeftTrigger;
        public NewControlButton Attack3Circle;
        public NewControlButton ReloadTriangle;
        public NewControlButton SwapWeaponSquare;
        public NewControlButton InteractRightShoulder;
        public NewControlButton Pause;
        
        private bool initialized;
        
        public void InitializeAndLinkToPlayer(Player player)
        {
            if (initialized) return;
            
            if (player == null)
            {
                Debug.LogError("ImprovedPlayerController.InitializeAndLinkToPlayer: player is null");
                return;
            }
            
            try
            {
                owner = player;
                controls = new PlayerControls();
                
                // Initialize simplified UI navigation
                UINavigation = gameObject.AddComponent<SimpleUINavigation>();
                UINavigation.Initialize(owner, controls);
                
                // Initialize aim and move axes (keep existing for gameplay)
                InitializeGameplayAxes();
                
                // Initialize button controls
                InitializeButtons();
                
                initialized = true;
                Debug.Log($"ImprovedPlayerController initialized for player {player.playerIndex}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ImprovedPlayerController.InitializeAndLinkToPlayer: Failed to initialize for player {player?.playerIndex}: {e.Message}");
                initialized = false;
            }
        }
        
        private void InitializeGameplayAxes()
        {
            if (owner == null || controls == null) return;
            
            try
            {
                AimAxis = owner.isUsingMouse ? 
                    new NewInputAxis(controls.PlayerMovement.MousePosition, owner) : 
                    new NewInputAxis(controls.PlayerMovement.StickAiming, owner);
                
                MoveAxis = new NewInputAxis(controls.PlayerMovement.Movement, owner);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ImprovedPlayerController.InitializeGameplayAxes: Failed for player {owner?.playerIndex}: {e.Message}");
            }
        }
        
        private void InitializeButtons()
        {
            if (owner == null || controls == null) return;
            
            try
            {
                // Initialize movement controls
                Pause = new NewInputButton(controls.PlayerMovement.Pause, owner);
                Jump = new NewInputButton(controls.PlayerMovement.Jump, owner);
                DashRightShoulder = new NewInputButton(controls.PlayerMovement.DashLeftShoulder, owner);
                Attack1RightTrigger = new NewInputButton(controls.PlayerMovement.Attack1RightTrigger, owner);
                Attack2LeftTrigger = new NewInputButton(controls.PlayerMovement.Attack2LeftTrigger, owner);
                Attack3Circle = new NewInputButton(controls.PlayerMovement.Attack3Circle, owner);
                ReloadTriangle = new NewInputButton(controls.PlayerMovement.ReloadTriangle, owner);
                InteractRightShoulder = new NewInputButton(controls.PlayerMovement.InteractRightShoulder, owner);
                SwapWeaponSquare = new NewInputButton(controls.PlayerMovement.SwapWeaponSquare, owner);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ImprovedPlayerController.InitializeButtons: Failed for player {owner?.playerIndex}: {e.Message}");
            }
        }
        
        private void FixedUpdate()
        {
            if (!initialized) return;
            MoveAxis?.update();
        }
        
        private void OnDestroy()
        {
            // UINavigation will clean up its own subscriptions
            initialized = false;
        }
    }
}