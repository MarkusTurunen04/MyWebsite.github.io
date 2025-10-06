using UnityEngine;

namespace IdleFrisbeeGolf.Core
{
    /// <summary>
    /// Simple wrapper for Firebase Analytics events.
    /// </summary>
    public class AnalyticsService : MonoBehaviour
    {
        public void LogSessionStart()
        {
            FirebaseAnalyticsHelper.LogEvent("session_start");
        }

        public void LogThrow(string result, bool manual)
        {
            if (manual)
            {
                FirebaseAnalyticsHelper.LogEvent("throw_manual", ("result", result));
            }
        }

        public void LogAdRewarded(string placement)
        {
            FirebaseAnalyticsHelper.LogEvent("ad_rewarded_shown", ("placement", placement));
        }

        public void LogAdInterstitial(string placement)
        {
            FirebaseAnalyticsHelper.LogEvent("ad_interstitial_shown", ("placement", placement));
        }

        public void LogIapPurchase(string productId)
        {
            FirebaseAnalyticsHelper.LogEvent("iap_purchase", ("product_id", productId));
        }

        public void LogPrestige(double earned, double total)
        {
            FirebaseAnalyticsHelper.LogEvent("prestige", ("earned", earned.ToString()), ("total", total.ToString()));
        }

        public void LogOfflineRewards(double amount)
        {
            FirebaseAnalyticsHelper.LogEvent("offline_rewards", ("amount", amount.ToString()));
        }
    }

    internal static class FirebaseAnalyticsHelper
    {
        public static void LogEvent(string name, params (string key, string value)[] parameters)
        {
#if FIREBASE_INSTALLED
            var bundle = new Firebase.Analytics.Parameter[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                bundle[i] = new Firebase.Analytics.Parameter(parameters[i].key, parameters[i].value);
            }
            Firebase.Analytics.FirebaseAnalytics.LogEvent(name, bundle);
#else
            Debug.Log($"Analytics Event: {name}");
#endif
        }
    }
}
