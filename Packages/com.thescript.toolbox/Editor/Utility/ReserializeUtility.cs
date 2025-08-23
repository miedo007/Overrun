using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Mtl.Toolbox
{
    public static class ReserializeUtility
    {
        [MenuItem("Mtl/Reserialize/All", false, -1000)]
        private static void ReserializeAll()
        {
            var folders = new[] { "Assets" };

            using var guids = ListPool.Get<string>();
            guids.AddRange(AssetDatabase.FindAssets("t:Prefab", folders));
            guids.AddRange(AssetDatabase.FindAssets("t:ScriptableObject", folders));

            Reserialize(guids);
        }

        [MenuItem("Mtl/Reserialize/Selected Assets", false, -999)]
        private static void ReserializeSelected()
        {
            Reserialize(Selection.assetGUIDs);
        }

        private static void Reserialize(IReadOnlyList<string> guids)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                float allCount = guids.Count;
                var progress = 0;

                foreach (var assetGuid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(assetGuid);
                    IncrementProgress(path);
                    var asset = AssetDatabase.LoadMainAssetAtPath(path);
                    if (asset == null)
                    {
                        continue;
                    }

                    EditorUtility.SetDirty(asset);
                }

                void IncrementProgress(string assetPath)
                {
                    EditorUtility.DisplayProgressBar("Reserialize",
                        $"Reserializing: {Path.GetFileNameWithoutExtension(assetPath)}",
                        ++progress / allCount);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();

                EditorUtility.UnloadUnusedAssetsImmediate();
            }
        }
    }
}