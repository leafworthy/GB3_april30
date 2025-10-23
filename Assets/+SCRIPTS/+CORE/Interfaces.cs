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
    public interface IDoableAbility
    {
        string AbilityName { get; }
       bool canDo();
        bool canStop(IDoableAbility abilityToStopFor);
        void Do();
        void Stop();
        void Resume();
    }


    public interface IPoolable
    {
        void OnPoolSpawn();
        void OnPoolDespawn();
    }
}
