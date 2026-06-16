using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public enum LogChannel
    {
        Damage,
        Spawning,
        Pooling,
        Spells,
        Session
    }

    public static class GameLog
    {
        private static readonly HashSet<LogChannel> EnabledChannels = new HashSet<LogChannel>
        {
            LogChannel.Damage,
            LogChannel.Spawning,
            LogChannel.Pooling,
            LogChannel.Spells,
            LogChannel.Session
        };

        [Conditional("UNITY_EDITOR")]
        public static void Info(LogChannel channel, string message, Object context = null)
        {
            if (!EnabledChannels.Contains(channel))
            {
                return;
            }

            UnityEngine.Debug.Log($"[{channel}] {message}", context);
        }
    }
}
