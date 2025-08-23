using UnityEngine;

namespace Mtl.Toolbox
{
    /// <summary>
    /// Handles the generation and maintenance of a UID on the object
    /// </summary>
    public abstract class UniqueScriptableObject : ScriptableObject
    {
        /// <summary>
        /// The Unique Identifier of this ScriptableObjects
        /// </summary>
        [field: SerializeField]
#if ODIN_INSPECTOR
        [field: Sirenix.OdinInspector.ReadOnly]
        // TODO Add a fallback [PHILL]
#endif
        public string Uid { get; private set; }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (!string.IsNullOrEmpty(Uid))
            {
                return;
            }

            Uid = UidGenerator.GetUid();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}