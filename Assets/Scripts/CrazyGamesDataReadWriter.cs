using Mtl.Save;
using UnityEngine;
using CrazyGames;

namespace Project.Application
{
    public class CrazyGamesDataReadWriter : ReadWriter
    {
        private readonly string _saveKey;

        public CrazyGamesDataReadWriter(string saveKey)
        {
            _saveKey = saveKey;
        }

        protected override void OnSave(string rawSave)
        {
            if (IsCrazyGamesDataAvailable())
            {
                CrazySDK.Data.SetString(_saveKey, rawSave);
            }
            else
            {
                // Fallback to PlayerPrefs if CrazySDK not available
                PlayerPrefs.SetString(_saveKey, rawSave);
            }
        }

        protected override bool OnTryLoad(out string rawSave)
        {
            if (IsCrazyGamesDataAvailable())
            {
                if (!CrazySDK.Data.HasKey(_saveKey))
                {
                    rawSave = string.Empty;
                    return false;
                }
                
                rawSave = CrazySDK.Data.GetString(_saveKey);
                return true;
            }
            else
            {
                // Fallback to PlayerPrefs if CrazySDK not available
                if (!PlayerPrefs.HasKey(_saveKey))
                {
                    rawSave = string.Empty;
                    return false;
                }
                
                rawSave = PlayerPrefs.GetString(_saveKey);
                return true;
            }
        }

        protected override void OnClear()
        {
            if (IsCrazyGamesDataAvailable())
            {
                CrazySDK.Data.DeleteKey(_saveKey);
            }
            else
            {
                // Fallback to PlayerPrefs if CrazySDK not available
                PlayerPrefs.DeleteKey(_saveKey);
            }
        }

        private bool IsCrazyGamesDataAvailable()
        {
            try
            {
                return CrazySDK.IsInitialized && CrazySDK.IsAvailable;
            }
            catch
            {
                return false;
            }
        }
    }
}