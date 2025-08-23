using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Mtl.Save
{
    [PublicAPI]
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private bool autoSave;
        [SerializeField] private int autoSaveTime = 5;

        private readonly List<SaveInfo> _saves = new List<SaveInfo>();
        private float _nextSaveTime;
        private readonly FileReadWriter _defaultWriter = new FileReadWriter("default");
        
        public void TryLoad(ISavable newSavable, Action<bool> onComplete, ReadWriter readWriter = null)
        {
            readWriter ??= _defaultWriter;
            var saveInfo = new SaveInfo(newSavable, readWriter);
            _saves.Add(saveInfo);

            newSavable.OnChanged += () =>
            {
                saveInfo.IsDirty = true;
            };

            var success = false;
            if (readWriter.TryLoad(out var rawSave))
            {
                success = true;
                JsonConvert.PopulateObject(rawSave, newSavable.Save, new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                });
            }
            
            onComplete?.Invoke(success);
        }

        public void ClearAll()
        {
            foreach (var saveInfo in _saves)
            {
                saveInfo.ReadWriter.Clear();
            }
        }
        
        public void Save()
        {
            _nextSaveTime = autoSaveTime;
            foreach (var saveInfo in _saves)
            {
                if (!saveInfo.IsDirty) continue;
                var rawSave = JsonConvert.SerializeObject(saveInfo.Savable.Save);
                saveInfo.ReadWriter.Save(rawSave);
                saveInfo.IsDirty = false;
            }
        }

        private void LateUpdate()
        {
            if (!autoSave) return;
            
            if (_nextSaveTime > 0)
            {
                _nextSaveTime -= Time.unscaledDeltaTime;
                return;
            }
            
            Save();
        }

        private class SaveInfo
        {
            public readonly ISavable Savable;
            public readonly ReadWriter ReadWriter;

            public bool IsDirty { get; set; }

            public SaveInfo(ISavable savable, ReadWriter readWriter)
            {
                Savable = savable;
                ReadWriter = readWriter;
            }
        }
    }
}
