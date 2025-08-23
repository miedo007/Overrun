using System;
using System.IO;
using System.Reflection;
using UnityEditor;

namespace Mtl.Toolbox
{
    public class UniqueScriptableObjectPostProcessor : AssetPostprocessor
    {
        private static readonly FieldInfo BackingField;
        private static readonly MethodInfo ValidateMethod;

        static UniqueScriptableObjectPostProcessor()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            BackingField = typeof(UniqueScriptableObject).GetField($"<{nameof(UniqueScriptableObject.Uid)}>k__BackingField", flags);
            ValidateMethod = typeof(UniqueScriptableObject).GetMethod("OnValidate", flags);
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                ProcessAsset(assetPath);
            }

            foreach (var assetPath in movedAssets)
            {
                ProcessAsset(assetPath);
            }
        }

        private static void ProcessAsset(string assetPath)
        {
            if (Path.GetExtension(assetPath) != ".asset")
            {
                return;
            }
            
            var importer = AssetImporter.GetAtPath(assetPath);
            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            
            // TODO Find an alternative to userData [PHILL]
            // userData could be overridden by any other importer so it's not safe
            if (importer.userData == assetGuid)
            {
                return;
            }

            var uniqueScriptableObject = AssetDatabase.LoadAssetAtPath<UniqueScriptableObject>(assetPath);
            if (uniqueScriptableObject == null)
            {
                return;
            }

            importer.userData = assetGuid;
            importer.SaveAndReimport();
            BackingField.SetValue(uniqueScriptableObject, null);
            ValidateMethod.Invoke(uniqueScriptableObject, Array.Empty<object>());
        }
    }
}