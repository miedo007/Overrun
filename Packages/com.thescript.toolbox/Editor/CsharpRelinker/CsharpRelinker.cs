// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using UnityEditor;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// // Disabled for now
// // TODO Need to fix this to properly relink prefabs and not cause slowdowns
// namespace Mtl.Toolbox
// {
//     /// <summary>
//     /// This utility replaces the regular Refresh when focusing on Unity
//     /// with a custom version that fixes invalidated managed wrappers
//     /// on freshly imported assets 
//     /// </summary>
//     [InitializeOnLoad]
//     internal class CsharpRelinker : AssetPostprocessor
//     {
//         private static readonly FieldInfo CachedIntPtr;
//         private static readonly List<Object> LostFocusObjects;
//
//         private static bool _isEditorFocused;
//         private static bool _isImporting;
//
//         static CsharpRelinker()
//         {
//             CachedIntPtr = typeof(Object).GetField("m_CachedPtr", BindingFlags.Instance | BindingFlags.NonPublic);
//             LostFocusObjects = new List<Object>();
//             _isEditorFocused = true;
//
//             EditorApplication.update += EditorUpdate;
//         }
//
//         private void OnPreprocessAsset()
//         {
//             if (_isImporting)
//             {
//                 return;
//             }
//
//             _isImporting = true;
//             CollectObjects();
//         }
//
//         private static void EditorUpdate()
//         {
//             if (_isEditorFocused && !UnityEditorInternal.InternalEditorUtility.isApplicationActive)
//             {
//                 _isEditorFocused = false;
//
//                 if (!_isImporting)
//                 {
//                     LostFocusObjects.Clear();
//                     CollectObjects();
//                 }
//             }
//             else if (!_isEditorFocused && UnityEditorInternal.InternalEditorUtility.isApplicationActive)
//             {
//                 _isEditorFocused = true;
//             }
//         }
//
//         private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
//         {
//             // Relink invalid wrappers 
//             foreach (var obj in LostFocusObjects)
//             {
//                 RelinkCsharpWrapper(obj);
//             }
//
//             LostFocusObjects.Clear();
//             _isImporting = false;
//         }
//
//         private static void CollectObjects()
//         {
//             foreach (var o in Resources.FindObjectsOfTypeAll<Object>())
//             {
//                 LostFocusObjects.Add(o);
//             }
//         }
//
//         private static void RelinkCsharpWrapper(Object wrapper)
//         {
//             // UnityEngine.Object overrides the Equals operator so we need to check for broken wrappers
//             if (wrapper != null)
//             {
//                 // Wrapper is still valid
//                 return;
//             }
//
//             var obj = (object) wrapper;
//             if (obj == null)
//             {
//                 // Not a broken wrapper since the managed object is also invalid
//                 return;
//             }
//
//             var instanceID = wrapper.GetInstanceID();
//             if (!AssetDatabase.Contains(instanceID))
//             {
//                 // Unknown object
//                 return;
//             }
//
//             // Fetch a new wrapper using the instanceID
//             var path = AssetDatabase.GetAssetPath(instanceID);
//             var newWrapper = AssetDatabase.LoadAllAssetsAtPath(path).FirstOrDefault(o => o != null && o.GetInstanceID() == instanceID);
//             if (newWrapper != null)
//             {
//                 // Update the int pointer to a new object value
//                 CachedIntPtr.SetValue(wrapper, CachedIntPtr.GetValue(newWrapper));
//             }
//         }
//     }
// }