using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Save
{
    [PublicAPI]
    public class SaveUtils
    {
        public static void ClearSaveOnDisk()
        {
            var directory = $"{Application.persistentDataPath}/{FileReadWriter.Subfolder}";
            DeleteDirectory($"{Application.persistentDataPath}/{FileReadWriter.Subfolder}");
            if (!Directory.Exists(directory))
            {
                Debug.Log($"Save on disk deleted");
            }
        }
        
        private static void DeleteDirectory(string targetDir)
        {
            var files = Directory.GetFiles(targetDir);
            var dirs = Directory.GetDirectories(targetDir);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
                Debug.Log($"{file} deleted...");
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
            Debug.Log($"{targetDir} deleted");
        }
    }
}