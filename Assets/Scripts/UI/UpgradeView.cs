using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Game;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFrisbeeGolf.UI
{
    /// <summary>
    /// UI hook for upgrade buttons.
    /// </summary>
    public class UpgradeView : MonoBehaviour
    {
        [SerializeField] private string upgradeId;
        [SerializeField] private Text label;
        [SerializeField] private Text costText;
        [SerializeField] private Button buyButton;
        [SerializeField] private UpgradeSystem upgradeSystem;

        private readonly System.Action<ScoreChangedMessage> _scoreListener;
        private readonly System.Action<UpgradePurchasedMessage> _upgradeListener;

        public UpgradeView()
        {
            _scoreListener = _ => Refresh();
            _upgradeListener = OnUpgradePurchased;
        }

        private void Awake()
        {
            buyButton.onClick.AddListener(Purchase);
            Refresh();
        }

        private void OnEnable()
        {
            EventBus.Subscribe(_scoreListener);
            EventBus.Subscribe(_upgradeListener);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(_scoreListener);
            EventBus.Unsubscribe(_upgradeListener);
        }

        private void Purchase()
        {
            upgradeSystem.Purchase(upgradeId);
        }

        private void OnUpgradePurchased(UpgradePurchasedMessage message)
        {
            if (message.Id == upgradeId)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            var level = upgradeSystem.GetLevel(upgradeId);
            var cost = upgradeSystem.GetUpgradeCost(upgradeId);
            label.text = $"Lv {level}";
            costText.text = NumberFormatter.Format(cost);
        }
    }
}
