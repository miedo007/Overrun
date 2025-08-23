using System;

namespace Mtl.Save
{
    public interface ISavable
    {
        event Action OnChanged;

        Save Save { get; }
    }
}
