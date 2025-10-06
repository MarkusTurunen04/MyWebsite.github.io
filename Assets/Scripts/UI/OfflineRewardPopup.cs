using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Game;
using IdleFrisbeeGolf.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFrisbeeGolf.UI
{
    public class OfflineRewardPopup : MonoBehaviour
    {
        [SerializeField] private Text amountText;
        [SerializeField] private Button collectButton;
        [SerializeField] private Button watchAdButton;
        [SerializeField] private AdsManager adsManager;
        [SerializeField] private IdleManager idleManager;

        private double _amount;
        private bool _collected;

        private void Awake()
        {
            collectButton.onClick.AddListener(Collect);
            watchAdButton.onClick.AddListener(WatchAd);
            EventBus.Subscribe<IdleManager.OfflineRewardMessage>(OnOfflineReward);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<IdleManager.OfflineRewardMessage>(OnOfflineReward);
        }

        private void Update()
        {
            watchAdButton.interactable = adsManager.IsRewardedReady("offline_boost");
        }

        private void OnOfflineReward(IdleManager.OfflineRewardMessage message)
        {
            _amount = message.Amount;
            _collected = false;
            amountText.text = NumberFormatter.Format(_amount);
            watchAdButton.interactable = adsManager.IsRewardedReady("offline_boost");
            gameObject.SetActive(true);
        }

        private void Collect()
        {
            if (_collected)
            {
                return;
            }

            idleManager.ClaimOfflineReward(_amount);
            _collected = true;
            gameObject.SetActive(false);
        }

        private void WatchAd()
        {
            if (!adsManager.IsRewardedReady("offline_boost"))
            {
                return;
            }

            adsManager.ShowRewarded("offline_boost");
            var bonus = _amount;
            _amount += bonus;
            amountText.text = NumberFormatter.Format(_amount);
            watchAdButton.interactable = false;
        }
    }
}
