# Activity Completion Interface Specification

## Overview
This specification defines how to extend the existing IActivity system with completion capability for animation interruption. The design prioritizes simplicity, backward compatibility, and optional implementation.

## Current System Analysis

### Existing Components
- **IActivity Interface**: Located in `/Assets/++SCRIPTS/+CORE/Interfaces.cs`
  - Single property: `string VerbName { get; }`
- **ActivityHandler Class**: Located in `/Assets/++SCRIPTS/+ABILITIES/ActivityHandler.cs`
  - Manages `Arms` and `Legs` activity handlers
  - Methods: `Do()`, `StopSafely()`, `StopCurrentActivity()`
- **Body Class**: Contains `doableArms` and `doableLegs` ActivityHandler instances

### Current Activity Patterns
1. Activities implement `IActivity` with `VerbName` property
2. Activities call `body.doableArms.Do(this)` or `body.doableLegs.Do(this)` to start
3. Activities call `body.doableArms.StopSafely(this)` or `body.doableLegs.StopSafely(this)` to stop
4. ActivityHandler validates activity ownership before stopping

## Interface Design

### Option 1: Extend IActivity Interface (Recommended)
```csharp
namespace GangstaBean.Core
{
    /// <summary>
    /// Interface for components that can be activities
    /// </summary>
    public interface IActivity
    {
        public string VerbName { get; }
        
        /// <summary>
        /// Called when activity should complete gracefully before being stopped.
        /// This method is optional - activities can choose not to implement completion logic.
        /// </summary>
        /// <param name="reason">Why the completion is being triggered</param>
        /// <param name="newActivity">The activity that will replace this one (can be null)</param>
        /// <returns>True if completion was handled, false to use normal stopping behavior</returns>
        public virtual bool CompleteActivity(CompletionReason reason, IActivity newActivity = null) => false;
    }
    
    /// <summary>
    /// Reasons why an activity completion is being triggered
    /// </summary>
    public enum CompletionReason
    {
        /// <summary>New activity is starting and needs this body part</summary>
        NewActivity,
        /// <summary>Animation system is interrupting for a new animation</summary>
        AnimationInterrupt,
        /// <summary>Player manually stopped the activity</summary>
        PlayerStop,
        /// <summary>System forced stop (death, pause, etc.)</summary>
        SystemStop,
        /// <summary>Activity duration or conditions ended naturally</summary>
        NaturalEnd
    }
}
```

### Option 2: Separate Interface (Alternative)
```csharp
namespace GangstaBean.Core
{
    /// <summary>
    /// Interface for activities that support graceful completion
    /// </summary>
    public interface ICompletableActivity
    {
        /// <summary>
        /// Called when activity should complete gracefully before being stopped
        /// </summary>
        /// <param name="reason">Why the completion is being triggered</param>
        /// <param name="newActivity">The activity that will replace this one (can be null)</param>
        /// <returns>True if completion was handled, false to use normal stopping behavior</returns>
        bool CompleteActivity(CompletionReason reason, IActivity newActivity = null);
    }
}
```

## Method Signature Specification

### CompleteActivity Method
```csharp
public virtual bool CompleteActivity(CompletionReason reason, IActivity newActivity = null)
```

**Parameters:**
- `reason`: Enum indicating why completion is triggered
- `newActivity`: Optional reference to the activity that will replace this one

**Return Value:**
- `true`: Activity handled completion and is ready to be stopped
- `false`: Activity doesn't support completion, use normal stop behavior

**Default Implementation:**
- Returns `false` by default (no completion logic)
- Existing activities continue working unchanged

## Integration Points

### 1. ActivityHandler Enhancement
```csharp
public class ActivityHandler
{
    // Existing methods remain unchanged for backward compatibility
    public bool Do(IActivity activity) { /* existing code */ }
    public bool StopCurrentActivity() { /* existing code */ }
    public void StopSafely(IActivity activity) { /* existing code */ }
    
    // New completion method
    public bool CompleteCurrentActivity(CompletionReason reason, IActivity newActivity = null)
    {
        if (!isActive || currentActivity == null) return false;
        
        // Try completion first
        bool completed = currentActivity.CompleteActivity(reason, newActivity);
        
        if (completed)
        {
            ActivityStory += $"\nCompleted {currentActivity.VerbName} due to {reason}";
            isActive = false;
            currentActivity = null;
            return true;
        }
        
        // Fall back to normal stop if completion not supported
        return StopCurrentActivity();
    }
    
    // Enhanced Do method with completion attempt
    public bool DoWithCompletion(IActivity activity, CompletionReason reason = CompletionReason.NewActivity)
    {
        if (isActive && currentActivity != null)
        {
            if (currentActivity.VerbName == activity.VerbName)
            {
                ActivityStory += "\nTried to start doing " + activity.VerbName + " but already doing it";
                return false;
            }
            
            // Try completing current activity first
            if (!CompleteCurrentActivity(reason, activity))
            {
                ActivityStory += $"\nCould not complete {currentActivity.VerbName} for {activity.VerbName}";
                return false;
            }
        }
        
        // Continue with normal Do logic
        return Do(activity);
    }
}
```

