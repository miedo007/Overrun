using JetBrains.Annotations;

namespace Mtl.Save
{
    [PublicAPI]
    public abstract class ReadWriter
    {
        public void Save(string rawSave)
        {
            OnSave(rawSave);
        }

        public bool TryLoad(out string rawSave)
        {
            return OnTryLoad(out rawSave);
        }

        public void Clear()
        {
            OnClear();
        }

        protected abstract void OnSave(string rawSave);
        protected abstract bool OnTryLoad(out string rawSave);
        protected abstract void OnClear();
    }
}


