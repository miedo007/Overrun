// Copyright (C) 2015-2021 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltimateClean
{
    /// <summary>
    /// Manages a queue of notifications across scenes (DontDestroyOnLoad).
    /// </summary>
    public class NotificationQueue : MonoBehaviour
    {
        public static NotificationQueue Instance { get; private set; }

        private readonly Queue<QueuedNotification> pendingNotifications = new Queue<QueuedNotification>(8);
        private bool notificationActive;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                pendingNotifications.Clear();
                notificationActive = false;
            };
        }

        public void EnqueueNotification(
            GameObject prefab,
            Canvas canvas,
            NotificationType type,
            NotificationPositionType position,
            float duration,
            string title,
            string message)
        {
            var notification = new QueuedNotification
            {
                Prefab = prefab,
                Canvas = canvas,
                Type = type,
                Position = position,
                Duration = duration,
                Title = title,
                Message = message
            };
            pendingNotifications.Enqueue(notification);
        }

        private void Update()
        {
            if (notificationActive || pendingNotifications.Count == 0)
                return;

            var info = pendingNotifications.Dequeue();

            var go = Instantiate(info.Prefab);
            go.transform.SetParent(info.Canvas.transform, false);

            var n = go.GetComponent<Notification>();
            n.Launch(info.Type, info.Position, info.Duration, info.Title, info.Message);

            notificationActive = true;
            n.OnCompleted += () => { notificationActive = false; };
        }
    }
}
