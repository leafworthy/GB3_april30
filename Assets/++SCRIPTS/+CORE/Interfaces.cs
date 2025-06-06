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
