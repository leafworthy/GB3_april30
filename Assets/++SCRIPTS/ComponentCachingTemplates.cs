using UnityEngine;

namespace __SCRIPTS
{
    /// <summary>
    /// Collection of proven component caching patterns for performance optimization.
    /// Use these templates to optimize GetComponent calls in performance-critical code.
    /// </summary>
    public static class ComponentCachingTemplates
    {
        #region Template 1: Basic Single Component Caching
        
        /// <summary>
        /// Template for caching a single component reference in Awake().
        /// Use this pattern when you need to access one component frequently.
        /// 
        /// BEFORE:
        /// void Update() {
        ///     GetComponent<Rigidbody2D>().velocity = newVelocity;
        /// }
        /// 
        /// AFTER:
        /// private Rigidbody2D cachedRigidbody;
        /// void Awake() {
        ///     cachedRigidbody = GetComponent<Rigidbody2D>();
        /// }
        /// void Update() {
        ///     if (cachedRigidbody != null) {
        ///         cachedRigidbody.velocity = newVelocity;
        ///     }
        /// }
        /// </summary>
        public class BasicSingleComponentTemplate : MonoBehaviour
        {
            // Step 1: Add cached field at class level
            private Rigidbody2D cachedRigidbody;
            
            // Step 2: Initialize in Awake()
            private void Awake()
            {
                cachedRigidbody = GetComponent<Rigidbody2D>();
                
                // Optional: Log warning if component not found
                if (cachedRigidbody == null)
                {
                    Debug.LogWarning($"Rigidbody2D component not found on {gameObject.name}", this);
                }
            }
            
            // Step 3: Use cached reference with null check
            private void Update()
            {
                if (cachedRigidbody != null)
                {
                    // Use cachedRigidbody instead of GetComponent<Rigidbody2D>()
                    // cachedRigidbody.velocity = newVelocity;
                }
            }
        }
        
        #endregion
        
        #region Template 2: Multiple Component Initialization
        
        /// <summary>
        /// Template for caching multiple components with validation.
        /// Use this when you need several components and want organized initialization.
        /// </summary>
        public class MultipleComponentTemplate : MonoBehaviour
        {
            // Step 1: Declare all cached components
            private Rigidbody2D cachedRigidbody;
            private Collider2D cachedCollider;
            private Animator cachedAnimator;
            private SpriteRenderer cachedRenderer;
            
            // Step 2: Centralized initialization
            private void Awake()
            {
                InitializeComponents();
                ValidateComponents();
            }
            
            private void InitializeComponents()
            {
                cachedRigidbody = GetComponent<Rigidbody2D>();
                cachedCollider = GetComponent<Collider2D>();
                cachedAnimator = GetComponent<Animator>();
                cachedRenderer = GetComponent<SpriteRenderer>();
            }
            
            private void ValidateComponents()
            {
                if (cachedRigidbody == null)
                    Debug.LogWarning($"Missing Rigidbody2D on {gameObject.name}", this);
                
                if (cachedCollider == null)
                    Debug.LogWarning($"Missing Collider2D on {gameObject.name}", this);
                
                // Optional components can skip warnings
                // if (cachedAnimator == null)
                //     Debug.LogWarning($"Missing Animator on {gameObject.name}", this);
            }
            
            // Step 3: Use components with appropriate null checks
            private void Update()
            {
                if (cachedRigidbody != null)
                {
                    // Physics operations
                }
                
                if (cachedAnimator != null)
                {
                    // Animation operations
                }
            }
        }
        
        #endregion
        
        #region Template 3: Lazy Initialization Pattern
        
        /// <summary>
        /// Template for lazy component initialization.
        /// Use this when the component might not be needed immediately or might be added dynamically.
        /// </summary>
        public class LazyInitializationTemplate : MonoBehaviour
        {
            // Step 1: Nullable cached component
            private Rigidbody2D _cachedRigidbody;
            
            // Step 2: Property with lazy initialization
            private Rigidbody2D CachedRigidbody
            {
                get
                {
                    if (_cachedRigidbody == null)
                    {
                        _cachedRigidbody = GetComponent<Rigidbody2D>();
                    }
                    return _cachedRigidbody;
                }
            }
            
            // Step 3: Use the property instead of direct GetComponent calls
            private void Update()
            {
                if (CachedRigidbody != null)
                {
                    // Use CachedRigidbody instead of GetComponent<Rigidbody2D>()
                }
            }
            
            // Step 4: Reset cache if component might be removed/added
            public void ResetComponentCache()
            {
                _cachedRigidbody = null;
            }
        }
        
        #endregion
        
        #region Template 4: Child Component Caching
        
        /// <summary>
        /// Template for caching components from child objects.
        /// Use this to optimize GetComponentInChildren calls.
        /// </summary>
        public class ChildComponentTemplate : MonoBehaviour
        {
            // Step 1: Cache child components
            private AimAbility cachedAimAbility;
            private MoveAbility cachedMoveAbility;
            private JumpAbility cachedJumpAbility;
            
            // Step 2: Initialize child component references
            private void Awake()
            {
                InitializeChildComponents();
            }
            
