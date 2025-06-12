using UnityEngine;

namespace __SCRIPTS
{
    /// <summary>
    /// Utility class to help migrate from old input system to new simplified UI navigation
    /// Can be attached to existing Player GameObjects to upgrade them
    /// </summary>
    public class UINavigationMigrator : MonoBehaviour
    {
        [Header("Migration Settings")]
        [SerializeField] private bool useImprovedSystem = true;
        [SerializeField] private bool debugLogging = false;
        
        private Player player;
        private PlayerController oldController;
        private ImprovedPlayerController newController;
        
        private void Awake()
        {
            player = GetComponent<Player>();
            if (player == null)
            {

                enabled = false;
                return;
            }
            
            MigrateToImprovedSystem();
        }
        
        [ContextMenu("Migrate to Improved System")]
        public void MigrateToImprovedSystem()
        {
            if (!useImprovedSystem) return;
            
            if (debugLogging) Debug.Log($"Migrating player {player.playerIndex} to improved UI navigation system");
            
            // Get existing controller
            oldController = GetComponent<PlayerController>();
            
            // Add new controller if it doesn't exist
            newController = GetComponent<ImprovedPlayerController>();
            if (newController == null)
            {
                newController = gameObject.AddComponent<ImprovedPlayerController>();
            }
            
            // Initialize new controller if player is already set up
            if (player.input != null)
            {
                newController.InitializeAndLinkToPlayer(player);
            }
            
            // Disable old controller (but don't destroy it in case we need to rollback)
            if (oldController != null)
            {
                oldController.enabled = false;
                if (debugLogging) Debug.Log($"Disabled old PlayerController for player {player.playerIndex}");
            }
            
            if (debugLogging) Debug.Log($"Successfully migrated player {player.playerIndex} to improved system");
        }
        
        [ContextMenu("Rollback to Old System")]
        public void RollbackToOldSystem()
        {
            if (debugLogging) Debug.Log($"Rolling back player {player.playerIndex} to old input system");
            
            // Enable old controller
            if (oldController != null)
            {
                oldController.enabled = true;
                if (debugLogging) Debug.Log($"Enabled old PlayerController for player {player.playerIndex}");
            }
            
            // Disable new controller
            if (newController != null)
            {
                newController.enabled = false;
                if (debugLogging) Debug.Log($"Disabled ImprovedPlayerController for player {player.playerIndex}");
            }
            
            useImprovedSystem = false;
        }
        
        /// <summary>
        /// Get the appropriate UI navigation system based on current configuration
        /// </summary>
        public bool HasImprovedNavigation()
        {
            return newController != null && newController.enabled && newController.UINavigation != null;
        }
        
        /// <summary>
        /// Get the current navigation system for external use
        /// </summary>
        public SimpleUINavigation GetUINavigation()
        {
            if (HasImprovedNavigation())
            {
                return newController.UINavigation;
            }
            return null;
        }
        
        // Store delegates for proper unsubscription
        private System.Action<IControlAxis> storedLeftHandler;
        private System.Action<IControlAxis> storedRightHandler;
        private System.Action<NewControlButton> storedSelectHandler;
        private System.Action<NewControlButton> storedCancelHandler;
        
        /// <summary>
        /// Subscribe to navigation events with automatic system detection
        /// WARNING: This API doesn't support multiple simultaneous subscriptions due to delegate storage limitations.
        /// Use the improved controller directly for complex scenarios.
        /// </summary>
        public void SubscribeToNavigation(
            System.Action onLeft = null,
            System.Action onRight = null,
            System.Action onSelect = null,
            System.Action onCancel = null)
        {
            // Clear any existing subscriptions first
            UnsubscribeFromNavigation();
            
            if (HasImprovedNavigation())
            {
                var nav = newController.UINavigation;
                if (onLeft != null) nav.OnNavigateLeft += onLeft;
                if (onRight != null) nav.OnNavigateRight += onRight;
                if (onSelect != null) nav.OnSelect += onSelect;
                if (onCancel != null) nav.OnCancel += onCancel;
                
                if (debugLogging) Debug.Log($"Subscribed to improved navigation for player {player.playerIndex}");
            }
            else if (oldController != null && oldController.enabled)
            {
                // Create and store delegate references for old system
                if (onLeft != null)
                {
                    storedLeftHandler = (axis) => onLeft();
                    oldController.UIAxis.OnLeft += storedLeftHandler;
                }
                if (onRight != null)
                {
                    storedRightHandler = (axis) => onRight();
                    oldController.UIAxis.OnRight += storedRightHandler;
                }
                if (onSelect != null)
                {
                    storedSelectHandler = (btn) => onSelect();
                    oldController.Select.OnPress += storedSelectHandler;
                }
                if (onCancel != null)
                {
                    storedCancelHandler = (btn) => onCancel();
                    oldController.Cancel.OnPress += storedCancelHandler;
                }
                
                if (debugLogging) Debug.Log($"Subscribed to old navigation for player {player.playerIndex}");
            }
        }
        
        /// <summary>
        /// Unsubscribe from navigation events with automatic system detection
        /// </summary>
        public void UnsubscribeFromNavigation()
        {
            if (HasImprovedNavigation())
            {
                // For improved system, we can't unsubscribe without the original delegates
                // This is a limitation - external code should manage their own subscriptions
                if (debugLogging) Debug.LogWarning($"Cannot unsubscribe from improved navigation without original delegates for player {player.playerIndex}");
            }
            else if (oldController != null)
            {
                // Unsubscribe using stored delegates
                if (storedLeftHandler != null)
                {
                    oldController.UIAxis.OnLeft -= storedLeftHandler;
                    storedLeftHandler = null;
                }
                if (storedRightHandler != null)
                {
                    oldController.UIAxis.OnRight -= storedRightHandler;
                    storedRightHandler = null;
                }
                if (storedSelectHandler != null)
                {
                    oldController.Select.OnPress -= storedSelectHandler;
                    storedSelectHandler = null;
                }
                if (storedCancelHandler != null)
                {
                    oldController.Cancel.OnPress -= storedCancelHandler;
                    storedCancelHandler = null;
                }
                
                if (debugLogging) Debug.Log($"Unsubscribed from old navigation for player {player.playerIndex}");
            }
        }
    }
}