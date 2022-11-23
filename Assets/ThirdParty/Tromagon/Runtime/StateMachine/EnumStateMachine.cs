using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tromagon.StateMachine
{
    [PublicAPI]
    public class EnumStateMachine
    {
        private Enum currentState;
        private EnumStateInfo currentStateInfo;
        private readonly Dictionary<Enum, EnumStateInfo> states;

        public EnumStateMachine(Dictionary<Enum, EnumStateInfo> states)
        {
            this.states = states;
        }

        public void ChangeState(Enum state)
        {
            if (!states.TryGetValue(state, out var stateInfo))
            {
                throw new Exception("State is not registered");
            }

            if (currentState != null)
            {
                states[currentState].ExitAction?.Invoke();
            }

            currentState = state;
            currentStateInfo = stateInfo;
            currentStateInfo.EnterAction?.Invoke();
        }

        public bool IsCurrentState(Enum stateId)
        {
            return currentState.Equals(stateId);
        }

        public void Update()
        {
            currentStateInfo.UpdateAction?.Invoke();
        }

        public void LateUpdate()
        {
            currentStateInfo.LateUpdateAction?.Invoke();
        }
    }

    [PublicAPI]
    public class EnumStateInfo
    {
        public Action EnterAction;
        public Action UpdateAction;
        public Action LateUpdateAction;
        public Action ExitAction;
    }
}