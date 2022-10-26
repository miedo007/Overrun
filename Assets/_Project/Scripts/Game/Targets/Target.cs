using System;
using UnityEngine;

namespace Project.Game.Targets
{
    public class Target : MonoBehaviour
    {
        public static event Action<Target> StateChanged;

        private bool _enabled = false;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                {
                    return;
                }

                _enabled = value;
                StateChanged?.Invoke(this);
            }
        }
    }
}