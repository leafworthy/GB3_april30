using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace __SCRIPTS
{
    /// <summary>
    /// Simplified, robust UI navigation system that replaces the complex NewInputAxis approach
    /// Designed to eliminate navigation timing issues and provide reliable left/right input
    /// </summary>
    public class SimpleUINavigation : MonoBehaviour
    {
        [Header("Navigation Settings")]
        [SerializeField] private float repeatDelay = 0.5f;      // Initial delay before repeating
        [SerializeField] private float repeatRate = 0.15f;     // Time between repeats
        [SerializeField] private float deadZone = 0.3f;        // Threshold for analog input
        
        // State tracking
        private Vector2 currentInput;
        private bool isNavigatingLeft;
        private bool isNavigatingRight;
        private float lastNavigationTime;
        private bool canRepeat;
        
        // Events
        public event Action OnNavigateLeft;
        public event Action OnNavigateRight;
        public event Action OnNavigateUp;
        public event Action OnNavigateDown;
        public event Action OnSelect;
        public event Action OnCancel;
        
        // Input references
        private InputAction moveAction;
        private InputAction selectAction;
        private InputAction cancelAction;
        
        private Player owner;
        
        public void Initialize(Player player, PlayerControls controls)
        {
            owner = player;
            
            // Get input actions
            moveAction = controls.UI.Movement;
            selectAction = controls.UI.Select;
            cancelAction = controls.UI.Cancel;
            
            // Subscribe to input events
            moveAction.performed += OnMovePerformed;
            moveAction.canceled += OnMoveCanceled;
            selectAction.performed += OnSelectPerformed;
            cancelAction.performed += OnCancelPerformed;
            
            ResetState();
        }
        
        private void OnDestroy()
        {
            // Clean up event subscriptions
            if (moveAction != null)
            {
                moveAction.performed -= OnMovePerformed;
                moveAction.canceled -= OnMoveCanceled;
            }
            if (selectAction != null) selectAction.performed -= OnSelectPerformed;
            if (cancelAction != null) cancelAction.performed -= OnCancelPerformed;
        }
        
        private void Update()
        {
            HandleRepeatNavigation();
        }
        
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            ProcessNavigationInput(input);
        }
        
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            ResetState();
        }
        
        private void OnSelectPerformed(InputAction.CallbackContext context)
        {
            OnSelect?.Invoke();
        }
        
        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            OnCancel?.Invoke();
        }
        
        private void ProcessNavigationInput(Vector2 input)
        {
            currentInput = input;
            
            // Check horizontal movement
            bool shouldNavigateLeft = input.x < -deadZone;
            bool shouldNavigateRight = input.x > deadZone;
            
            // Handle left navigation
            if (shouldNavigateLeft && !isNavigatingLeft)
            {
                isNavigatingLeft = true;
                isNavigatingRight = false;
                TriggerNavigation(OnNavigateLeft);
            }
            // Handle right navigation  
            else if (shouldNavigateRight && !isNavigatingRight)
            {
                isNavigatingRight = true;
                isNavigatingLeft = false;
                TriggerNavigation(OnNavigateRight);
            }
            // Handle stopping navigation
            else if (!shouldNavigateLeft && !shouldNavigateRight)
            {
                ResetHorizontalState();
            }
            
            // Handle vertical movement (similar logic)
            if (input.y > deadZone)
            {
                OnNavigateUp?.Invoke();
            }
            else if (input.y < -deadZone)
            {
                OnNavigateDown?.Invoke();
            }
        }
        
        private void TriggerNavigation(Action navigationEvent)
        {
            navigationEvent?.Invoke();
            lastNavigationTime = Time.time;
            canRepeat = false; // Start with no repeat, will enable after delay
        }
        
        private void HandleRepeatNavigation()
        {
            if (!isNavigatingLeft && !isNavigatingRight) return;
            
            float timeSinceLastNav = Time.time - lastNavigationTime;
            
            // Enable repeat after initial delay
            if (!canRepeat && timeSinceLastNav >= repeatDelay)
            {
                canRepeat = true;
                lastNavigationTime = Time.time; // Reset timer for repeat rate
            }
            
            // Trigger repeat navigation
            if (canRepeat && timeSinceLastNav >= repeatRate)
            {
                if (isNavigatingLeft && currentInput.x < -deadZone)
                {
                    OnNavigateLeft?.Invoke();
                    lastNavigationTime = Time.time;
                }
                else if (isNavigatingRight && currentInput.x > deadZone)
                {
                    OnNavigateRight?.Invoke();
                    lastNavigationTime = Time.time;
                }
            }
        }
        
        private void ResetState()
        {
            ResetHorizontalState();
            currentInput = Vector2.zero;
        }
        
        private void ResetHorizontalState()
        {
            isNavigatingLeft = false;
            isNavigatingRight = false;
            canRepeat = false;
        }
        
        // Public getters for debugging
        public bool IsNavigatingLeft => isNavigatingLeft;
        public bool IsNavigatingRight => isNavigatingRight;
        public Vector2 CurrentInput => currentInput;
    }
}