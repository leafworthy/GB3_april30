using System;
using UnityEngine;
using UnityEngine.InputSystem;
// GameAction is in the global namespace, no need for additional using statements

/// <summary>
/// Interface for input providers that abstract away the input system implementation
/// </summary>
public interface IInputProvider
{
    /// <summary>
    /// Get the current movement input vector
    /// </summary>
    Vector2 MovementInput { get; }
    
    /// <summary>
    /// Get the current aim direction vector
    /// </summary>
    Vector2 AimDirection { get; }
    
    /// <summary>
    /// Get the world space position the player is aiming at
    /// </summary>
    Vector2 AimPosition { get; }
    
    /// <summary>
    /// Check if the attack button is being held
    /// </summary>
    bool IsAttacking { get; }
    
    /// <summary>
    /// Check if the interact button is being pressed
    /// </summary>
    bool IsInteracting { get; }
    
    /// <summary>
    /// Check if the dodge button is being pressed
    /// </summary>
    bool IsDodging { get; }
    
    /// <summary>
    /// Check if the special ability button is being pressed
    /// </summary>
    bool IsUsingSpecial { get; }
    
    /// <summary>
    /// Get the device being used for input (keyboard, gamepad, etc.)
    /// </summary>
    InputDevice CurrentDevice { get; }
    
    /// <summary>
    /// Check if mouse is being used for aiming
    /// </summary>
    bool IsUsingMouse { get; }
    
    /// <summary>
    /// Register for movement input change notifications
    /// </summary>
    void RegisterMovementChangedCallback(Action<Vector2> callback);
    
    /// <summary>
    /// Unregister from movement input change notifications
    /// </summary>
    void UnregisterMovementChangedCallback(Action<Vector2> callback);
    
    /// <summary>
    /// Register for aim direction change notifications
    /// </summary>
    void RegisterAimChangedCallback(Action<Vector2> callback);
    
    /// <summary>
    /// Unregister from aim direction change notifications
    /// </summary>
    void UnregisterAimChangedCallback(Action<Vector2> callback);
    
    /// <summary>
    /// Register for attack button press notifications
    /// </summary>
    void RegisterAttackCallback(Action<bool> callback);
    
    /// <summary>
    /// Unregister from attack button press notifications
    /// </summary>
    void UnregisterAttackCallback(Action<bool> callback);
    
    /// <summary>
    /// Register for interact button press notifications
    /// </summary>
    void RegisterInteractCallback(Action callback);
    
    /// <summary>
    /// Unregister from interact button press notifications
    /// </summary>
    void UnregisterInteractCallback(Action callback);
    
    /// <summary>
    /// Register for dodge button press notifications
    /// </summary>
    void RegisterDodgeCallback(Action callback);
    
    /// <summary>
    /// Unregister from dodge button press notifications
    /// </summary>
    void UnregisterDodgeCallback(Action callback);
    
    /// <summary>
    /// Register for special ability button press notifications
    /// </summary>
    void RegisterSpecialCallback(Action callback);
    
    /// <summary>
    /// Unregister from special ability button press notifications
    /// </summary>
    void UnregisterSpecialCallback(Action callback);
    
    // ========= Legacy Support Methods =========
    
    /// <summary>
    /// Legacy method to get the current aim direction vector.
    /// Use the AimDirection property instead in new code.
    /// </summary>
    Vector2 GetAimDirection();
    
    /// <summary>
    /// Enable this input provider.
    /// </summary>
    void Enable();
    
    /// <summary>
    /// Disable this input provider.
    /// </summary>
    void Disable();
    
    /// <summary>
    /// Check if a specific button is pressed.
    /// For common buttons like attack, interact, etc., use the dedicated properties instead.
    /// </summary>
    /// <param name="action">The button to check</param>
    /// <returns>True if the button is pressed, false otherwise</returns>
    bool IsButtonPressed(GameAction action);
}