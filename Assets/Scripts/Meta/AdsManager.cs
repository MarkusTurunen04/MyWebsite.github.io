using System.Collections.Generic;
using IdleFrisbeeGolf.Core;
using UnityEngine;

namespace IdleFrisbeeGolf.Meta
{
    /// <summary>
    /// AdMob/Unity Ads implementation behind interface for testability.
    /// </summary>
    public class AdsManager : MonoBehaviour, IAdsManager
    {
        private readonly Dictionary<string, float> _rewardedCooldowns = new();
        private float _interstitialTimer;
        private bool _interstitialAvailable;
        private bool _adsRemoved;
        private AnalyticsService _analytics;

        private void Awake()
        {
            _analytics = FindObjectOfType<AnalyticsService>();
        }

        public void Initialize(bool adsRemoved)
        {
            _adsRemoved = adsRemoved;
            _interstitialTimer = 60f;
            RequestInterstitial();
        }

        public bool IsRewardedReady(string placement)
        {
            return !_adsRemoved && (!_rewardedCooldowns.ContainsKey(placement) || _rewardedCooldowns[placement] <= 0f);
        }

        public void ShowRewarded(string placement)
        {
            if (!IsRewardedReady(placement))
            {
                return;
            }

            Debug.Log($"Showing rewarded ad: {placement}");
            _analytics.LogAdRewarded(placement);
            _rewardedCooldowns[placement] = placement == "double_gain" ? 1800f : 0f; // 30 minutes cooldown
        }

        public bool CanShowInterstitial()
        {
            return !_adsRemoved && _interstitialAvailable && _interstitialTimer <= 0f;
        }

        public void ShowInterstitial(string placement)
        {
            if (!CanShowInterstitial())
            {
                return;
            }

            Debug.Log($"Showing interstitial: {placement}");
            _analytics.LogAdInterstitial(placement);
            _interstitialTimer = 120f;
            _interstitialAvailable = false;
            RequestInterstitial();
        }

        public void RequestInterstitial()
        {
            if (_adsRemoved)
            {
                return;
            }

            _interstitialAvailable = true;
            Debug.Log("Interstitial requested");
        }

        public void Tick(float deltaTime)
        {
            if (_adsRemoved)
            {
                return;
            }

            if (_interstitialTimer > 0f)
            {
                _interstitialTimer -= deltaTime;
            }

            var keys = new List<string>(_rewardedCooldowns.Keys);
            foreach (var key in keys)
            {
                if (_rewardedCooldowns[key] <= 0f)
                {
                    continue;
                }

                _rewardedCooldowns[key] -= deltaTime;
            }
        }
    }
}
