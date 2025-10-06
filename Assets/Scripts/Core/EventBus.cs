using System;
using System.Collections.Generic;

namespace IdleFrisbeeGolf.Core
{
    /// <summary>
    /// Simple event bus allowing decoupled broadcast of gameplay events.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _listeners = new();

        public static void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var delegates))
            {
                delegates = new List<Delegate>();
                _listeners[type] = delegates;
            }

            if (!delegates.Contains(callback))
            {
                delegates.Add(callback);
            }
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (_listeners.TryGetValue(type, out var delegates))
            {
                delegates.Remove(callback);
            }
        }

        public static void Publish<T>(T message)
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var delegates))
            {
                return;
            }

            for (var i = delegates.Count - 1; i >= 0; i--)
            {
                if (delegates[i] is Action<T> action)
                {
                    action.Invoke(message);
                }
            }
        }

        public static void Clear()
        {
            _listeners.Clear();
        }
    }
}