            private void InitializeChildComponents()
            {
                // Cache GetComponentInChildren calls
                cachedAimAbility = GetComponentInChildren<AimAbility>();
                cachedMoveAbility = GetComponentInChildren<MoveAbility>();
                cachedJumpAbility = GetComponentInChildren<JumpAbility>();
                
                // Validation
                if (cachedAimAbility == null)
                    Debug.LogWarning($"AimAbility not found in children of {gameObject.name}", this);
                if (cachedMoveAbility == null)
                    Debug.LogWarning($"MoveAbility not found in children of {gameObject.name}", this);
                if (cachedJumpAbility == null)
                    Debug.LogWarning($"JumpAbility not found in children of {gameObject.name}", this);
            }
            
            // Step 3: Use cached references
            private void Update()
            {
                if (cachedAimAbility != null)
                {
                    // Use cachedAimAbility instead of GetComponentInChildren<AimAbility>()
                }
            }
            
            // Step 4: Handle dynamic child changes
            public void RefreshChildComponents()
            {
                InitializeChildComponents();
            }
        }
        
        #endregion
        
        #region Template 5: Component Lifecycle Management
        
        /// <summary>
        /// Template for managing component caching with proper cleanup.
        /// Use this for components that need careful lifecycle management.
        /// </summary>
        public class ComponentLifecycleTemplate : MonoBehaviour
        {
            private Rigidbody2D cachedRigidbody;
            private bool componentsInitialized = false;
            
            private void Awake()
            {
                InitializeComponents();
            }
            
            private void InitializeComponents()
            {
                if (componentsInitialized) return;
                
                cachedRigidbody = GetComponent<Rigidbody2D>();
                componentsInitialized = true;
                
                // Subscribe to events that might require re-initialization
                // Example: gameObject.OnComponentAdded += OnComponentChanged;
            }
            
            private void OnDestroy()
            {
                // Cleanup cached references to prevent memory leaks
                cachedRigidbody = null;
                componentsInitialized = false;
                
                // Unsubscribe from events
                // Example: gameObject.OnComponentAdded -= OnComponentChanged;
            }
            
            // Handle component changes during runtime
            private void OnComponentChanged()
            {
                componentsInitialized = false;
                InitializeComponents();
            }
            
            // Safe access method
            private bool TryGetCachedRigidbody(out Rigidbody2D rigidbody)
            {
                if (!componentsInitialized)
                {
                    InitializeComponents();
                }
                
                rigidbody = cachedRigidbody;
                return cachedRigidbody != null;
            }
            
            private void Update()
            {
                if (TryGetCachedRigidbody(out Rigidbody2D rb))
                {
                    // Use rb safely
                }
            }
        }
        
        #endregion
        
        #region Template 6: Performance-Critical Component Access
        
        /// <summary>
        /// Template for ultra-performance-critical component access.
        /// Use this in Update/FixedUpdate loops where every millisecond counts.
        /// </summary>
        public class PerformanceCriticalTemplate : MonoBehaviour
        {
            // Step 1: Cache all components used in Update loops
            private Transform cachedTransform;
            private Rigidbody2D cachedRigidbody;
            private Collider2D cachedCollider;
            
            // Step 2: Cache even built-in component references
            private void Awake()
            {
                // Even cache transform for maximum performance
                cachedTransform = transform;
                cachedRigidbody = GetComponent<Rigidbody2D>();
                cachedCollider = GetComponent<Collider2D>();
                
                // Ensure all critical components are present
                Debug.Assert(cachedTransform != null, "Transform should never be null");
                Debug.Assert(cachedRigidbody != null, $"Rigidbody2D required on {gameObject.name}");
                Debug.Assert(cachedCollider != null, $"Collider2D required on {gameObject.name}");
            }
            
            // Step 3: Ultra-optimized Update loop
            private void Update()
            {
                // No null checks in performance-critical paths if components are required
                // Use Debug.Assert in Awake to ensure they exist
                
                Vector3 position = cachedTransform.position;
                Vector2 velocity = cachedRigidbody.linearVelocity;
                
                // Perform calculations...
                
                cachedTransform.position = position;
                cachedRigidbody.linearVelocity = velocity;
            }
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Utility method to safely get and cache a component.
        /// Use this helper in your custom caching implementations.
        /// </summary>
        public static T GetAndCache<T>(GameObject target, ref T cachedComponent) where T : Component
        {
            if (cachedComponent == null && target != null)
            {
                cachedComponent = target.GetComponent<T>();
            }
            return cachedComponent;
        }
        
        /// <summary>
        /// Utility method to safely get and cache a component from children.
        /// </summary>
        public static T GetAndCacheInChildren<T>(GameObject target, ref T cachedComponent) where T : Component
        {
            if (cachedComponent == null && target != null)
            {
                cachedComponent = target.GetComponentInChildren<T>();
            }
            return cachedComponent;
        }
        
        /// <summary>
        /// Validates that all required components are cached properly.
        /// Call this in Awake after caching components.
        /// </summary>
        public static void ValidateRequiredComponent<T>(T component, GameObject owner, string componentName) where T : Component
        {
            if (component == null)
            {
                Debug.LogError($"Required component {componentName} not found on {owner.name}", owner);
            }
        }
        
        #endregion
    }
    
    #region Component Caching Attribute
    
    /// <summary>
    /// Attribute to mark fields that should be cached components.
    /// Can be used by automated tools to identify optimization opportunities.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class CachedComponentAttribute : System.Attribute
    {
        public bool Required { get; set; } = true;
        public bool LazyInitialize { get; set; } = false;
        
        public CachedComponentAttribute(bool required = true, bool lazyInitialize = false)
        {
            Required = required;
            LazyInitialize = lazyInitialize;
        }
    }
    
    #endregion
}