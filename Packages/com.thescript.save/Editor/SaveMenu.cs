using System.IO;
using UnityEditor;
using UnityEngine;

namespace Mtl.Save.Editor
{
    public static class SaveMenu
    {
        [MenuItem("Mtl/Save/Show Save in Explorer")]
        public static void ShowSaveInExplorer()
        {
            EditorUtility.RevealInFinder($"{Application.persistentDataPath}/{FileReadWriter.Subfolder}");
        }

        [MenuItem("Mtl/Save/Clear All")]
        public static void ClearAllSaves()
        {
            ClearSaveOnDisk();
            ClearPlayerPrefs();
        }

        [MenuItem("Mtl/Save/Clear Saves on Disk")]
        public static void ClearSaveOnDisk()
        {
            SaveUtils.ClearSaveOnDisk();
        }

        [MenuItem("Mtl/Save/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log($"PlayerPrefs deleted");
        }
        
    }
}

