using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mtl.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("Mtl/UI/Flip Graphic")]
    public class UIFlip : UIBehaviour, IMeshModifier
    {
        [SerializeField]
        private bool _flipX;

        [SerializeField]
        private bool _flipY;

        private Graphic _graphic;

        protected override void Awake()
        {
            _graphic = GetComponent<Graphic>();
        }

        public void ModifyMesh(Mesh mesh)
        {
            // Use ModifyMesh(VertexHelper) instead
        }

        public void ModifyMesh(VertexHelper verts)
        {
            var rect = ((RectTransform)transform).rect;

            var vert = new UIVertex();
            for (int i = 0, iMax = verts.currentVertCount; i < iMax; ++i)
            {
                verts.PopulateUIVertex(ref vert, i);
                var normalizedPos = (Vector2)vert.position - rect.min;
                normalizedPos.x /= rect.width;
                normalizedPos.y /= rect.height;

                if (_flipX)
                {
                    normalizedPos.x = 1 - normalizedPos.x;
                }

                if (_flipY)
                {
                    normalizedPos.y = 1 - normalizedPos.y;
                }

                vert.position.x = normalizedPos.x * rect.width + rect.xMin;
                vert.position.y = normalizedPos.y * rect.height + rect.yMin;
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