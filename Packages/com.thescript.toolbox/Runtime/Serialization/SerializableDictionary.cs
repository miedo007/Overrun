// This is based on the recommendations in the documentation of Odin
// https://odininspector.com/tutorials/serialize-anything/serializing-dictionaries
// See "Serializing Dictionaries With Unity"

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mtl.Toolbox
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [HideInInspector, SerializeField]
        private List<TKey> _keys = new List<TKey>();

        [HideInInspector, SerializeField]
        private List<TValue> _values = new List<TValue>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var (key, value) in this)
            {
                _keys.Add(key);
                _values.Add(value);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            for (var i = 0; i < _keys.Count && i < _values.Count; ++i)
            {
                this[_keys[i]] = _values[i];
            }
        }
    }
}