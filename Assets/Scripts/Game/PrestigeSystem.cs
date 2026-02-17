using System;
using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Data;
using UnityEngine;

namespace IdleFrisbeeGolf.Game
{
    /// <summary>
    /// Prestige system awarding spirit tokens based on total score.
    /// </summary>
    public class PrestigeSystem : MonoBehaviour
    {
        private GameState _state;
        private PrestigeConfig _config;
        private Action<double> _onPrestige;

        public void Initialize(GameState state, PrestigeConfig config, Action<double> onPrestige)
        {
            _state = state;
            _config = config;
            _onPrestige = onPrestige;
        }

        public double PreviewTokens()
        {
            return Math.Floor(Math.Pow(Math.Max(0, _state.totalScore) / _config.scoreDivider, _config.exponent));
        }

        public bool CanPrestige()
        {
            return PreviewTokens() > 0;
        }

        public void ExecutePrestige()
        {
            if (!CanPrestige())
            {
                return;
            }

            var tokens = PreviewTokens();
            _state.totalScore = 0;
            _state.currentScore = 0;
            _onPrestige?.Invoke(tokens);
            EventBus.Publish(new PrestigeMessage(tokens));
        }
    }

    public readonly struct PrestigeMessage
    {
        public readonly double Tokens;

        public PrestigeMessage(double tokens)
        {
            Tokens = tokens;
        }
    }
}
