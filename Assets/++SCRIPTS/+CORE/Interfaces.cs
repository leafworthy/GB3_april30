namespace GangstaBean.Core
{
    /// <summary>
    /// Interface for components that need a player reference
    /// </summary>
    public interface INeedPlayer
    {
        void SetPlayer(__SCRIPTS.Player _player);
    }

    /// <summary>
    /// Interface for components that can be activities
    /// </summary>
    public interface IActivity
    {
        public string VerbName { get; }
        
        /// <summary>
        /// Called when activity should complete gracefully before being stopped.
        /// Default implementation returns false to use standard cleanup.
        /// Override only if custom completion logic is needed.
        /// </summary>
        /// <param name="reason">Why the completion is being triggered</param>
        /// <param name="newActivity">The activity that will replace this one (can be null)</param>
        /// <returns>True if completion was handled, false to use default cleanup behavior</returns>
        bool TryCompleteGracefully(CompletionReason reason, IActivity newActivity = null) => false;
    }

    /// <summary>
    /// Interface for activities that support completion behavior
    /// NOTE: This interface is deprecated. Use IActivity.TryCompleteGracefully() instead.
    /// </summary>
    [System.Obsolete("Use IActivity.TryCompleteGracefully() instead")]
    public interface ICompletableActivity : IActivity
    {
        /// <summary>
        /// Called when activity should complete gracefully before being stopped.
        /// </summary>
        /// <param name="reason">Why the completion is being triggered</param>
        /// <param name="newActivity">The activity that will replace this one (can be null)</param>
        /// <returns>True if completion was handled, false to use normal stopping behavior</returns>
        bool CompleteActivity(CompletionReason reason, IActivity newActivity = null);
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

    /// <summary>
    /// Interface for components that need to reset their state when reused from object pool
    /// </summary>
    public interface IPoolable
    {
        void OnPoolSpawn();
        void OnPoolDespawn();
    }
}
