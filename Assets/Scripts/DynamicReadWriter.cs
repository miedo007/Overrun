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
            if (SaveSystemIntegration.IsUserLoggedIn && SaveSystemIntegration.IsCrazySDKReady())
            {
                return _crazyGamesReadWriter;
            }
            return _fileReadWriter;
        }

        protected override void OnSave(string rawSave)
        {
            var currentWriter = GetCurrentReadWriter();
            var writerType = currentWriter is CrazyGamesDataReadWriter ? "Crazy Games Data" : "local file";
            Debug.Log($"Saving {_saveKey} using {writerType}");
            
            currentWriter.Save(rawSave);
        }

        protected override bool OnTryLoad(out string rawSave)
        {
            var currentWriter = GetCurrentReadWriter();
            var writerType = currentWriter is CrazyGamesDataReadWriter ? "Crazy Games Data" : "local file";
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