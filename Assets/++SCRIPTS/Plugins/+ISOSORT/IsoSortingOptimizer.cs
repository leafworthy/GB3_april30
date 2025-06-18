using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
    /// <summary>
    /// Add this component to GameObjects with IsoSpriteSorting to enable optimized change detection.
    /// This is the safest, most practical optimization approach.
    /// </summary>
    [RequireComponent(typeof(IsoSpriteSorting))]
    public class IsoSortingOptimizer : MonoBehaviour
    {
        #region Settings
        [Header("Change Detection")]
        [SerializeField] private bool enableOptimization = true;
        [SerializeField] private float movementThreshold = 0.01f;
        [SerializeField] private bool onlyTrackMovableSprites = true;
        #endregion

        #region State
        private IsoSpriteSorting targetSprite;
        private Vector3 lastPosition;
        private bool lastActiveState;
        private Transform cachedTransform;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            targetSprite = GetComponent<IsoSpriteSorting>();
            cachedTransform = transform;
            InitializeTracking();
        }

        private void LateUpdate()
        {
            if (enableOptimization && Application.isPlaying)
            {
                CheckForChanges();
            }
        }
        #endregion

        #region Change Detection
        private void InitializeTracking()
        {
            if (cachedTransform != null)
            {
                lastPosition = cachedTransform.position;
            }
            lastActiveState = gameObject.activeInHierarchy;
        }

        private void CheckForChanges()
        {
            if (targetSprite == null || cachedTransform == null) return;

            // Only track movable sprites if setting is enabled
            if (onlyTrackMovableSprites && !targetSprite.isMovable) return;

            bool hasChanged = false;

            // Check position change
            Vector3 currentPosition = cachedTransform.position;
            if (Vector3.Distance(currentPosition, lastPosition) > movementThreshold)
            {
                lastPosition = currentPosition;
                hasChanged = true;
            }

            // Check active state change
            bool currentActiveState = gameObject.activeInHierarchy;
            if (currentActiveState != lastActiveState)
            {
                lastActiveState = currentActiveState;
                hasChanged = true;
            }

            // Notify optimization system
            if (hasChanged)
            {
                SimpleOptimizedSystem.NotifyMovement(targetSprite);
            }
        }
        #endregion

        #region Public API
        public void ForceUpdate()
        {
            if (targetSprite != null)
            {
                SimpleOptimizedSystem.MarkSpriteChanged(targetSprite);
            }
        }

        public void ResetTracking()
        {
            InitializeTracking();
        }
        #endregion

        #region Editor Support
        #if UNITY_EDITOR
        [ContextMenu("Add to All IsoSpriteSorting in Scene")]
        private void AddToAllInScene()
        {
            var allIsoSprites = FindObjectsByType<IsoSpriteSorting>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            int added = 0;

            foreach (var isoSprite in allIsoSprites)
            {
                if (isoSprite.GetComponent<IsoSortingOptimizer>() == null)
                {
                    isoSprite.gameObject.AddComponent<IsoSortingOptimizer>();
                    added++;
                }
            }

            Debug.Log($"Added IsoSortingOptimizer to {added} GameObjects");
        }

        [ContextMenu("Remove from All in Scene")]
        private void RemoveFromAllInScene()
        {
            var allOptimizers = FindObjectsByType<IsoSortingOptimizer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            int removed = 0;

            foreach (var optimizer in allOptimizers)
            {
                if (optimizer != this) // Don't remove self
                {
                    DestroyImmediate(optimizer);
                    removed++;
                }
            }

            Debug.Log($"Removed {removed} IsoSortingOptimizer components from scene");
        }
        #endif
        #endregion
    }
}