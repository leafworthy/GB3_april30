namespace __SCRIPTS
{
    public abstract class State<T> where T : StateMachine<T>
    {
        public abstract void Enter(T machine);
        public abstract void Update(T machine);
        public abstract void Exit(T machine);

        protected void SwitchState(T machine, State<T> newState)
        {
            machine.SwitchState(newState);
        }
    }
}
