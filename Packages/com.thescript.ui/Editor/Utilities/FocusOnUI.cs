using UnityEditor;
using UnityEngine;

namespace Mtl.UI
{
    internal static class FocusOnUI
    {
        [MenuItem("GameObject/Isolate and Align %_U", true)]
        private static bool ValidateFocus()
        {
            var selected = Selection.activeObject as GameObject;
            return selected != null && selected.scene.IsValid();
        }

        [MenuItem("GameObject/Isolate and Align %_U")]
        private static void Focus()
        {
            var selected = Selection.activeObject as GameObject;
            SceneVisibilityManager.instance.Isolate(selected, true);
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.orthographic = true;
            // ReSharper disable once PossibleNullReferenceException
            sceneView.AlignViewToObject(selected.transform);
            sceneView.FrameSelected();
        }
    }
}