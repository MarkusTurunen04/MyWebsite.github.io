using System;
using IdleFrisbeeGolf.Data;
using IdleFrisbeeGolf.Game;
using NUnit.Framework;
using UnityEngine;
using IdleFrisbeeGolf.Meta;

namespace IdleFrisbeeGolf.Tests
{
    public class IdleGameTests
    {
        [Test]
        public void AutoThrowGeneratesScore()
        {
            var state = new GameState
            {
                autoThrowers = 1,
                accuracyMultiplier = 1f,
                powerMultiplier = 1f
            };
            var economy = ScriptableObject.CreateInstance<EconomyConfig>();
            economy.baseScorePerThrow = 10;
            economy.autoThrowBaseInterval = 1;
            var analytics = new GameObject("analytics").AddComponent<IdleFrisbeeGolf.Core.AnalyticsService>();
            var throwSystem = new GameObject("throw").AddComponent<ThrowSystem>();
            throwSystem.Initialize(state, economy, analytics);
            throwSystem.SetRandomGenerator(new System.Random(1));
            var result = throwSystem.Tick(1f, 1f);
            Assert.Greater(result, 0);
        }

        [Test]
        public void PrestigeCalculatesTokens()
        {
            var state = new GameState { totalScore = 4_000_000 };
            var config = ScriptableObject.CreateInstance<PrestigeConfig>();
            config.scoreDivider = 1e6;
            config.exponent = 0.5f;
            var prestige = new GameObject("prestige").AddComponent<PrestigeSystem>();
            prestige.Initialize(state, config, _ => { });
            Assert.AreEqual(2, prestige.PreviewTokens());
        }

        [Test]
        public void OfflineCappedAtEightHours()
        {
            var state = new GameState { autoThrowers = 2, lastActiveTimestamp = DateTimeOffset.UtcNow.AddHours(-10).ToUnixTimeSeconds() };
            var economy = ScriptableObject.CreateInstance<EconomyConfig>();
            economy.baseScorePerThrow = 10;
            economy.autoThrowBaseInterval = 1;
            var offline = new GameObject("offline").AddComponent<OfflineCalculator>();
            offline.Initialize(state);
            var reward = offline.CalculateOfflineRewards(economy);
            Assert.LessOrEqual(reward, 10 * 3600 * 8 * (1 + state.autoThrowers * 0.05f));
        }

        private class MockIAPManager : IIAPManager
        {
            public bool IsInitialized => true;
            public string LastPurchase;

            public void Initialize(System.Action<string> onPurchaseCompleted)
            {
            }

            public void PurchaseProduct(string productId)
            {
                LastPurchase = productId;
            }
        }

        private class MockAdsManager : IAdsManager
        {
            public bool RewardedReady = true;
            public bool InterstitialReady = true;
            public string LastRewarded;
            public string LastInterstitial;

            public bool IsRewardedReady(string placement) => RewardedReady;
            public void ShowRewarded(string placement) => LastRewarded = placement;
            public bool CanShowInterstitial() => InterstitialReady;
            public void ShowInterstitial(string placement) => LastInterstitial = placement;
            public void RequestInterstitial() { }
            public void Tick(float deltaTime) { }
        }

        [Test]
        public void AdsCanBeMocked()
        {
            var ads = new MockAdsManager();
            Assert.IsTrue(ads.IsRewardedReady("double_gain"));
            ads.ShowRewarded("double_gain");
            Assert.AreEqual("double_gain", ads.LastRewarded);
        }

        [Test]
        public void IapCanBeMocked()
        {
            var iap = new MockIAPManager();
            iap.PurchaseProduct("gems_small");
            Assert.AreEqual("gems_small", iap.LastPurchase);
        }
    }
}
