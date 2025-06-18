# Phase 1 Developer Checklist - Performance Optimization
## Weeks 0-3: Setup & Guaranteed Performance Wins

**Developer:** ________________  
**Start Date:** ________________  
**Target Completion:** ________________  

---

## Week 0: Setup & Validation

### Day 1: Profiling Infrastructure Setup
- [ ] Install Unity Profiler package (Window > Package Manager > Unity Profiler)
- [ ] Configure Unity Profiler for deep profiling
  - [ ] Enable "Deep Profile" in Profiler window
  - [ ] Set sample rate to 1000 samples/second
  - [ ] Enable "Call Stacks" for memory allocations
- [ ] Create custom profiler markers in critical methods:
  ```csharp
  using Unity.Profiling;
  static readonly ProfilerMarker s_UpdateMarker = new ProfilerMarker("Player.Update");
  
  void Update() {
      using (s_UpdateMarker.Auto()) {
          // existing update code
      }
  }
  ```
- [ ] Add profiler markers to these specific methods:
  - [ ] `Player.Update()` and `Player.FixedUpdate()`
  - [ ] `Player.SelectClosestInteractable()`
  - [ ] `AimAbility.Update()`
  - [ ] `FollowCursor.Update()`
- [ ] Set up automated performance testing scene
  - [ ] Create "PerformanceTest" scene with 50+ enemies, 20+ interactables
  - [ ] Add performance test script that runs for 60 seconds
  - [ ] Configure build settings for consistent testing

**🤖 CLAUDE TODO:** Create automated script to add profiler markers to all Update/FixedUpdate methods

### Day 2: Baseline Measurement & Documentation
- [ ] Record baseline performance (3 separate 5-minute sessions):
  - [ ] Session 1: Average frame time: _____ ms
  - [ ] Session 2: Average frame time: _____ ms  
  - [ ] Session 3: Average frame time: _____ ms
  - [ ] **Baseline Average:** _____ ms
- [ ] Document specific hotspots using Unity Profiler:
  - [ ] `Player.SelectClosestInteractable`: _____ ms/frame
  - [ ] GetComponent calls: _____ ms/frame total
  - [ ] Physics queries: _____ ms/frame
  - [ ] Memory allocations: _____ MB/second
- [ ] Screenshot Unity Profiler results and save to `/Documentation/Baseline/`
- [ ] Create performance tracking spreadsheet with weekly targets

**🤖 CLAUDE TODO:** Generate automated performance report script that exports profiler data to CSV

### Day 3: Team Training & Code Review Setup  
- [ ] Complete Unity Profiler training:
  - [ ] Watch Unity Profiler tutorial videos (30 minutes)
  - [ ] Practice identifying performance bottlenecks in current build
  - [ ] Learn Memory Profiler usage for allocation tracking
- [ ] Set up code review process:
  - [ ] Create performance-focused code review checklist
  - [ ] Configure branch protection requiring performance validation
  - [ ] Set up automated performance regression tests
- [ ] Document current component access patterns:
  - [ ] Count GetComponent calls in Update loops: _____
  - [ ] Identify null-check patterns: _____
  - [ ] List components frequently accessed: _____

### Day 4: Technical Spike - Spatial Grid Validation
- [ ] Create prototype spatial grid implementation:
  ```csharp
  public class InteractionGridPrototype : MonoBehaviour {
      // Implement basic grid with 100, 500, 1000 test objects
      // Measure performance vs linear search
  }
  ```
- [ ] Test with varying object counts:
  - [ ] 100 objects: Grid time _____ ms vs Linear _____ ms
  - [ ] 500 objects: Grid time _____ ms vs Linear _____ ms  
  - [ ] 1000 objects: Grid time _____ ms vs Linear _____ ms
- [ ] Measure grid maintenance overhead:
  - [ ] Object add/remove cost: _____ ms
  - [ ] Grid update frequency needed: _____ fps
  - [ ] Memory overhead: _____ MB
- [ ] **Decision:** Use spatial grid? YES / NO
- [ ] Document findings in `/Documentation/TechnicalSpikes/SpatialGridEvaluation.md`

**🤖 CLAUDE TODO:** Create spatial grid prototype and automated performance comparison script

