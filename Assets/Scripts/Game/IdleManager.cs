using System;
using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Data;
using UnityEngine;

namespace IdleFrisbeeGolf.Game
{
    /// <summary>
    /// Central runtime orchestrator keeping score, offline progress and analytics in sync.
    /// </summary>
    public class IdleManager : MonoBehaviour
    {
        [SerializeField] private EconomyConfig economyConfig;
        [SerializeField] private PrestigeConfig prestigeConfig;
        [SerializeField] private ThrowSystem throwSystem;
        [SerializeField] private UpgradeSystem upgradeSystem;
        [SerializeField] private PrestigeSystem prestigeSystem;
        [SerializeField] private OfflineCalculator offlineCalculator;
        [SerializeField] private AnalyticsService analyticsService;

        private SaveSystem _saveSystem;
        private GameState _state;
        private double _spiritBonusMultiplier = 1;
        private double _pendingOfflineReward;

        public GameState State => _state;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;

            _saveSystem = new SaveSystem("idle_save.json", SystemInfo.deviceUniqueIdentifier);
            Load();
            InitializeSystems();
            analyticsService.LogSessionStart();
            EventBus.Publish(new SessionStartedMessage());
        }

        private void InitializeSystems()
        {
            throwSystem.Initialize(_state, economyConfig, analyticsService);
            upgradeSystem.Initialize(_state, economyConfig, OnUpgradePurchased);
            prestigeSystem.Initialize(_state, prestigeConfig, OnPrestige);
            offlineCalculator.Initialize(_state);
            _spiritBonusMultiplier = 1 + _state.spiritTokens * prestigeConfig.spiritBonusPerToken;
        }

        private void Start()
        {
            var offlineRewards = offlineCalculator.CalculateOfflineRewards(economyConfig);
            if (offlineRewards > 0)
            {
                _pendingOfflineReward = offlineRewards;
                analyticsService.LogOfflineRewards(offlineRewards);
                EventBus.Publish(new OfflineRewardMessage(offlineRewards));
            }
        }

        private void Update()
        {
            var deltaScore = throwSystem.Tick(Time.deltaTime, GetThrowMultiplier());
            if (deltaScore > 0)
            {
                ApplyScore(deltaScore);
            }
        }

        public void ManualThrow()
        {
            var gained = throwSystem.ManualThrow(GetThrowMultiplier());
            ApplyScore(gained);
        }

        private double GetThrowMultiplier()
        {
            return _state.courseMultiplier * _spiritBonusMultiplier;
        }

        private void ApplyScore(double amount)
        {
            _state.currentScore += amount;
            _state.totalScore += amount;
            EventBus.Publish(new ScoreChangedMessage(_state.currentScore, throwSystem.PointsPerSecond));
        }

        public void GrantBonus(double amount)
        {
            ApplyScore(amount);
        }

        public bool CanAfford(double cost) => _state.currentScore >= cost;

        public void SpendScore(double cost)
        {
            _state.currentScore = Math.Max(0, _state.currentScore - cost);
            EventBus.Publish(new ScoreChangedMessage(_state.currentScore, throwSystem.PointsPerSecond));
        }

        public void Save()
        {
            _state.lastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _saveSystem.Save(_state);
        }

        public void ClaimOfflineReward(double amount)
        {
            if (_pendingOfflineReward <= 0)
            {
                return;
            }

            ApplyScore(amount);
            _pendingOfflineReward = 0;
        }

        private void Load()
        {
            if (!_saveSystem.TryLoad(out _state))
            {
                _state = new GameState
                {
                    currentScore = 0,
                    totalScore = 0,
                    gems = 0,
                    spiritTokens = 0,
                    lastActiveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Save();
            }
            else
            {
                offlineCalculator.Refresh();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void OnUpgradePurchased(EconomyConfig.UpgradeDefinition upgrade, int level)
        {
            switch (upgrade.type)
            {
                case EconomyConfig.UpgradeType.Accuracy:
                    throwSystem.SetAccuracy(1f + level * 0.01f);
                    break;
                case EconomyConfig.UpgradeType.Power:
                    throwSystem.SetPower(1 + level * 0.1f);
                    break;
                case EconomyConfig.UpgradeType.AutoThrow:
                    throwSystem.SetAutoThrowLevel(level);
                    break;
                case EconomyConfig.UpgradeType.CourseMultiplier:
                    _state.courseMultiplier = 1 + level * 0.1f;
                    break;
            }
        }

        private void OnPrestige(double tokensAwarded)
        {
            _state.spiritTokens += tokensAwarded;
            _spiritBonusMultiplier = 1 + _state.spiritTokens * prestigeConfig.spiritBonusPerToken;
            throwSystem.ResetProgress();
            upgradeSystem.ResetUpgrades();
            _state.currentScore = 0;
            _state.courseMultiplier = 1;
            _state.accuracyMultiplier = 1f;
            _state.powerMultiplier = 1f;
            analyticsService.LogPrestige(tokensAwarded, _state.spiritTokens);
        }
    }

    public readonly struct ScoreChangedMessage
    {
        public readonly double Current;
        public readonly double PerSecond;

        public ScoreChangedMessage(double current, double perSecond)
        {
            Current = current;
            PerSecond = perSecond;
        }
    }

    public readonly struct OfflineRewardMessage
    {
        public readonly double Amount;

        public OfflineRewardMessage(double amount)
        {
            Amount = amount;
        }
    }

    public readonly struct SessionStartedMessage { }
}
