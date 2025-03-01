using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Adapter that connects the existing UnityInputProvider to the new IInputProvider interface
/// </summary>
public class UnityInputProviderAdapter : MonoBehaviour, IInputProvider
{
    private UnityInputProvider originalProvider;
    
    // Properties for the new interface
    public Vector2 MovementInput => originalProvider?.GetMovementDirection() ?? Vector2.zero;
    public Vector2 AimDirection => originalProvider?.GetAimDirection() ?? Vector2.zero;
    public Vector2 AimPosition => GetWorldAimPosition();
    public bool IsAttacking => originalProvider?.IsButtonPressed(GameAction.PrimaryAttack) ?? false;
    public bool IsInteracting => originalProvider?.IsButtonPressed(GameAction.Interact) ?? false;
    public bool IsDodging => originalProvider?.IsButtonPressed(GameAction.Dash) ?? false;
    public bool IsUsingSpecial => originalProvider?.IsButtonPressed(GameAction.SpecialAttack) ?? false;
    public InputDevice CurrentDevice => null; // Not available in the original provider
    public bool IsUsingMouse => originalProvider?.IsUsingMouse ?? false;
    
    private void Awake()
    {
        // Get the original provider
        originalProvider = GetComponent<UnityInputProvider>();
        
        if (originalProvider == null)
        {
            Debug.LogError("UnityInputProviderAdapter requires a UnityInputProvider component");
            enabled = false;
        }
    }
    
    // Helper method to convert aim direction to world position
    private Vector2 GetWorldAimPosition()
    {
        if (IsUsingMouse)
        {
            // For mouse input, this would be handled by the original provider
            // We'll need to get the actual mouse position
            Vector2 mousePosition = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            if (Camera.main != null)
            {
                return Camera.main.ScreenToWorldPoint(mousePosition);
            }
            return Vector2.zero;
        }
        else
        {
            // For gamepad input, calculate based on aim direction
            Vector2 aimDir = AimDirection;
            if (aimDir.magnitude < 0.1f)
            {
                aimDir = MovementInput.normalized;
            }
            
            return (Vector2)transform.position + aimDir * 10f;
        }
    }
    
    // IInputProvider implementation - callback registration methods
    public void RegisterMovementChangedCallback(Action<Vector2> callback)
    {
        if (originalProvider != null)
        {
            originalProvider.RegisterMovementChangedCallback(callback);
        }
    }
    
    public void UnregisterMovementChangedCallback(Action<Vector2> callback)
    {
        if (originalProvider != null)
        {
            originalProvider.UnregisterMovementChangedCallback(callback);
        }
    }
    
    public void RegisterAimChangedCallback(Action<Vector2> callback)
    {
        if (originalProvider != null)
        {
            originalProvider.RegisterAimChangedCallback(callback);
        }
    }
    
    public void UnregisterAimChangedCallback(Action<Vector2> callback)
    {
        if (originalProvider != null)
        {
            originalProvider.UnregisterAimChangedCallback(callback);
        }
    }
    
    // New interface methods that map to the original provider's button system
    // Removed duplicate method declaration - implementation is below
    
    // Dictionary to store wrapped callbacks for proper unregistration
    private Dictionary<Action<bool>, (Action pressAction, Action releaseAction)> wrappedAttackCallbacks = 
        new Dictionary<Action<bool>, (Action pressAction, Action releaseAction)>();
    
    public void RegisterAttackCallback(Action<bool> callback)
    {
        if (originalProvider != null)
        {
            // Create wrapped actions
            Action pressAction = () => callback(true);
            Action releaseAction = () => callback(false);
            
            // Store for later unregistration
            wrappedAttackCallbacks[callback] = (pressAction, releaseAction);
            
            // Register with original provider
            originalProvider.RegisterButtonPressedCallback(GameAction.PrimaryAttack, pressAction);
            originalProvider.RegisterButtonReleasedCallback(GameAction.PrimaryAttack, releaseAction);
        }
    }
    
    public void UnregisterAttackCallback(Action<bool> callback)
    {
        if (originalProvider != null && wrappedAttackCallbacks.TryGetValue(callback, out var actions))
        {
            // Unregister using stored wrapped actions
            originalProvider.UnregisterButtonPressedCallback(GameAction.PrimaryAttack, actions.pressAction);
            originalProvider.UnregisterButtonReleasedCallback(GameAction.PrimaryAttack, actions.releaseAction);
            
            // Remove from dictionary
            wrappedAttackCallbacks.Remove(callback);
        }
    }
    
    public void RegisterInteractCallback(Action callback)
    {
        if (originalProvider != null)
        {
            originalProvider.RegisterButtonPressedCallback(GameAction.Interact, callback);
        }
    }
    
    public void UnregisterInteractCallback(Action callback)
    {
        if (originalProvider != null)
        {
            originalProvider.UnregisterButtonPressedCallback(GameAction.Interact, callback);
        }
    }
    
    public void RegisterDodgeCallback(Action callback)
    {
        if (originalProvider != null)
        {
            originalProvider.RegisterButtonPressedCallback(GameAction.Dash, callback);
        }
    }
    
    public void UnregisterDodgeCallback(Action callback)
    {
        if (originalProvider != null)
        {
            originalProvider.UnregisterButtonPressedCallback(GameAction.Dash, callback);
        }
    }
    
    public void RegisterSpecialCallback(Action callback)
    {
        if (originalProvider != null)
        {
            originalProvider.RegisterButtonPressedCallback(GameAction.SpecialAttack, callback);
        }
    }
    
    public void UnregisterSpecialCallback(Action callback)
    {
        if (originalProvider != null)
        {
            originalProvider.UnregisterButtonPressedCallback(GameAction.SpecialAttack, callback);
        }
    }
    
    // ========= Legacy Support Methods Implementation =========
    
    /// <summary>
    /// Legacy method to get the current aim direction vector.
    /// </summary>
    public Vector2 GetAimDirection()
    {
        return originalProvider?.GetAimDirection() ?? Vector2.zero;
    }
    
    /// <summary>
    /// Enable this input provider.
    /// </summary>
    public void Enable()
    {
        if (originalProvider != null)
        {
            originalProvider.Enable();
        }
    }
    
    /// <summary>
    /// Disable this input provider.
    /// </summary>
    public void Disable()
    {
        if (originalProvider != null)
        {
            originalProvider.Disable();
        }
    }
    
    /// <summary>
    /// Check if a specific button is pressed.
    /// </summary>
    public bool IsButtonPressed(GameAction action)
    {
        return originalProvider != null && originalProvider.IsButtonPressed(action);
    }
}