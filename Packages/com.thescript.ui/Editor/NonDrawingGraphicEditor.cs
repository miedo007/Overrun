// @creator: Slipp Douglas Thompson
// @license: Public Domain per The Unlicense.  See <http://unlicense.org/>.
// @purpose: Slimmed-down Inspector UI for `NonDrawingGraphic` class.
// @why: Because this functionality should be built-into Unity.
// @usage: Add a `NonDrawingGraphic` component to the GameObject you want clickable, but without its own image/graphics.
// @intended project path: Assets/Plugins/Editor/UnityEngine UI Extensions/NonDrawingGraphicEditor.cs
// @interwebsouce: https://gist.github.com/capnslipp/349c18283f2fea316369

using System;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using Vengadores.UIFramework.Utils;

namespace Vengadores.UIFramework.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
    public class NonDrawingGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI ()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Script, Array.Empty<GUILayoutOption>());
            // skipping AppearanceControlsGUI
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}