### Day 5: Week 0 Validation & Planning
- [ ] Validate all tools and infrastructure working
- [ ] Confirm baseline measurements are consistent
- [ ] Review technical spike results with team
- [ ] Finalize Week 1 implementation approach based on spike
- [ ] **Week 0 Success Gate:** All tools ready, baseline established, approach validated

**📋 CODE REVIEW CHECKPOINT:** Infrastructure setup and baseline documentation

---

## Week 1: Component Reference Optimization

### Day 1: Audit & Analysis
- [ ] Complete audit of expensive GetComponent calls:
  - [ ] Run search for `GetComponent` in Update/FixedUpdate methods
  - [ ] Document files requiring optimization:
    - [ ] `Player.cs` - Line numbers: _____
    - [ ] `AimAbility.cs` - Line numbers: _____
    - [ ] `FollowCursor.cs` - Line numbers: _____
    - [ ] Attack scripts - List files: _____
- [ ] Create ComponentCache utility class:
  ```csharp
  public static class ComponentCache {
      public static T GetOrCache<T>(ref T cached, GameObject source) where T : Component {
          if (cached == null && source != null) {
              cached = source.GetComponent<T>();
          }
          return cached;
      }
  }
  ```
- [ ] Write unit tests for ComponentCache utility

**🤖 CLAUDE TODO:** Generate automated script to find all GetComponent calls in Update loops

### Day 2: Player.cs Optimization
- [ ] **BEFORE:** Record current Player.cs performance with profiler
  - [ ] `Player.Update()` time: _____ ms
  - [ ] `Player.FixedUpdate()` time: _____ ms
- [ ] Implement component caching in Player.cs:
  ```csharp
  // Add these private fields at top of Player class:
  private AimAbility cachedAimAbility;
  private MoveAbility cachedMoveAbility;
  private JumpAbility cachedJumpAbility;
  private bool componentsInitialized = false;
  
  // Add this method:
  private void InitializeComponentCache() {
      if (SpawnedPlayerGO != null && !componentsInitialized) {
          cachedAimAbility = SpawnedPlayerGO.GetComponentInChildren<AimAbility>();
          cachedMoveAbility = SpawnedPlayerGO.GetComponentInChildren<MoveAbility>();
          cachedJumpAbility = SpawnedPlayerGO.GetComponentInChildren<JumpAbility>();
          componentsInitialized = true;
      }
  }
  
  // Replace GetComponent calls with cached references
  ```
- [ ] Update all Update/FixedUpdate methods to use cached references
- [ ] Add null validation with graceful degradation
- [ ] **AFTER:** Record optimized Player.cs performance
  - [ ] `Player.Update()` time: _____ ms (improvement: ____%)
  - [ ] `Player.FixedUpdate()` time: _____ ms (improvement: ____%)

### Day 3: AimAbility.cs & FollowCursor.cs Optimization
- [ ] **BEFORE:** Record baseline performance:
  - [ ] `AimAbility.Update()` time: _____ ms
  - [ ] `FollowCursor.Update()` time: _____ ms
- [ ] Optimize AimAbility.cs component access:
  - [ ] Cache frequently accessed components in Awake()
  - [ ] Replace GetComponent calls with cached references
  - [ ] Add component validation methods
- [ ] Optimize FollowCursor.cs:
  - [ ] Cache AimAbility reference
  - [ ] Implement lazy initialization pattern
  - [ ] Add null checking with fallback behavior
- [ ] **AFTER:** Record optimized performance:
  - [ ] `AimAbility.Update()` time: _____ ms (improvement: ____%)
  - [ ] `FollowCursor.Update()` time: _____ ms (improvement: ____%)

**📋 CODE REVIEW CHECKPOINT:** Component caching implementation and performance validation

### Day 4: Attack Scripts Optimization
- [ ] Identify attack scripts with GetComponent calls:
  - [ ] List files requiring optimization: _____
- [ ] Cache physics components (Rigidbody2D, Collider2D):
  ```csharp
  private Rigidbody2D cachedRigidbody;
  private Collider2D cachedCollider;
  
  void Awake() {
      cachedRigidbody = GetComponent<Rigidbody2D>();
      cachedCollider = GetComponent<Collider2D>();
  }
  ```
- [ ] Update attack scripts to use cached references
- [ ] Add validation for required components
- [ ] Test all attack functionality to ensure no regressions

### Day 5: Week 1 Validation & Testing
- [ ] Run comprehensive performance tests:
  - [ ] Overall Update loop improvement: ____% (Target: 10%+)
  - [ ] Zero gameplay regressions confirmed
  - [ ] All attack systems functioning correctly
