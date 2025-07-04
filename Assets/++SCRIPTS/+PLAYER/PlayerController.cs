﻿using UnityEngine;

namespace __SCRIPTS
{
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
        public NewControlButton DashRightShoulder;
        public NewControlButton Attack1RightTrigger;
        public NewControlButton Attack2LeftTrigger;
        public NewControlButton Attack3Circle;
        public NewControlButton ReloadTriangle;
        public NewControlButton SwapWeaponSquare;
        public NewControlButton InteractRightShoulder;
        public NewControlButton Pause;
        public NewControlButton Unpause;


        private bool initialized;

        private void Start()
        {
            SetAxes();
        }

      
        public void InitializeAndLinkToPlayer(Player player)
        {
            if (initialized) return;
            
            if (player == null)
            {

                return;
            }

            try
            {
                owner = player; 
                controls = new PlayerControls();
                
                // Safe axes initialization - only after owner is set
                SetAxes();

                // Initialize UI controls
                Select = new NewInputButton(controls.UI.Select, owner);
                Cancel = new NewInputButton(controls.UI.Cancel, owner);
                Unpause = new NewInputButton(controls.UI.Unpause, owner);
                // Note: Removed duplicate Select assignment
                
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
                
                initialized = true;
            }
            catch (System.Exception e)
            {

                initialized = false;
            }
        }

        private void FixedUpdate()
        {
            if (!initialized) return;
            MoveAxis?.update();
        }

        private void SetAxes()
        {
            if (owner == null || controls == null)
            {

                return;
            }

            try
            {
                AimAxis = owner.isUsingMouse ? 
                    new NewInputAxis(controls.PlayerMovement.MousePosition, owner) : 
                    new NewInputAxis(controls.PlayerMovement.StickAiming, owner);

                UIAxis = new NewInputAxis(controls.UI.Movement, owner);
                MoveAxis = new NewInputAxis(controls.PlayerMovement.Movement, owner);
            }
            catch (System.Exception e)
            {

            }
        }
    }
}