### 2. Body Class Integration Points
```csharp
public class Body : ThingWithHeight
{
    // Existing doableArms and doableLegs remain unchanged
    public Arms doableArms = new();
    public Legs doableLegs = new();
    
    // Optional convenience methods for completion
    public bool CompleteArmsActivity(CompletionReason reason, IActivity newActivity = null)
    {
        return doableArms.CompleteCurrentActivity(reason, newActivity);
    }
    
    public bool CompleteLegsActivity(CompletionReason reason, IActivity newActivity = null)
    {
        return doableLegs.CompleteCurrentActivity(reason, newActivity);
    }
}
```

### 3. Activity Implementation Integration
Activities can optionally override the completion method:

```csharp
public class DashAbility : MonoBehaviour, IActivity
{
    public string VerbName => "Dash";
    
    // Optional completion implementation
    public override bool CompleteActivity(CompletionReason reason, IActivity newActivity = null)
    {
        switch (reason)
        {
            case CompletionReason.AnimationInterrupt:
                // Handle animation interruption gracefully
                CancelInvoke(nameof(SafetyArmRelease));
                RefreshAimingSystem();
                return true;
                
            case CompletionReason.NewActivity:
                // Check if we can be interrupted by this specific activity
                if (newActivity?.VerbName == "Shooting")
                {
                    // Allow shooting to interrupt dash
                    return true;
                }
                return false; // Don't allow other activities to interrupt
                
            default:
                return false; // Use normal stop behavior
        }
    }
}
```

## Implementation Guidelines

### For Activity Developers
1. **Optional Implementation**: Only implement `CompleteActivity()` if your activity needs special completion logic
2. **Quick Completion**: Keep completion logic fast and simple
3. **State Cleanup**: Use completion to clean up state that normal stopping might miss
4. **Return Appropriately**: Return `true` only if completion was actually handled

### Common Completion Patterns
```csharp
public override bool CompleteActivity(CompletionReason reason, IActivity newActivity = null)
{
    switch (reason)
    {
        case CompletionReason.AnimationInterrupt:
            // Cancel pending animations/invokes
            // Clean up visual states
            // Return true if handled
            break;
            
        case CompletionReason.NewActivity:
            // Check compatibility with incoming activity
            // Allow or deny interruption based on current state
            break;
            
        case CompletionReason.PlayerStop:
            // Handle user-initiated stopping
            // May need different cleanup than system stops
            break;
    }
    
    return false; // Default: use normal stop behavior
}
```

## Backward Compatibility

### Guaranteed Compatibility
1. **Existing IActivity implementations**: Continue working unchanged
2. **Existing ActivityHandler usage**: All current methods remain functional
3. **Default behavior**: Activities without completion logic behave exactly as before
4. **No breaking changes**: No existing method signatures modified

### Migration Path
1. **Phase 1**: Add interface with default implementation
2. **Phase 2**: Update ActivityHandler with completion methods
3. **Phase 3**: Activities can optionally implement completion
4. **Phase 4**: Animation system can use completion for interruption

## Usage Examples

### Basic Animation Interruption
```csharp
// In animation system
if (body.doableArms.isActive)
{
    // Try completion first, fall back to normal stop
    body.doableArms.CompleteCurrentActivity(CompletionReason.AnimationInterrupt);
}
```

### Smart Activity Starting
```csharp
// In attack system - try completing current activity before starting
if (!body.doableArms.DoWithCompletion(shootingActivity, CompletionReason.NewActivity))
{
    // Could not start shooting (current activity refused to complete)
    return false;
}
```

### Activity-Specific Completion
```csharp
public class ReloadActivity : IActivity
{
    public string VerbName => "Reloading";
    
    public override bool CompleteActivity(CompletionReason reason, IActivity newActivity = null)
    {
        if (reason == CompletionReason.AnimationInterrupt && reloadProgress > 0.8f)
        {
            // If reload is almost done, let it complete
            return false;
        }
        
        if (newActivity?.VerbName == "Shooting")
        {
            // Allow shooting to interrupt reload
            CancelReload();
            return true;
        }
        
        return false;
    }
}
```

## File Locations

### Files to Create/Modify
1. **Interfaces.cs**: Add `CompletionReason` enum and extend `IActivity`
2. **ActivityHandler.cs**: Add completion methods
3. **Body.cs**: Add optional convenience methods

### Implementation Order
1. Modify `Interfaces.cs` with completion interface
2. Update `ActivityHandler.cs` with completion logic
3. Test with existing activities (should work unchanged)
4. Activities can gradually implement completion as needed
5. Animation system can start using completion for interruption

This specification ensures the completion system integrates seamlessly with the existing activity pattern while providing the flexibility needed for animation interruption without breaking any existing functionality.