- [ ] Update performance tracking spreadsheet
- [ ] Create before/after profiler comparison screenshots
- [ ] **Week 1 Success Gate:** 10%+ Update loop improvement achieved

**🤖 CLAUDE TODO:** Generate automated performance regression test that runs after each optimization

---

## Week 2: Player Interaction System Optimization

### Day 1: Interaction System Analysis & Implementation Choice
- [ ] Review Week 0 technical spike results
- [ ] **Implementation Choice:** Spatial Grid / Simple Distance Culling
- [ ] If Spatial Grid chosen:
  - [ ] Implement InteractionSpatialGrid class
  - [ ] Add grid update mechanisms for moving objects
  - [ ] Create grid visualization for debugging
- [ ] If Simple Distance Culling chosen:
  - [ ] Implement early distance rejection in SelectClosestInteractable
  - [ ] Add configurable interaction range
  - [ ] Optimize Vector2.Distance calls to use sqrMagnitude

### Day 2: SelectClosestInteractable Optimization
- [ ] **BEFORE:** Record baseline performance:
  - [ ] `SelectClosestInteractable()` time: _____ ms
  - [ ] Number of interactables checked: _____
- [ ] Implement chosen optimization approach:
  ```csharp
  // For Simple Distance Culling:
  private void SelectClosestInteractable() {
      const float MAX_INTERACTION_DISTANCE_SQR = 100f; // 10 units squared
      
      PlayerInteractable closest = null;
      float closestDistanceSqr = float.MaxValue;
      
      foreach(var interactable in allInteractables) {
          Vector2 offset = interactable.transform.position - transform.position;
          float distanceSqr = offset.sqrMagnitude;
          
          if (distanceSqr > MAX_INTERACTION_DISTANCE_SQR) continue; // Early rejection
          
          if (distanceSqr < closestDistanceSqr) {
              closestDistanceSqr = distanceSqr;
              closest = interactable;
          }
      }
      
      currentInteractable = closest;
  }
  ```
- [ ] Add profiler markers to measure optimization impact
- [ ] Test with varying numbers of interactables (10, 50, 100+)

### Day 3: Integration & Edge Case Testing
- [ ] Integrate optimized interaction system with Player.cs
- [ ] Test edge cases:
  - [ ] No interactables in range
  - [ ] Multiple interactables at same distance
  - [ ] Interactables being destroyed during selection
  - [ ] Player moving at high speed
- [ ] Validate interaction UI updates correctly
- [ ] Ensure interaction prompts appear/disappear smoothly

**📋 CODE REVIEW CHECKPOINT:** Interaction optimization implementation and edge case handling

### Day 4: Performance Validation & Tuning
- [ ] **AFTER:** Record optimized performance:
  - [ ] `SelectClosestInteractable()` time: _____ ms (improvement: ____%)
  - [ ] Objects checked per frame: _____ (reduction: ____%)
- [ ] Tune interaction distance for optimal gameplay:
  - [ ] Test various distances: 5, 8, 10, 12 units
  - [ ] Optimal distance: _____ units
- [ ] Profile with maximum object counts to ensure scalability
- [ ] Document performance improvements in tracking spreadsheet

### Day 5: Week 2 Validation & Testing  
- [ ] Run comprehensive interaction testing:
  - [ ] All interaction types working correctly
  - [ ] Performance target achieved: 30%+ improvement in interaction detection
  - [ ] No gameplay regressions
- [ ] **Week 2 Success Gate:** 30%+ interaction optimization achieved

**🤖 CLAUDE TODO:** Create automated interaction performance test with varying object densities

---

## Week 3: Memory Allocation Reduction

### Day 1: Memory Allocation Analysis
- [ ] Run Unity Memory Profiler to identify allocation hotspots:
  - [ ] Enable "Track Object Allocations" in Memory Profiler
  - [ ] Record 5-minute gameplay session
  - [ ] Identify top allocation sources:
    - [ ] Vector3/Vector2 allocations: _____ per second
    - [ ] String concatenations: _____ per second  
    - [ ] Collection allocations: _____ per second
- [ ] Document allocation hotspots with line numbers:
  - [ ] `AimAbility.cs` rotation loops: Line _____
  - [ ] Debug.Log statements: List files _____
  - [ ] Other allocation sources: _____

