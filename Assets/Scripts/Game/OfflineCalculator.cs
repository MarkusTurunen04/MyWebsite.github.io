using System;
using IdleFrisbeeGolf.Data;
using UnityEngine;

namespace IdleFrisbeeGolf.Game
{
    /// <summary>
    /// Calculates offline earnings capped to 8 hours.
    /// </summary>
    public class OfflineCalculator : MonoBehaviour
    {
        private GameState _state;
        private const double MaxOfflineHours = 8;

        public void Initialize(GameState state)
        {
            _state = state;
            Refresh();
        }

        public void Refresh()
        {
            _state.lastActiveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public double CalculateOfflineRewards(EconomyConfig economyConfig)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var elapsedSeconds = Math.Max(0, now - _state.lastActiveTimestamp);
            var clamped = Math.Min(elapsedSeconds, MaxOfflineHours * 3600);
            var multiplier = 1 + _state.autoThrowers * 0.05f;
            var rewardPerSecond = economyConfig.baseScorePerThrow / economyConfig.autoThrowBaseInterval;
            var total = rewardPerSecond * clamped * multiplier;
            _state.lastActiveTimestamp = now;
            return total;
        }
    }
}
