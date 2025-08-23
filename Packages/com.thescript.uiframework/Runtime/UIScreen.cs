using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.UiFramework
{
    [PublicAPI]
    public abstract class UIScreen : MonoBehaviour
    {
        public event Action<UIScreen> OnOpenEvent;
        public event Action<UIScreen> OnCloseEvent;

        public bool IsOpened { get; private set; }

        internal void Create()
        {
            OnCreated();
        }

        internal void Open()
        {
            if (gameObject.activeSelf)
            {
                Debug.LogWarning($"UIScreen {GetType()} already opened");
                return;
            }

            gameObject.SetActive(true);
            IsOpened = true;
            OnOpenEvent?.Invoke(this);
            OnOpened();
        }

        public void Close()
        {
            if (!gameObject.activeSelf)
            {
                Debug.LogWarning($"UIScreen {GetType()} already closed");
                return;
            }

            gameObject.SetActive(false);
            IsOpened = false;
            OnCloseEvent?.Invoke(this);
            OnClosed();
        }

        protected virtual void OnCreated() { }
        protected virtual void OnOpened() { }
        protected virtual void OnClosed() { }
    }

    [PublicAPI]
    public abstract class UIScreen<T> : UIScreen
    {
        protected T Parameters { get; private set; }

        internal void Open(T parameters)
        {
            Parameters = parameters;
            Open();
        }
    }
}