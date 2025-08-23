using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mtl.Toolbox
{
    [Serializable]
    public struct MinMaxInt
    {
        public int Min;
        public int Max;

        public bool InclusiveMax;

        public readonly float Clamp(int value) => Mathf.Clamp(value, Min, Max);

        public readonly int GetValue() => Random.Range(Min, InclusiveMax ? Max + 1 : Max);

        public static implicit operator int(MinMaxInt minMaxInt) => minMaxInt.GetValue();
    }
}