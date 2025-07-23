using UnityEngine;

namespace __SCRIPTS
{
    public abstract class StateMachine<T> : MonoBehaviour where T : StateMachine<T>
    {
        private State<T> currentState;

        public void SwitchState(State<T> newState)
        {
            if (currentState != null)
            {
                currentState.Exit(this as T);
            }

            currentState = newState;

            if (currentState != null)
            {
                currentState.Enter(this as T);
            }
        }

        protected virtual void Update()
        {
            if (currentState != null)
            {
                currentState.Update(this as T);
            }
        }

        protected virtual void Start()
        {
        }
    }
}
