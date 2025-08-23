using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Save
{
    [PublicAPI]
    public class FileReadWriter : ReadWriter
    {
        private string _fileDestination;
        private readonly string _filename;

        public static string Subfolder = "Save/";

        public FileReadWriter(string filename)
        {
            _filename = filename;
        }
        
        protected override void OnSave(string rawSave)
        {
            ValidateFileDestination();
            var file = new FileInfo(_fileDestination);
            if (file.Directory != null) file.Directory.Create();
            File.WriteAllText(_fileDestination, rawSave);
        }

        protected override bool OnTryLoad(out string rawSave)
        {
            ValidateFileDestination();
            
            if (!File.Exists(_fileDestination))
            {
                rawSave = string.Empty;
                return false;
            }

            rawSave = File.ReadAllText(_fileDestination);
            return true;
        }

        protected override void OnClear()
        {
            ValidateFileDestination();
            
            if (File.Exists(_fileDestination))
            {
                File.Delete(_fileDestination);
            }
        }

        private void ValidateFileDestination()
        {
            if (string.IsNullOrEmpty(_fileDestination))
            {
                _fileDestination = $"{Application.persistentDataPath}/{Subfolder}{_filename}.sav";
            }
        }
    }
}