**🤖 CLAUDE TODO:** Generate script to find all new Vector3/Vector2 allocations in Update loops

### Day 2: Vector3/Vector2 Optimization
- [ ] **BEFORE:** Record baseline GC allocations: _____ MB/second
- [ ] Implement cached Vector3 pattern in AimAbility.cs:
  ```csharp
  // Add static cached vectors at class level:
  private static readonly Vector3 cachedRotation = Vector3.zero;
  
  // Replace allocation in RotateWithAim loop:
  foreach (var obj in body.RotateWithAim) {
      cachedRotation.Set(0, 0, rotation); // Reuse existing Vector3
      obj.transform.eulerAngles = cachedRotation;
  }
  ```
- [ ] Apply similar pattern to other Vector allocation hotspots
- [ ] Cache frequently used Vector2 calculations
- [ ] Validate visual appearance unchanged after optimization

### Day 3: String Allocation & Debug Optimization
- [ ] Wrap Debug.Log statements in conditional compilation:
  ```csharp
  #if UNITY_EDITOR
      Debug.Log($"Player health: {currentHealth}/{maxHealth}");
  #endif
  ```
- [ ] Replace string concatenation with StringBuilder where needed
- [ ] Remove or optimize debug output in production builds
- [ ] Implement structured logging system for performance-critical paths

### Day 4: Collection & Object Pooling
- [ ] Identify frequently allocated collections (Lists, Arrays)
- [ ] Implement object reuse patterns:
  ```csharp
  // Reuse collections instead of creating new ones
  private List<PlayerInteractable> reusableList = new List<PlayerInteractable>();
  
  public List<PlayerInteractable> GetNearbyInteractables() {
      reusableList.Clear(); // Reuse existing list
      // Fill list with results
      return reusableList;
  }
  ```
- [ ] Cache commonly used objects (Transform references, etc.)
- [ ] Validate object pooling doesn't affect gameplay logic

**📋 CODE REVIEW CHECKPOINT:** Memory optimization implementation and allocation reduction

### Day 5: Week 3 Validation & Testing
- [ ] **AFTER:** Record optimized GC allocations: _____ MB/second
- [ ] Calculate improvement: ____% reduction (Target: 15%+)
- [ ] Run extended gameplay session (30 minutes) to test memory stability
- [ ] Validate no memory leaks introduced by optimizations
- [ ] **Week 3 Success Gate:** 15%+ memory allocation reduction achieved

**🤖 CLAUDE TODO:** Create automated memory allocation tracking and reporting system

---

## Phase 1 Summary & Validation

### Overall Performance Validation
- [ ] Run comprehensive performance test suite:
  - [ ] Frame time improvement: ____% (Target: 15%+ overall)
  - [ ] Update loop optimization: ____% (Target: 25%+)
  - [ ] Memory allocation reduction: ____% (Target: 15%+)
- [ ] Compare before/after Unity Profiler results
- [ ] Validate zero gameplay regressions
- [ ] Update performance tracking documentation

### Code Quality & Documentation
- [ ] All changes peer reviewed and approved
- [ ] Performance optimization patterns documented
- [ ] Code comments added explaining optimization rationale
- [ ] Unit tests written for critical optimizations
- [ ] Knowledge transfer documentation updated

### Phase 1 Success Criteria
- [ ] **Primary Target:** 15-20% overall frame time improvement ✅/❌
- [ ] **Component Caching:** 10%+ Update loop improvement ✅/❌  
- [ ] **Interaction Optimization:** 30%+ detection improvement ✅/❌
- [ ] **Memory Reduction:** 15%+ allocation reduction ✅/❌
- [ ] **Zero Regressions:** All gameplay functionality intact ✅/❌

**📋 FINAL CODE REVIEW CHECKPOINT:** Complete Phase 1 implementation review and performance validation

---

## Notes & Issues Tracking

### Issues Encountered:
1. ________________________________
2. ________________________________  
3. ________________________________

### Performance Improvements Achieved:
1. ________________________________
2. ________________________________
3. ________________________________

### Recommendations for Phase 2:
1. ________________________________
2. ________________________________
3. ________________________________

---

**Completed By:** ________________  
**Date:** ________________  
**Phase 1 Status:** PASSED / NEEDS REVISION  
**Ready for Phase 2:** YES / NO