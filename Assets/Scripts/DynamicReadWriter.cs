using Mtl.Save;
using UnityEngine;

namespace Project.Application
{
    /// <summary>
    /// A ReadWriter that dynamically chooses between local file storage and Crazy Games Data
    /// based on the current login status. This allows the save system to seamlessly switch
    /// between storage backends without requiring SaveManager modifications.
    /// </summary>
    public class DynamicReadWriter : ReadWriter
    {
        private readonly string _saveKey;
        private FileReadWriter _fileReadWriter;
        private CrazyGamesDataReadWriter _crazyGamesReadWriter;

        public DynamicReadWriter(string saveKey)
        {
            _saveKey = saveKey;
            _fileReadWriter = new FileReadWriter(saveKey);
            _crazyGamesReadWriter = new CrazyGamesDataReadWriter(saveKey);
        }

        private ReadWriter GetCurrentReadWriter()
        {
            // Use CrazyGames storage (cloud OR localStorage) when SDK is ready
            // This works for both logged-in users (cloud) and guest users (localStorage)
            if (SaveSystemIntegration.IsCrazySDKReady())
            {
                return _crazyGamesReadWriter;
            }
            // Fallback to local file storage only when SDK is not ready (e.g., in Editor)
            return _fileReadWriter;
        }

        protected override void OnSave(string rawSave)
        {
            var currentWriter = GetCurrentReadWriter();
            var writerType = currentWriter is CrazyGamesDataReadWriter 
                ? (SaveSystemIntegration.IsUserLoggedIn ? "Crazy Games Cloud" : "Crazy Games localStorage") 
                : "local file";
            Debug.Log($"[DynamicReadWriter] ★ Saving {_saveKey} using {writerType}");
            Debug.Log($"[DynamicReadWriter] Data to save: {rawSave}");
            
            currentWriter.Save(rawSave);
            
            Debug.Log($"[DynamicReadWriter] ★ Save completed for {_saveKey}");
        }

        protected override bool OnTryLoad(out string rawSave)
        {
            var currentWriter = GetCurrentReadWriter();
            var writerType = currentWriter is CrazyGamesDataReadWriter 
                ? (SaveSystemIntegration.IsUserLoggedIn ? "Crazy Games Cloud" : "Crazy Games localStorage") 
                : "local file";
            Debug.Log($"Loading {_saveKey} using {writerType}");
            
            // If we're using cloud storage, try to load from there
            if (currentWriter is CrazyGamesDataReadWriter crazyWriter)
            {
                if (crazyWriter.TryLoad(out rawSave))
                {
                    Debug.Log($"Successfully loaded {_saveKey} from Crazy Games Data");
                    return true;
                }
                else
                {
                    Debug.Log($"No cloud data found for {_saveKey}, falling back to local file");
                    // Fall back to local file if no cloud data exists
                    return _fileReadWriter.TryLoad(out rawSave);
                }
            }
            
            return currentWriter.TryLoad(out rawSave);
        }

        protected override void OnClear()
        {
            var currentWriter = GetCurrentReadWriter();
            currentWriter.Clear();
        }
    }
}