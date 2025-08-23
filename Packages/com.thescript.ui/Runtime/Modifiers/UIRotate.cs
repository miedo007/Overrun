using JetBrains.Annotations;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mtl.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("Mtl/UI/Rotate Graphic")]
    public class UIRotate : UIBehaviour, IMeshModifier
    {
        [PublicAPI]
        private enum Rotation
        {
#if ODIN_INSPECTOR
            [LabelText("0º")]
#endif
            [InspectorName("0º")]
            Zero = 0,

#if ODIN_INSPECTOR
            [LabelText("90º")]
#endif
            [InspectorName("90º")]
            Ninety = 90,

#if ODIN_INSPECTOR
            [LabelText("180º")]
#endif
            [InspectorName("180º")]
            OneEighty = 180,

#if ODIN_INSPECTOR
            [LabelText("270º")]
#endif
            [InspectorName("270º")]
            TwoSeventy = 270
        }

        [SerializeField]
        private Rotation _rotation = Rotation.Zero;

        [SerializeField]
        [Tooltip("Use the pivot if true, the center otherwise")]
        private bool _usePivot = true;

        private Graphic _graphic;

        protected override void Awake()
        {
            _graphic = GetComponent<Graphic>();
        }

        public void ModifyMesh(Mesh mesh)
        {
            // Unused becuase ModifyMesh(VertexHelper) is prefered
        }

        public void ModifyMesh(VertexHelper verts)
        {
            var rotation = Quaternion.Euler(0, 0, -(int)_rotation);

            var rectTr = (RectTransform)transform;
            var rect = rectTr.rect;
            var pivot = rectTr.pivot;
            pivot = _usePivot
                ? new Vector2(rect.size.x * pivot.x, rect.size.y * pivot.y) + rect.min
                : rect.center;

            var vert = new UIVertex();
            for (int i = 0, iMax = verts.currentVertCount; i < iMax; ++i)
            {
                verts.PopulateUIVertex(ref vert, i);

                var normalizedPos = (Vector2)vert.position - pivot;
                normalizedPos.x /= rect.width;
                normalizedPos.y /= rect.height;
                normalizedPos = rotation * normalizedPos;

                vert.position.x = normalizedPos.x * rect.width + pivot.x;
                vert.position.y = normalizedPos.y * rect.height + pivot.y;

                verts.SetUIVertex(vert, i);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (_graphic == null)
            {
                Awake();
            }

            _graphic.SetVerticesDirty();
        }
#endif
    }
}