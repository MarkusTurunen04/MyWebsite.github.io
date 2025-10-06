using System;
using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Data;
using UnityEngine;

namespace IdleFrisbeeGolf.Game
{
    /// <summary>
    /// Handles throw resolution and probability driven outcomes.
    /// </summary>
    public class ThrowSystem : MonoBehaviour
    {
        private GameState _state;
        private EconomyConfig _economyConfig;
        private AnalyticsService _analytics;
        private double _autoThrowTimer;
        private System.Random _random;
        private double _autoThrowInterval;

        public double PointsPerSecond { get; private set; }
        public double AutoThrowers => _state?.autoThrowers ?? 0;

        public void Initialize(GameState state, EconomyConfig economyConfig, AnalyticsService analytics)
        {
            _state = state;
            _economyConfig = economyConfig;
            _analytics = analytics;
            _random ??= new System.Random();
            _autoThrowInterval = _economyConfig.autoThrowBaseInterval;
        }

        public double Tick(float deltaTime, double multiplier)
        {
            if (_state.autoThrowers <= 0)
            {
                PointsPerSecond = 0;
                return 0;
            }

            _autoThrowTimer += deltaTime * _state.autoThrowers;

            var total = 0d;
            while (_autoThrowTimer >= _autoThrowInterval)
            {
                _autoThrowTimer -= _autoThrowInterval;
                total += ResolveThrow(multiplier, false);
            }

            if (deltaTime > 0f)
            {
                PointsPerSecond = total / deltaTime;
            }

            return total;
        }

        public double ManualThrow(double multiplier)
        {
            var score = ResolveThrow(multiplier, true);
            _state.manualThrows++;
            return score;
        }

        private double ResolveThrow(double multiplier, bool manual)
        {
            var roll = _random.NextDouble();
            double baseScore;
            if (roll <= 0.01f)
            {
                baseScore = _economyConfig.baseScorePerThrow * 10;
                _analytics.LogThrow("ace", manual);
            }
            else if (roll <= 0.21f)
            {
                baseScore = _economyConfig.baseScorePerThrow * 3;
                _analytics.LogThrow("birdie", manual);
            }
            else
            {
                baseScore = _economyConfig.baseScorePerThrow;
                _analytics.LogThrow("par", manual);
            }

            var accuracyMultiplier = _state.accuracyMultiplier;
            var powerMultiplier = _state.powerMultiplier;
            var autoMultiplier = manual ? 1 : 1 + (_state.autoThrowers * 0.05f);
            var total = baseScore * accuracyMultiplier * powerMultiplier * multiplier * autoMultiplier;
            return total;
        }

        public void SetAccuracy(double value)
        {
            _state.accuracyMultiplier = value;
        }

        public void SetPower(double value)
        {
            _state.powerMultiplier = value;
        }

        public void SetAutoThrowLevel(int level)
        {
            _state.autoThrowers = level;
            _autoThrowInterval = Mathf.Max((float)(_economyConfig.autoThrowBaseInterval / (1 + level * 0.1f)), (float)_economyConfig.autoThrowMinInterval);
        }

        public void ResetProgress()
        {
            _state.manualThrows = 0;
            _state.autoThrowers = 0;
            _autoThrowTimer = 0;
            _autoThrowInterval = _economyConfig.autoThrowBaseInterval;
        }

        public void SetRandomGenerator(System.Random random)
        {
            _random = random;
        }
    }
}
