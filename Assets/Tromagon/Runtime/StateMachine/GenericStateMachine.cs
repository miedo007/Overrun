using System;
using System.Collections.Generic;
using JetBrains.Annotations;
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable LocalVariableHidesMember

namespace Tromagon.StateMachine
{
    [PublicAPI]
    public class GenericStateMachine
    {
        private IState currentState;
        private IUpdateState updateState;
        private ILateUpdateState lateUpdateState;

        private readonly Dictionary<Type, IState> cache = new Dictionary<Type, IState>();

        public void ChangeState<T>() where T : IState
        {
            if (currentState is IExitState exitState)
            {
                exitState.OnExit();
            }

            this.updateState = null;
            this.lateUpdateState = null;

            var type = typeof(T);
            if (!cache.TryGetValue(type, out var state))
            {
                state = Activator.CreateInstance<T>();
                cache.Add(type, state);
            }

            currentState = state;
            if (currentState is IUpdateState updateState)
            {
                this.updateState = updateState;
            }

            if (currentState is ILateUpdateState lateUpdateState)
            {
                this.lateUpdateState = lateUpdateState;
            }

            if (currentState is IEnterState enterState)
            {
                enterState.OnEnter();
            }
        }

        public void Update()
        {
            updateState?.OnUpdate();
        }

        public void LateUpdate()
        {
            lateUpdateState?.OnLateUpdate();
        }
    }

    public interface IState
    {
    }

    public interface IEnterState
    {
        void OnEnter();
    }

    public interface IExitState
    {
        void OnExit();
    }

    public interface IUpdateState
    {
        void OnUpdate();
    }

    public interface ILateUpdateState
    {
        void OnLateUpdate();
    }
}