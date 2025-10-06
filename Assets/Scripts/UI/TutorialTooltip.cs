using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Game;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFrisbeeGolf.UI
{
    public class TutorialTooltip : MonoBehaviour
    {
        [SerializeField] private Text tooltipText;
        [SerializeField] private GameObject panel;
        [SerializeField] private IdleManager idleManager;
        [SerializeField] private UpgradeSystem upgradeSystem;
        [SerializeField] private PrestigeSystem prestigeSystem;

        private int _step;

        private void OnEnable()
        {
            _step = 0;
            panel.SetActive(true);
            tooltipText.text = "Tap to throw!";
            EventBus.Subscribe<ScoreChangedMessage>(OnScoreChanged);
            EventBus.Subscribe<UpgradePurchasedMessage>(OnUpgradePurchased);
            EventBus.Subscribe<PrestigeMessage>(OnPrestiged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ScoreChangedMessage>(OnScoreChanged);
            EventBus.Unsubscribe<UpgradePurchasedMessage>(OnUpgradePurchased);
            EventBus.Unsubscribe<PrestigeMessage>(OnPrestiged);
        }

        private void OnScoreChanged(ScoreChangedMessage _)
        {
            if (_step == 0)
            {
                _step = 1;
                tooltipText.text = "Buy an upgrade";
            }
        }

        private void OnUpgradePurchased(UpgradePurchasedMessage _)
        {
            if (_step <= 1)
            {
                _step = 2;
                tooltipText.text = "Prestige to earn Spirit Tokens";
            }
        }

        private void OnPrestiged(PrestigeMessage _)
        {
            panel.SetActive(false);
            enabled = false;
        }
    }
}
