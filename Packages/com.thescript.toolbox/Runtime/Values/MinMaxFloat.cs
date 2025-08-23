using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mtl.Toolbox
{
    [Serializable]
    public struct MinMaxFloat
    {
        [Tooltip("Min is inclusive")]
        public float Min;

        [Tooltip("Max is inclusive")]
        public float Max;

        public readonly float Clamp(float value) => Mathf.Clamp(value, Min, Max);

        public readonly float GetValue() => Random.Range(Min, Max);

        public static implicit operator float(MinMaxFloat minMaxFloat) => minMaxFloat.GetValue();
    }
}