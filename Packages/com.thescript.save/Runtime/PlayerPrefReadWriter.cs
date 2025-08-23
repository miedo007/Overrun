using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Save
{
    [PublicAPI]
    public class PlayerPrefReadWriter : ReadWriter
    {
        private readonly string _saveKey;

        public PlayerPrefReadWriter(string saveKey)
        {
            _saveKey = saveKey;
        }

        protected override void OnSave(string rawSave)
        {
            PlayerPrefs.SetString(_saveKey, rawSave);
        }

        protected override bool OnTryLoad(out string rawSave)
        {
            if (!PlayerPrefs.HasKey(_saveKey))
            {
                rawSave = string.Empty;
                return false;
            }
            
            rawSave = PlayerPrefs.GetString(_saveKey);
            return true;
        }

        protected override void OnClear()
        {
            PlayerPrefs.DeleteKey(_saveKey);
        }
    }
}

